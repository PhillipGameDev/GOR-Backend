// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GridWorld.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Grid used to divide the world.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace GameOfRevenge.GameHandlers
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ExitGames.Concurrency.Channels;

    using Newtonsoft.Json.Linq;
    using System.IO;
    using ExitGames.Logging;
    using Newtonsoft.Json;
    using ExitGames.Concurrency.Fibers;
    using System.Collections.Concurrent;
    using GameOfRevenge.Interface;
    using GameOfRevenge.Model;
    using GameOfRevenge.Common.Models.Kingdom;
    using GameOfRevenge.Common.Services;
    using GameOfRevenge.Common.Models;
    using GameOfRevenge.GameApplication;

    /// <summary>
    /// Grid used to divide the world.
    /// It contains Regions.
    /// </summary>
    /// //worldRow = X    20
    //worldColumn = y 20
    //tilesRow = A  10
    //tiles Row = B  10 
    //posDiff = i  1
    //Area = X*A*i * Y*B*1 = 20*10*1 * 20*10*1 = 200 * 200
    //tiles dimentions A * B * i = 10 * 10 * 1 = 10 * 10
    public class CountryGrid : IDisposable, IWorld
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        public int WorldId { get; set; }
        public string Name { get; set; }
        public Region[][] WorldRegions { get; set; }
        public BoundingBox Area { get; private set; }
        public Vector TileDimensions { get; private set; }
        public int TileX { get; private set; }
        public int TileY { get; private set; }
        public IPlayersManager PlayersManager { get; set; }
        public List<WorldDataTable> WorldData { get; private set; }
        public IFiber WorldFiber { get; private set; }
        private readonly Random rd = new Random();
        public CountryGrid(string name, BoundingBox area, Vector tileDimensions, int worldId, List<WorldDataTable> worldData)
        {
            try
            {
                this.WorldData = worldData;
                this.WorldId = worldId;
                this.Name = name;
                this.WorldFiber = new PoolFiber();
                this.WorldFiber.Start();
                this.PlayersManager = new PlayersManager();
                this.Area = area;
                this.TileDimensions = tileDimensions;
                this.TileX = (int)Math.Ceiling(Area.Size.X / (double)tileDimensions.X);
                this.TileY = (int)Math.Ceiling(Area.Size.Y / (double)tileDimensions.Y);
                this.WorldRegions = new Region[TileX][];
                for (int x = 0; x < TileX; x++)
                {
                    try
                    {
                        this.WorldRegions[x] = new Region[TileY];
                        for (int y = 0; y < TileY; y++)
                        {
                            try
                            {
                                this.WorldRegions[x][y] = new Region(x, y, TileDimensions, worldId, this);
                                var worldPlayer = worldData.Where(d => d.X == x && d.Y == y).FirstOrDefault();
                                if (worldPlayer != null)
                                {
                                    var actor = new MmoActor(worldPlayer.TileData.PlayerId.ToString(), this, this.WorldRegions[x][y]);
                                    this.WorldRegions[x][y].SpawnCityInTile(actor);
                                    this.PlayersManager.AddPlayer(worldPlayer.TileData.PlayerId.ToString(), actor);
                                }
                            }
                            catch (Exception ex)
                            {
                                log.InfoFormat("Exception in CountryGrid Constructor {0} {1} ", ex.Message, ex.StackTrace);
                            }

                        }
                    }
                    catch(Exception ex)
                    {
                        log.InfoFormat("Exception in CountryGrid Constructor {0} {1} ", ex.Message, ex.StackTrace);
                    }
                }
                GameService.BRealTimeUpdateManager.Update(InvokeAttackComplete);
            }
            catch(Exception ex)
            {
                log.InfoFormat("Exception in CountryGrid Constructor {0} {1} {2} ", ex.Message, ex.StackTrace,
                    JsonConvert.SerializeObject(worldData));
            }
        
        }
        public void InvokeAttackComplete(AttackStatusData data)
        {
            var attacker = this.PlayersManager.GetPlayer(data.Attacker.PlayerId.ToString());
            var defender = this.PlayersManager.GetPlayer(data.Defender.PlayerId.ToString());
            if (attacker != null && defender != null)
            {
                var result = new AttackResultResponse();
                if (data.WinnerPlayerId == attacker.PlayerId)
                    result.WinnerUserName = attacker.UserName;
                else
                    result.WinnerUserName = defender.UserName;
                attacker.SendEvent(EventCode.AttackResult, result);
                defender.SendEvent(EventCode.AttackResult, result);
            }
        }
        public Region SpawnPlayerInNewCity() // todo
        {
            bool isSpawn = false;
            do
            {
                int X = rd.Next(0, 10);
                int Y = rd.Next(0, 10);
                if (!this.WorldRegions[X][Y].IsBooked)
                {
                    isSpawn = true;
                    return this.WorldRegions[X][Y];
                }
            }
            while (!isSpawn);
            return null;
        }
        public (Region region, MmoActor actor, IInterestArea iA) GetPlayerPosition(int playerId)
        {
            IInterestArea iA = null;
            var actor = this.PlayersManager.GetPlayer(playerId.ToString());
            var data = this.WorldData.Where(d => d.TileData.PlayerId == playerId).FirstOrDefault();
            if (data == null && actor == null)
            {
                var tile = this.SpawnPlayerInNewCity();
                data = new WorldDataTable();
                data.WorldId = this.WorldId;
                data.X = tile.X;
                data.Y = tile.Y;
                data.TileData = new WorldTileData
                {
                    PlayerId = playerId
                };
                this.WorldData.Add(data);
                actor = new MmoActor(playerId.ToString(), this, this.WorldRegions[data.X][data.Y]);
                iA = new InterestArea(this.WorldRegions[data.X][data.Y], actor, true);
                this.WorldRegions[data.X][data.Y].SpawnCityInTile(actor);
                new DelayedAction().WaitForCallBack(() =>
                {
                    GameService.BKingdomManager.UpdateWorldTileData(this.WorldId, data.X, data.Y, data.TileData);
                }
                , 0);
                this.PlayersManager.AddPlayer(playerId.ToString(), actor);
            }
            else
                iA = new InterestArea(this.WorldRegions[data.X][data.Y], actor, false);
            return (this.WorldRegions[data.X][data.Y], actor, iA);
        }
        public double GetDistanceBw2Points(Region p1, Region p2)
        {
            double totalDistance = Math.Sqrt(Math.Pow((p2.X - p1.X), 2)
                  + Math.Pow((p2.Y - p1.Y), 2));
            return totalDistance;
        }
        public void Dispose()
        {
            this.WorldFiber.Dispose();
            this.PlayersManager.ClearAll();
            GC.SuppressFinalize(this);
        }
    }
}
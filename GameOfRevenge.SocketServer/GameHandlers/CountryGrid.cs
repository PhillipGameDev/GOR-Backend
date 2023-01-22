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
    using GameOfRevenge.Common.Interface;
    using GameOfRevenge.Business.Manager;
    using System.Threading.Tasks;

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

        private static readonly IAccountManager accountManager = new AccountManager();

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
                    this.WorldRegions[x] = new Region[TileY];
                    for (int y = 0; y < TileY; y++)
                    {
                        this.WorldRegions[x][y] = new Region(x, y, TileDimensions, worldId, this);
                        var worldPlayer = worldData.Find(wd => (wd.X == x) && (wd.Y == y));// Where(d => (d.X == x) && (d.Y == y)).FirstOrDefault();
                        if (worldPlayer == null) continue;

                        int plyId = worldPlayer.TileData.PlayerId;
                        PlayerInfo plyInfo = null;
                        var task = accountManager.GetAccountInfo(plyId);
                        task.Wait();

                        if (task.Result.IsSuccess && task.Result.HasData) plyInfo = task.Result.Data;

                        var actor = new MmoActor(plyId, plyInfo, this, this.WorldRegions[x][y]);
                        actor.Tile.SpawnCityInTile(actor);
                        this.PlayersManager.AddPlayer(plyId, actor);
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
            var attacker = PlayersManager.GetPlayer(data.Attacker.PlayerId);
            var defender = PlayersManager.GetPlayer(data.Defender.PlayerId);
            if (attacker != null && defender != null)
            {
                var winnerPly = (data.WinnerPlayerId == attacker.PlayerId) ? attacker : defender;
                var result = new AttackResultResponse();
                result.WinnerId = winnerPly.PlayerId;
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
        public (Region region, MmoActor actor, IInterestArea iA) GetPlayerPosition(int playerId, PlayerInfo playerInfo)
        {
            IInterestArea iA = null;
            var actor = this.PlayersManager.GetPlayer(playerId);
            var data = this.WorldData.Where(d => d.TileData.PlayerId == playerId).FirstOrDefault();
            if (data == null && actor == null)
            {
                var tile = this.SpawnPlayerInNewCity();
                data = new WorldDataTable()
                {
                    WorldId = this.WorldId,
                    X = tile.X,
                    Y = tile.Y,
                    TileData = new WorldTileData(playerId)
                };
                this.WorldData.Add(data);
                var worldRegion = this.WorldRegions[data.X][data.Y];
                actor = new MmoActor(playerId, playerInfo, this, worldRegion);
                iA = new InterestArea(worldRegion, actor, true);
                worldRegion.SpawnCityInTile(actor);
                new DelayedAction().WaitForCallBack(() =>
                {
                    GameService.BKingdomManager.UpdateWorldTileData(this.WorldId, data.X, data.Y, data.TileData);
                }
                , 0);
                this.PlayersManager.AddPlayer(playerId, actor);
            }
            else
            {
                iA = new InterestArea(this.WorldRegions[data.X][data.Y], actor, false);
            }
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
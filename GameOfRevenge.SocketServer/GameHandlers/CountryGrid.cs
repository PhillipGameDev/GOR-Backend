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
                log.InfoFormat("world size = {0},{1}", Area.Size.X, Area.Size.Y);
                log.InfoFormat("tile size = {0},{1}", tileDimensions.X, tileDimensions.Y);
                this.TileX = (int)Math.Ceiling(Area.Size.X / (double)tileDimensions.X);
                this.TileY = (int)Math.Ceiling(Area.Size.Y / (double)tileDimensions.Y);

                var count = 0;
                this.WorldRegions = new Region[TileX][];
                log.Info("region len = "+WorldRegions.Length);
                for (int x = 0; x < TileX; x++)
                {
                    this.WorldRegions[x] = new Region[TileY];
                    for (int y = 0; y < TileY; y++)
                    {
                        var worldRegion = new Region(x, y, TileDimensions, worldId, this);
                        var worldPlayer = worldData.Find(wd => (wd.X == x) && (wd.Y == y));
                        if (worldPlayer != null)
                        {
                            PlayerInfo plyInfo = null;
                            int playerId = worldPlayer.TileData.PlayerId;
                            var task = accountManager.GetAccountInfo(playerId);
                            task.Wait();
                            if (task.Result.IsSuccess && task.Result.HasData)
                            {
                                plyInfo = task.Result.Data;

                                var actor = new MmoActor(playerId, plyInfo, this, worldRegion);
                                actor.WorldRegion.SpawnPlayerInRegion(actor);
                                this.PlayersManager.AddPlayer(playerId, actor);

                                count++;
                            }
                            else
                            {
                                log.InfoFormat("player {0} missing for tile {1} ({2},{3})", playerId, worldPlayer.Id, worldPlayer.X, worldPlayer.Y);
                            }
                        }
                        this.WorldRegions[x][y] = worldRegion;
                    }
                }
                log.InfoFormat("Total users in world {0}", count);


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
        public Region FindFreeRegion()
        {
            int mapSize = WorldRegions.Length;
            //find randomly
            int xtries = 20;
            do
            {
                int x = rd.Next(0, mapSize);
                int vtries = 10;
                do
                {
                    int y = rd.Next(0, mapSize);
                    if (!WorldRegions[x][y].IsBooked)
                    {
                        return WorldRegions[x][y];
                    }
                    vtries--;
                } while (vtries > 0);
                xtries--;
            }
            while (xtries > 0);
            //lineal
            int total = mapSize * mapSize;
            int pos = rd.Next(0, total);
            do
            {
                pos++;
                int x = pos % mapSize;
                int y = (int)(pos / mapSize) % mapSize;
                if (!WorldRegions[x][y].IsBooked)
                {
                    return WorldRegions[x][y];
                }
                total--;
            } while (total > 0);


            return null;
        }

/*        public (Region region, MmoActor actor, IInterestArea iA) GetPlayerPosition(int playerId, PlayerInfo playerInfo)
        {
            IInterestArea iA = null;
            var actor = this.PlayersManager.GetPlayer(playerId);
            var data = this.WorldData.Find(d => (d.TileData.PlayerId == playerId));
            if (data == null && actor == null)
            {
                var tile = FindFreeRegion();
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
                worldRegion.SpawnPlayerInRegion(actor);
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
        }*/

        public (MmoActor actor, IInterestArea iA) GetPlayerPosition(int playerId, PlayerInfo playerInfo)
        {
            IInterestArea interestArea = null;
            var actor = this.PlayersManager.GetPlayer(playerId);
            var data = this.WorldData.Find(d => (d.TileData.PlayerId == playerId));
            if (data == null)
            {
                var region = FindFreeRegion();
                if (region != null)
                {
                    data = new WorldDataTable()
                    {
                        WorldId = this.WorldId,
                        X = region.X,
                        Y = region.Y,
                        TileData = new WorldTileData(playerId)
                    };
                    this.WorldData.Add(data);
                    var worldRegion = WorldRegions[data.X][data.Y];
                    actor = new MmoActor(playerId, playerInfo, this, worldRegion);
                    worldRegion.SpawnPlayerInRegion(actor);
                    new DelayedAction().WaitForCallBack(() =>
                    {
                        GameService.BKingdomManager.UpdateWorldTileData(this.WorldId, data.X, data.Y, data.TileData);
                    }
                    , 0);
                    this.PlayersManager.AddPlayer(playerId, actor);
                    interestArea = new InterestArea(worldRegion, actor, true);
                }
            }
            else
            {
                var worldRegion = WorldRegions[data.X][data.Y];
                interestArea = new InterestArea(worldRegion, actor, false);
            }

            return (actor, interestArea);
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
﻿// --------------------------------------------------------------------------------------------------------------------
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
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ExitGames.Logging;
    using ExitGames.Concurrency.Fibers;
    using Newtonsoft.Json;
    using GameOfRevenge.Common;
    using GameOfRevenge.Interface;
    using GameOfRevenge.Model;
    using GameOfRevenge.Common.Models.Kingdom;
    using GameOfRevenge.Common.Models;
    using GameOfRevenge.GameApplication;
    using GameOfRevenge.Common.Interface;
    using GameOfRevenge.Business.Manager;
    using GameOfRevenge.Business.Manager.UserData;
    using GameOfRevenge.Business.Manager.Base;
    using GameOfRevenge.Common.Models.Structure;

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
        private static readonly IPlayerDataManager manager = new PlayerDataManager();

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
                log.InfoFormat("tile xy = {0},{1}", TileX, TileY);

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
                                actor.WorldRegion.SetPlayerInRegion(actor);
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

                var marchingList = new List<MarchingArmy>();
                var taskMarching = manager.GetAllMarchingTroops();
                taskMarching.Wait();
                if (taskMarching.Result.IsSuccess && taskMarching.Result.HasData)
                {
                    count = 0;
                    var list = taskMarching.Result.Data;
                    foreach (var data in list)
                    {
                        MarchingArmy marching = null;
                        if (!string.IsNullOrEmpty(data.Value))
                        {
                            try
                            {
                                marching = JsonConvert.DeserializeObject<MarchingArmy>(data.Value);
                            }
                            catch (Exception ex)
                            {
                                log.Info("Error: "+ex.Message);
                                log.Info("Error deserializing marching army: " + data.Value);
                            }
                        }
                        if (marching == null) continue;

                        var attackerId = (int)data.Id;
                        var task = BaseUserDataManager.GetFullPlayerData(attackerId);
                        task.Wait();
                        if (!task.Result.IsSuccess || !task.Result.HasData)
                        {
                            //delete marching
                            continue;
//                                throw new DataNotExistExecption(task.Result.Message);
                        }
                        var attackerData = task.Result.Data;

                        var defenderId = marching.TargetPlayer;
                        task = BaseUserDataManager.GetFullPlayerData(defenderId);
                        task.Wait();
                        if (!task.Result.IsSuccess || !task.Result.HasData)
                        {
                            continue;
                        }
                        var defenderData = task.Result.Data;

                        byte watchLevel = 0;
                        foreach (var structures in defenderData.Structures)
                        {
                            if (structures.StructureType != StructureType.WatchTower) continue;

                            var watchTowers = structures.Buildings;
                            if (watchTowers?.Count > 0)
                            {
                                watchLevel = (byte)watchTowers.Max(x => x.CurrentLevel);
                            }
                            break;
                        }

                        var response = new AttackResponseData()
                        {
                            AttackerId = attackerId,
                            EnemyId = defenderId,

                            WatchLevel = watchLevel,

                            AttackerUsername = attackerData.PlayerName,
                            EnemyUsername = defenderData.PlayerName,
                            KingLevel = attackerData.King.Level,
//                                LocationX = location.X,
//                                LocationY = location.Y,

                            Troops = marching.TroopsToArray(),
                            Heroes = marching.HeroesToArray(attackerData.Heroes),

                            StartTime = marching.StartTime.ToUniversalTime().ToString("s") + "Z",// timestart,
                            ReachedTime = marching.ReachedTime,// reachedTime,
                            BattleDuration = marching.BattleDuration//battleDuration
                        };
                        var attackStatus = new AttackStatusData()
                        {
                            MarchingArmy = marching,
                            AttackData = response
                        };
//                            attackStatus.State = 1;

                        GameService.BRealTimeUpdateManager.AddNewAttackOnWorld(attackStatus);
                        count++;
                    }
                    log.InfoFormat("Total attacks running:{0}", count);
                }
                else
                {
                    log.Info("Error pulling marching troops");
                }

                GameService.BRealTimeUpdateManager.Update(InvokeAttackComplete);
            }
            catch(Exception ex)
            {
                log.InfoFormat("Exception in CountryGrid Constructor {0} {1} {2} ", ex.Message, ex.StackTrace,
                    JsonConvert.SerializeObject(worldData));
            }
        }

        void InvokeAttackComplete(AttackStatusData data, bool forAttacker)
        {
            var playerId = forAttacker? data.AttackData.AttackerId : data.AttackData.EnemyId;
            var actor = PlayersManager.GetPlayer(playerId);
            if (actor != null)
            {
                var result = new AttackResultResponse
                {
                    WinnerId = data.MarchingArmy.Report.AttackerWon? data.AttackData.AttackerId : data.AttackData.EnemyId
                };
                actor.SendEvent(EventCode.AttackResult, result);
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

        public async Task<(MmoActor actor, IInterestArea iA)> GetPlayerPositionAsync(int playerId, PlayerInfo playerInfo)
        {
            log.Info("GetPlayerPositionAsync START");
            IInterestArea interestArea = null;
            var actor = PlayersManager.GetPlayer(playerId);
            log.Info("actor =" + (actor != null));
            var data = WorldData.Find(d => (d.TileData.PlayerId == playerId));
            if (data == null)
            {
                log.Info("data null");

                var region = FindFreeRegion();
                if (region != null)
                {
                    log.Info("region ok");
                    var tileData = new WorldTileData(playerId);
                    var resp = await GameService.BKingdomManager.UpdateWorldTileData(WorldId, region.X, region.Y, tileData);
                    if (resp.IsSuccess && resp.HasData)
                    {
                        log.Info("ok set properties tile="+resp.Data.Id);
                        var setResp = await GameService.BAccountManager.SetProperties(playerId, worldTileId: resp.Data.Id);
                        if (setResp.IsSuccess)
                        {
                            log.Info("ok!!");
                            data = new WorldDataTable()
                            {
                                WorldId = WorldId,
                                X = region.X,
                                Y = region.Y,
                                TileData = tileData
                            };
                            WorldData.Add(data);

                            var worldRegion = WorldRegions[region.X][region.Y];
                            actor = new MmoActor(playerId, playerInfo, this, worldRegion);
                            worldRegion.SetPlayerInRegion(actor);
                            log.Info("ok!! 1");
                            PlayersManager.AddPlayer(playerId, actor);
                            log.Info("ok!! 2");

                            interestArea = new InterestArea(worldRegion, actor, true);
                        }
                    }
                }
            }
            else
            {
                log.Info("data found x="+data.X+"  "+data.Y);
                var worldRegion = WorldRegions[data.X][data.Y];
                interestArea = new InterestArea(worldRegion, actor, false);
            }

            log.Info("GetPlayerPositionAsync END");
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
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
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ExitGames.Logging;
    using ExitGames.Concurrency.Fibers;
    using Newtonsoft.Json;
    using GameOfRevenge.Common.Models.Kingdom;
    using GameOfRevenge.Common.Models;
    using GameOfRevenge.Common.Interface;
    using GameOfRevenge.Common.Models.Structure;
    using GameOfRevenge.Common.Models.PlayerData;
    using GameOfRevenge.Business.Manager;
    using GameOfRevenge.Business.Manager.UserData;
    using GameOfRevenge.Business.Manager.Base;
    using GameOfRevenge.Interface;
    using GameOfRevenge.Model;
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

        private static readonly IAccountManager accountManager = new AccountManager();
        private static readonly IPlayerDataManager manager = new PlayerDataManager();

        public int WorldId { get; private set; }
        public string Name { get; private set; }
        public int SizeX { get; private set; }
        public int SizeY { get; private set; }

        public Region[][] WorldRegions { get; set; }
        public Vector TileDimensions { get; private set; }
        public IPlayersManager PlayersManager { get; set; }
        public List<PlayerID> WorldPlayers { get; private set; }
        public IFiber WorldFiber { get; private set; }
        private readonly Random rd = new Random();

        public CountryGrid(WorldTable world, List<PlayerID> worldPlayers)
        {
            var w = GlobalConst.GetPopWorld(world.ZoneX, world.ZoneY);
            var worldId = world.Id;

            try
            {
                Name = w.worldName;
                SizeX = world.ZoneX * world.ZoneSize;
                SizeY = world.ZoneY * world.ZoneSize;
                log.InfoFormat("world size = {0},{1}", SizeX, SizeY);

                WorldId = worldId;
                WorldPlayers = worldPlayers;
                WorldFiber = new PoolFiber();
                WorldFiber.Start();

                var count = 0;
                PlayersManager = new PlayersManager();
                WorldRegions = new Region[SizeX][];
                for (int x = 0; x < SizeX; x++)
                {
                    WorldRegions[x] = new Region[SizeY];
                    for (int y = 0; y < SizeY; y++)
                    {
                        var worldRegion = new Region(x, y, TileDimensions, worldId, this);
                        WorldRegions[x][y] = worldRegion;

                        var worldPlayer = WorldPlayers.Find(wd => (wd.X == x) && (wd.Y == y));
                        if (worldPlayer == null) continue;

                        int playerId = worldPlayer.PlayerId;
                        var task = accountManager.GetAccountInfo(playerId);
                        task.Wait();
                        if (task.Result.IsSuccess && task.Result.HasData)
                        {
                            var plyInfo = task.Result.Data;
                            var actor = new MmoActor(playerId, plyInfo, this, worldRegion);
                            actor.WorldRegion.SetPlayerInRegion(actor);
                            PlayersManager.AddPlayer(playerId, actor);
                            count++;
                        }
                        else
                        {
                            log.InfoFormat("player data {0} missing for tile ({1},{2})", playerId, x, y);
                        }
                    }
                }
                log.InfoFormat("Total users in world {0}", count);

                InstantiateMarchingTroops();

                GameService.BRealTimeUpdateManager.Update(MarchingArmyComplete);
            }
            catch(Exception ex)
            {
                log.InfoFormat("Exception in CountryGrid Constructor {0} {1} {2} ", ex.Message, ex.StackTrace,
                    JsonConvert.SerializeObject(worldPlayers));
                throw ex;
            }
        }

        void InstantiateMarchingTroops()
        {
            var marchingList = new List<MarchingArmy>();
            var taskMarching = manager.GetAllMarchingTroops();
            taskMarching.Wait();
            if (taskMarching.Result.IsSuccess && taskMarching.Result.HasData)
            {
                var count = 0;
                var list = taskMarching.Result.Data;
                log.Info("marching troops = " + list.Count);
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
                            log.Error("Error: " + ex.Message);
                            log.Info("Error deserializing marching army: " + data.Value);
                        }
                    }
                    if (marching == null) continue;

                    marching.MarchingId = data.Id;
                    var attackerId = data.PlayerId;
                    var task = BaseUserDataManager.GetFullPlayerData(attackerId);
                    task.Wait();
                    if (!task.Result.IsSuccess || !task.Result.HasData)
                    {
                        log.Error("Error: attacker " + attackerId + " data not found");
                        continue;
                    }
                    var attackerData = task.Result.Data;

                    var defenderId = marching.TargetId;
                    task = BaseUserDataManager.GetFullPlayerData(defenderId);
                    task.Wait();
                    if (!task.Result.IsSuccess || !task.Result.HasData)
                    {
                        log.Error("Error: defender " + defenderId + " data not found");
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
                        MarchingId = marching.MarchingId,
                        MarchingType = marching.MarchingType,

                        AttackerId = attackerId,
                        AttackerName = attackerData.PlayerName,

                        KingLevel = attackerData.King.Level,
                        WatchLevel = watchLevel,

                        TargetId = defenderId,
                        TargetName = defenderData.PlayerName,

                        Troops = marching.TroopsToArray(),
                        Heroes = marching.HeroesToArray(attackerData.Heroes),

                        StartTime = marching.StartTime.ToUniversalTime().ToString("s") + "Z",
                        Distance = marching.Distance,
                        AdvanceReduction = marching.AdvanceReduction,
                        ReturnReduction = marching.ReturnReduction,
                        Duration = marching.Duration
                    };
                    var attackStatus = new AttackStatusData()
                    {
                        MarchingArmy = marching,
                        AttackData = response
                    };
                    //                            attackStatus.State = 1;

                    GameService.BRealTimeUpdateManager.AddNewMarchingArmy(attackStatus);
                    count++;
                }
                log.InfoFormat("Total attacks running:{0}", count);
            }
            else
            {
                log.Info("Error pulling marching troops");
            }
        }

        void MarchingArmyComplete(AttackStatusData data, int notify)
        {
            var marchingArmy = data.MarchingArmy;
            var result = new MarchingResultResponse()
            {
                MarchingId = marchingArmy.MarchingId,
                MarchingType = marchingArmy.MarchingType.ToString(),
                AttackerId = data.AttackData.AttackerId,
                AttackerName = data.AttackData.AttackerName,
                TargetId = data.AttackData.TargetId,
                TargetName = data.AttackData.TargetName
            };
            if (!marchingArmy.IsRecalling)
            {
                if ((marchingArmy.MarchingType == MarchingType.AttackPlayer) ||
                    (marchingArmy.MarchingType == MarchingType.AttackMonster))
                {
                    if (marchingArmy.Report != null) result.WinnerId = marchingArmy.Report.WinnerId;
                }
                else if (marchingArmy.MarchingType == MarchingType.ReinforcementPlayer)
                {
                    result.WinnerId = data.AttackData.TargetId;
                }
            }

            var notifyAttacker = (notify != RealTimeUpdateManager.NOTIFY_TARGET);
            var playerId = notifyAttacker ? result.AttackerId : result.TargetId;
            var actor = PlayersManager.GetPlayer(playerId);
            if (actor != null) actor.SendEvent(EventCode.MarchingResult, result);

            if (notify == RealTimeUpdateManager.NOTIFY_ALL)
            {
                actor = PlayersManager.GetPlayer(result.TargetId);
                if (actor != null) actor.SendEvent(EventCode.MarchingResult, result);
            }

            if (marchingArmy.MarchingType == MarchingType.ReinforcementPlayer)
            {
//                await UpdatePlayerData(playerData);
//                await UpdatePlayerData(targetPlayerData);
            }

            if (notifyAttacker)
            {
                var task = manager.UpdatePlayerDataID(result.AttackerId, marchingArmy.MarchingId, string.Empty);
                task.Wait();
                if (!task.Result.IsSuccess)
                {
                    log.Debug(task.Result.Message);
                }
            }
        }

/*        public Region FindFreeRegion()
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
                int y = (int)Math.Ceiling(pos / (float)mapSize);
                if (!WorldRegions[x][y].IsBooked)
                {
                    return WorldRegions[x][y];
                }
                total--;
            } while (total > 0);


            return null;
        }*/

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
            log.Info("worlddata = "+(WorldPlayers != null));
            var data = WorldPlayers.Find(wd => (wd.PlayerId == playerId));
            if (data == null)
            {
                log.Info("data null");
                var plyResp = await GameService.BAccountManager.GetAllPlayerIDs(playerId: playerId, length: 1);
                if (plyResp.IsSuccess && plyResp.HasData && (plyResp.Data.Count > 0))
                {
                    var plyData = plyResp.Data[0];
                    var worldRegion = WorldRegions[plyData.X][plyData.Y];

                    actor = new MmoActor(playerId, playerInfo, this, worldRegion);
                    worldRegion.SetPlayerInRegion(actor);
                    PlayersManager.AddPlayer(playerId, actor);
                    WorldPlayers.Add(plyData);

                    interestArea = new InterestArea(worldRegion, actor, true);
                }
            }
            else
            {
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
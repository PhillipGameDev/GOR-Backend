﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using ExitGames.Logging;
using GameOfRevenge.Interface;
using GameOfRevenge.Common.Models.Kingdom;
using GameOfRevenge.GameApplication;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Common.Models.Monster;
using System.Threading.Tasks;

namespace GameOfRevenge.GameHandlers
{
    public class WorldHandler : IWorldHandler
    {
        public static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentDictionary<int, IWorld> worlds;
        public ConcurrentDictionary<int, IWorld> Worlds => worlds;

        public IWorld DefaultWorld
        {
            get
            {
                if (worlds.Count > 0) return worlds.ElementAt(0).Value; else return default;
            }
        }

        public WorldHandler()
        {
            worlds = new ConcurrentDictionary<int, IWorld>();
        }

        public const int MONSTERS_PER_WORLD = 1000;
        public const int MONSTERS_PER_TILE = 3;

        public async Task AddMonsters(List<MonsterTable> monsters, WorldTable world, int zone)
        {
            int offsetX = zone % world.ZoneX;
            int offsetY = zone / world.ZoneY;

            var random = new Random();

            for (int x = 0; x < world.ZoneSize; x++)
            {
                for (int y = 0; y < world.ZoneSize; y++)
                {
                    int finalX = offsetX + x, finalY = offsetY + y;

                    for (int l = monsters.FindAll(m => m.X == finalX && m.Y == finalY).Count; l < MONSTERS_PER_TILE; l++)
                    {
                        var ms = await GameService.BMonsterManager.AddNewMonster(world, monsters, finalX, finalY, random);
                        monsters.Add(new MonsterTable()
                        {
                            X = ms.Item1,
                            Y = ms.Item2
                        });
                    }
                }
            }
        }

        public async Task<List<MonsterTable>> GetMonsters(WorldTable world)
        { 
            var resp = await GameService.BMonsterManager.GetMonsterWorldData(world.Id);
            if (!resp.IsSuccess || !resp.HasData) throw new Exception(resp.Message);

            var currentMonsters = resp.Data;

            log.Info("--- PREPARE MONSTER ---: " + world.Id + "," + CacheMonsterManager.AllItems.Count);

            if (currentMonsters.Count >= (world.CurrentZone + 1) * world.ZoneSize * world.ZoneSize * MONSTERS_PER_TILE) return currentMonsters;

            for (int zone = 0; zone <= world.CurrentZone; zone++)
            {
                await AddMonsters(currentMonsters, world, zone);
            }

            resp = await GameService.BMonsterManager.GetMonsterWorldData(world.Id);
            if (!resp.IsSuccess || !resp.HasData) throw new Exception(resp.Message);

            return resp.Data;
        }

        public async Task AddNewZone(WorldTable worldTable, int zoneId)
        {
            var world = Worlds[worldTable.Id];

            await GameService.BKingdomManager.UpdateWorld(worldTable.Id, zoneId);
            await AddMonsters(world.WorldMonsters, worldTable, zoneId);

            var resp = await GameService.BKingdomManager.AddZoneFortress(worldTable.Id, zoneId);
            if (resp.IsSuccess && resp.HasData)
            {
                world.WorldForts.Add(resp.Data);
            }
        }

        public void SetupWorld(string worldCode)
        {
            var task = GameService.BKingdomManager.GetWorld(worldCode);
            task.Wait();
            if (!task.Result.IsSuccess || !task.Result.HasData) throw new Exception(task.Result.Message);

            var world = task.Result.Data;
            if (!worlds.ContainsKey(world.Id))
            {
                WorldGrid countryGrid;

                var monsterTask = GetMonsters(world);
                monsterTask.Wait();
                if (monsterTask.Result == null) throw new Exception("Error to get monsters");

                log.Debug("MONSTERS -- " + monsterTask.Result.Count);

                if (world.CurrentZone == -1)
                {
                    var task1 = GameService.BKingdomManager.GetWorldTilesData(world.Id);
                    task1.Wait();
                    var worldData = task1.Result.Data;
                    foreach (var wt in worldData)
                    {
                        var task2 = GameService.BKingdomManager.UpdateWorldTileData(-Math.Abs(wt.X), -Math.Abs(wt.Y), tileId: wt.WorldTileId);
                        task2.Wait();
                    }

                    var task3 = GameService.BAccountManager.GetAllPlayerIDs(includeTileId: true);
                    task3.Wait();
                    var playerIDs = task3.Result.Data;

                    var players = new List<Player>();
                    foreach (var player in playerIDs)
                    {
                        var plyId = player.PlayerId;
                        if (player.WorldTileId == 0)
                        {
                            log.Info("tile data missing for player "+ plyId);
                            continue;
                        }

                        var task4 = GameService.BAccountManager.GetAccountInfo(plyId);
                        task4.Wait();
                        if (task4.Result.IsSuccess && task4.Result.HasData)
                        {
                            var tile = worldData.Find(wt => (wt.WorldTileId == player.WorldTileId));
                            if (tile == null)
                            {
                                log.Info(".tile data missing for player " + plyId);
                                continue;
                            }

                            players.Add(new Player()
                            {
                                PlayerId = plyId,
                                WorldTileId = tile.WorldTileId,
                                X = player.X,
                                Y = player.Y,
                                Info = task4.Result.Data
                            });
                        }
                        else
                        {
                            log.Info("Error loading player " + plyId + " properties");
                        }
                    }

                    countryGrid = new WorldGrid(world, DistributePlayers(players, world), monsterTask.Result, null);
                }
                else
                {
                    List<ZoneFortressTable> allForts = null;
                    var task1 = GameService.BKingdomManager.GetAllZoneFortress();
                    task1.Wait();
                    if (task1.Result.IsSuccess && task1.Result.HasData)
                    {
                        allForts = task1.Result.Data;
                        log.Info("Zone fortress = " + allForts.Count);
                        foreach (var fortress in allForts)
                        {
                            var fortressDataResp = GameService.BKingdomManager.GetZoneFortressById(fortress.ZoneFortressId);
                            fortressDataResp.Wait();
                            var fortressData = fortressDataResp.Result.Data;

                            var playerData = new PlayerCompleteData() { Troops = fortressData.GetAllTroops() };
                            var power = new BattlePower(playerData, null, CacheTroopDataManager.GetFullTroopData, null, (str) => log.Info(str));
                            if ((fortress.HitPoints != power.HitPoints) ||
                                (fortress.Attack != power.Attack) || (fortress.Defense != power.Defense))
                            {
                                fortress.HitPoints = power.HitPoints;
                                fortress.Attack = power.Attack;
                                fortress.Defense = power.Defense;
                                var update = GameService.BKingdomManager.UpdateZoneFortress(fortress.ZoneFortressId,
                                            hitPoints: power.HitPoints, attack: power.Attack, defense: power.Defense);
                                update.Wait();
                            }
                        }
                        log.Info("Load all fortress");
                    }
                    else
                    {
                        log.Info("No zone fortress - "+task1.Result.Message);
                    }

                    var task2 = GameService.BAccountManager.GetAllPlayerIDs();
                    task2.Wait();
                    if (task2.Result.IsSuccess && task2.Result.HasData)
                    {
                        countryGrid = new WorldGrid(world, task2.Result.Data, monsterTask.Result, allForts);
                    }
                    else
                    {
                        throw new Exception("Error loading players "+task2.Result.Message);
                    }
                }
                worlds.TryAdd(world.Id, countryGrid);
            }
            //            AddNewCityOnWorld(worldId, pvpWorld.worldName, pvpWorld.boundingBox, pvpWorld.tileDimention, worldData);
        }

/*        public void AddNewCityOnWorld(int worldId, string worldName, BoundingBox boundingBox, Vector tiles, List<WorldDataTable> worldData)
        {
            if (!worlds.TryGetValue(worldId, out IWorld world))
            {
                world = new CountryGrid(worldName, boundingBox, tiles, worldId, worldData);
                worlds.TryAdd(worldId, world);
            }
        }*/

        List<PlayerID> DistributePlayers(List<Player> players, WorldTable world)
        {
            var invadedPlayers = new List<Player>();
            var nonInvadedPlayers = new List<Player>();

            foreach (var ply in players)
            {
                if ((DateTime.UtcNow - ply.Info.LastLogin).TotalDays > (30 * 6))
                {
                    invadedPlayers.Add(ply);
                }
                else
                {
                    nonInvadedPlayers.Add(ply);
                }
            }
            log.Info("invaded ="+invadedPlayers.Count);
            log.Info("non invaded =" + nonInvadedPlayers.Count);

            var totalPlayers = players.Count;
            var playersPerZone = (world.ZoneSize * world.ZoneSize) - 4;
            var numZonesUsed = (int)Math.Ceiling(totalPlayers / (double)playersPerZone);
            var invadedPerZone = invadedPlayers.Count / numZonesUsed;

            var distributedPlayers = new List<PlayerID>();
            var zonePlayers = new List<PlayerID>();
            var currZoneX = 0;
            var currZoneY = 0;
            var playerCount = playersPerZone;
            var invadedCount = invadedPerZone;
            for (int num = 0; num < totalPlayers; num++)
            {
                Player plyData = null;
                if ((invadedPlayers.Count > 0) && (invadedCount > 0))
                {
                    plyData = invadedPlayers.First();
                    invadedPlayers.RemoveAt(0);
                    invadedCount--;
                }
                else if (nonInvadedPlayers.Count > 0)
                {
                    plyData = nonInvadedPlayers.First();
                    nonInvadedPlayers.RemoveAt(0);
                }
                if (plyData == null) continue;

                var data = GameService.BAccountManager.AddPlayerToZone(plyData.PlayerId, world.ZoneSize, zonePlayers);
                if (data != null)
                {
                    var task = GameService.BKingdomManager.UpdateWorldTileData(data.X, data.Y, tileId: plyData.WorldTileId);
                    task.Wait();
                    if (!task.Result.IsSuccess)
                    {
                        log.Info("error updating tile " + task.Result.Message);
                    }
                }
                else
                {
                    log.Info("Unable to assign world position for player "+plyData.PlayerId);
                }

                playerCount--;
                if (playerCount == 0)
                {
                    foreach (var zonePly in zonePlayers)
                    {
                        zonePly.X += currZoneX * world.ZoneSize;
                        zonePly.Y += currZoneY * world.ZoneSize;
                        distributedPlayers.Add(zonePly);
                    }
                    zonePlayers.Clear();

                    currZoneX++;
                    if (currZoneX >= world.ZoneSize)
                    {
                        currZoneX = 0;
                        currZoneY++;
                    }
                    playerCount = playersPerZone;
                    invadedCount = invadedPerZone;
                }
            }
            var zone = (currZoneY * world.ZoneX) + currZoneX;
            world.CurrentZone = (short)zone;
            var task2 = GameService.BKingdomManager.UpdateWorld(world.Id, zone);
            task2.Wait();
            log.Info("regorganized = " + distributedPlayers.Count);

            return distributedPlayers;
        }
    }
}

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using ExitGames.Logging;
using GameOfRevenge.Interface;
using GameOfRevenge.Common.Models.Kingdom;
using GameOfRevenge.GameApplication;
using GameOfRevenge.Common.Models;

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

        public void SetupWorld(string worldCode)
        {
            var task = GameService.BKingdomManager.GetWorld(worldCode);
            task.Wait();
            if (!task.Result.IsSuccess || !task.Result.HasData) throw new Exception(task.Result.Message);

            var world = task.Result.Data;
            if (!worlds.ContainsKey(world.Id))
            {
                CountryGrid countryGrid;
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

                    countryGrid = new CountryGrid(world, DistributePlayers(players, world), null);
                }
                else
                {
                    var task2 = GameService.BAccountManager.GetAllPlayerIDs();
                    task2.Wait();
                    if (task2.Result.IsSuccess && task2.Result.HasData)
                    {
                        countryGrid = new CountryGrid(world, task2.Result.Data, null);
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

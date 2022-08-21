using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Logging;
using GameOfRevenge.Interface;
using GameOfRevenge.Common.Models.Kingdom;
using GameOfRevenge.Model;

namespace GameOfRevenge.GameHandlers
{
    public class WorldHandler : IWorldHandler
    {
        private readonly ConcurrentDictionary<int, IWorld> worlds;
        public ConcurrentDictionary<int, IWorld> Worlds => worlds;
        public IWorld DefaultWorld { get { if (worlds.Count > 0) return worlds.ElementAt(0).Value; else return default; } }

        public WorldHandler()
        {
            worlds = new ConcurrentDictionary<int, IWorld>();
        }

        public void SetupPvpWorld(int worldId, List<WorldDataTable> worldData)
        {
            var pvpWorld = GlobalConst.GetPopWorld();
            AddNewCityOnWorld(worldId, pvpWorld.worldName, pvpWorld.boundingBox, pvpWorld.tileDimention, worldData);
        }

        public void AddNewCityOnWorld(int worldId, string worldName, BoundingBox boundingBox, Vector tiles, List<WorldDataTable> worldData)
        {
            if (!worlds.TryGetValue(worldId, out IWorld world))
            {
                world = new CountryGrid(worldName, boundingBox, tiles, worldId, worldData);
                worlds.TryAdd(worldId, world);
            }
        }
    }
}

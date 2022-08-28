using GameOfRevenge.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameOfRevenge.Common;
using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.Buildings.Handlers;
using GameOfRevenge.ResourcesHandler;
using ExitGames.Logging;
using Newtonsoft.Json;
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.GameApplication;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Interface.Model;

namespace GameOfRevenge.GameHandlers
{
    public class PlayerSocketDataManager : IPlayerSocketDataManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        public readonly List<PlayerDataTable> playerData;
        public readonly MmoActor player;

        public Dictionary<StructureType, List<IPlayerBuildingManager>> PlayerBuildings { get; private set; }
        public Dictionary<ResourceType, IPlayerResources> PlayerResources { get; private set; }
        public IPlayerAttackHandler AttackHandler { get; set; }

        public PlayerSocketDataManager(List<PlayerDataTable> playerData, MmoActor player)
        {
            this.AttackHandler = new PlayerAttackHandler(player);
            this.PlayerBuildings = new Dictionary<StructureType, List<IPlayerBuildingManager>>();
            this.PlayerResources = new Dictionary<ResourceType, IPlayerResources>();
            this.playerData = playerData;
            this.player = player;
            this.DataContributeAccType();
        }
        public void DataContributeAccType()
        {
            foreach (var item in playerData)
            {
                if (item.DataType == DataType.Structure)
                    this.AddStructureOnPlayer(item);
                else if (item.DataType == DataType.Resource)
                    this.AddResourcesOnPlayer(item);
                else if (item.DataType == DataType.Troop)
                    this.AddTroopOnPlayerBuilding(item);
            }
        }
        public void AddTroopOnPlayerBuilding(PlayerDataTable data)
        {
            var troops = GameService.BPlayerResourceManager.PlayerDataToUserTroopData(data);
            if (troops != null)
            {
                log.InfoFormat("Player Data Convert to Troop {0} playerData {1} ",
                  JsonConvert.SerializeObject(troops), JsonConvert.SerializeObject(data));
                foreach (var troop in troops.Value)
                {
                    if (troop.InTraning != null)
                    {
                        foreach (var trainer in troop.InTraning)
                        {
                            var building = GetPlayerBuildingByLocationId(trainer.BuildingLocId);
                            if (building != null)
                            {
                                var troop1 = GameService.GameBuildingManager[building.StructureType]
                                    .Troops[troops.ValueId].AddTroopOnPlayerBuilding(troops.ValueId, building, this.player);
                                troop1.Init(troops);
                            }
                        }
                    }
                }

                //troop.
            }
        }
        public void AddResourcesOnPlayer(PlayerDataTable data)
        {
            var resource = GameService.BPlayerResourceManager.PlayerDataToUserResourceData(data);
            if (resource != null)
            {
                log.InfoFormat("Player Data Convert to resource {0} playerData {1} ",
                    JsonConvert.SerializeObject(resource), JsonConvert.SerializeObject(data));
                IPlayerResources r = null;
                switch (resource.ValueId)
                {
                    case ResourceType.Food:
                        r = new PlayerResources(CacheResourceDataManager.Food, resource.Value, this.player);
                        break;
                    case ResourceType.Gems:
                        r = new PlayerResources(CacheResourceDataManager.Gems, resource.Value, this.player);
                        break;
                    case ResourceType.Ore:
                        r = new PlayerResources(CacheResourceDataManager.Ore, resource.Value, this.player);
                        break;
                    case ResourceType.Wood:
                        r = new PlayerResources(CacheResourceDataManager.Wood, resource.Value, this.player);
                        break;
                    default:
                        break;
                }
                if (r != null)
                {
                    log.InfoFormat("Add nerw resources on player account Resource {0} ", resource.ValueId.ToString());
                    PlayerResources.Add(resource.ValueId, r);
                    return;
                }
            }
            else
                log.InfoFormat("Reource not founf when convert player data to resouse info {0} ", JsonConvert.SerializeObject(playerData));
            return;
        }
        public void AddStructureOnPlayer(PlayerDataTable data)
        {
            var structure = GameService.BPlayerStructureManager.PlayerDataToUserStructureData(data);
            if (structure != null)
            {
                log.InfoFormat("Player Data Convert to structure {0} playerData {1} ",
                    JsonConvert.SerializeObject(structure), JsonConvert.SerializeObject(data));
                var multipleBuildings = GameService.BPlayerStructureManager.GetMultipleBuildings(structure);
                if (!this.PlayerBuildings.ContainsKey(structure.ValueId))
                {
                    foreach (var build in multipleBuildings)
                    {
                        IGameBuildingManager gameBuilding = GameService.GameBuildingManager[structure.ValueId];
                        this.AddStructure(build.Key, build.Value, gameBuilding);
                    }
                }
                else
                    log.InfoFormat("that structure is already defined in player structureType {0} ", structure.ValueId.ToString());
            }
            else
                log.InfoFormat("structure is null when convert to user structure line 97");
        }
        public void AddStructure(int locationId, UserStructureData structure, IGameBuildingManager gameBuilding)
        {
            IPlayerBuildingManager playerBuilding = null;
            switch (structure.ValueId)
            {
                case StructureType.CityCounsel:
                    playerBuilding = new CityCounsilBuilding(gameBuilding, this.player, structure);
                    break;
                case StructureType.WatchTower:
                    playerBuilding = new WatchTower(gameBuilding, this.player, structure);
                    break;
                case StructureType.Blacksmith:
                    playerBuilding = new BlackSmith(gameBuilding, this.player, structure);
                    break;
                case StructureType.Barracks:
                    playerBuilding = new BarracksBuilding(gameBuilding, this.player, structure);
                    break;
                case StructureType.Academy:
                    playerBuilding = new Academy(gameBuilding, this.player, structure);
                    break;
                case StructureType.Embassy:
                    playerBuilding = new Embassy(gameBuilding, this.player, structure);
                    break;
                case StructureType.Farm:
                    playerBuilding = new Farm(gameBuilding, this.player, structure);
                    break;
                case StructureType.Gate:
                    playerBuilding = new Gate(gameBuilding, this.player, structure);
                    break;
                case StructureType.InfantryCamp:
                    playerBuilding = new InfantryCamp(gameBuilding, this.player, structure);
                    break;
                case StructureType.Infirmary:
                    playerBuilding = new Infirmary(gameBuilding, this.player, structure);
                    break;
                case StructureType.Market:
                    playerBuilding = new Market(gameBuilding, this.player, structure);
                    break;
                case StructureType.Mine:
                    playerBuilding = new Mine(gameBuilding, this.player, structure);
                    break;
                case StructureType.Sawmill:
                    playerBuilding = new SawMill(gameBuilding, this.player, structure);
                    break;
                case StructureType.ShootingRange:
                    playerBuilding = new ShootingRange(gameBuilding, this.player, structure);
                    break;
                case StructureType.Warehouse:
                    playerBuilding = new WareHouse(gameBuilding, this.player, structure);
                    break;
                case StructureType.Workshop:
                    playerBuilding = new Workshop(gameBuilding, this.player, structure);
                    break;
                case StructureType.Stable:
                    playerBuilding = new Stable(gameBuilding, this.player, structure);
                    break;
                case StructureType.Other:
                    break;
                case StructureType.TrainingHeroes:
                    break;
                default:
                    break;
            }
            if (playerBuilding != null)
            {
                log.InfoFormat("Add New Structure On Player Account {0} ", structure.ValueId.ToString());
                List<IPlayerBuildingManager> list = null;
                if (!this.PlayerBuildings.ContainsKey(structure.ValueId))
                {
                    list = new List<IPlayerBuildingManager>();
                    this.PlayerBuildings.Add(structure.ValueId, list);
                }
                else
                    list = this.PlayerBuildings[structure.ValueId];
                list.Add(playerBuilding);
            }
        }
        public (bool succ, string msg) CheckRequirmentsAndUpdateValues(IReadOnlyList<IReadOnlyDataRequirement> requirments)
        {
            foreach (var item in requirments)
            {
                if (item.DataType == DataType.Structure)
                {
                    if (this.PlayerBuildings.ContainsKey((StructureType)item.ValueId))
                    {
                        bool isSucc = this.PlayerBuildings[(StructureType)item.ValueId].Any(d => d.HasAvailableRequirment(item));
                        if (!isSucc) return (false, "Requirment not found.");
                    }
                    else return (false, "Player need to create structure.");
                }
                else if (item.DataType == DataType.Resource)
                {
                    if (this.PlayerResources.ContainsKey((ResourceType)item.ValueId))
                    {
                        bool isSucc = this.PlayerResources[(ResourceType)item.ValueId].HasAvailableRequirment(item);
                        if (!isSucc)
                            return (false, "Requirment not found.");
                    }
                    else
                        return (false, "Player have insufficient resources.");
                }
            }
            foreach (var item in requirments)
                if (item.DataType == DataType.Resource)
                    this.PlayerResources[(ResourceType)item.ValueId].UpdateResourceValue(-item.Value);
            return (true, "Success");
        }
        public IPlayerBuildingManager GetPlayerBuilding(StructureType structType, int locationId)
        {
            if (this.PlayerBuildings.ContainsKey(structType))
                return player.PlayerDataManager.PlayerBuildings[structType].Where(d => d.Location == locationId).FirstOrDefault();
            return null;
        }
        public IPlayerBuildingManager GetPlayerBuilding(int structType, int locationId)
        {
            return this.GetPlayerBuilding((StructureType)structType, locationId);
        }
        public IPlayerBuildingManager GetPlayerBuildingByLocationId(int locationId)
        {
            foreach (var buildings in this.PlayerBuildings)
            {
                if (buildings.Value.Any(d => d.Location == locationId))
                    return buildings.Value.Where(d => d.Location == locationId).FirstOrDefault();
            }
            return null;
        }
        public void Dispose()
        {
            this.PlayerBuildings.Clear();
            this.PlayerResources.Clear();
            this.playerData.Clear();
            GC.SuppressFinalize(this);
        }
    }
}

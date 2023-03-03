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
using GameOfRevenge.Business.Manager.UserData;

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
//        public UserKingDetails King { get; set; }
//        public List<UserRecordBuilderDetails> Builders { get; set; }

        public PlayerSocketDataManager(List<PlayerDataTable> playerData, MmoActor player)
        {
            this.AttackHandler = new PlayerAttackHandler(player);
            this.PlayerBuildings = new Dictionary<StructureType, List<IPlayerBuildingManager>>();
            this.PlayerResources = new Dictionary<ResourceType, IPlayerResources>();
            this.playerData = playerData;
            this.player = player;
//            this.Builders = new List<UserRecordBuilderDetails>();
            this.DataContributeAccType(playerData);
        }

        public void DataContributeAccType(List<PlayerDataTable> playerData)
        {
            var sortedList = playerData.OrderBy(x => x.DataType);
            PlayerDataTable currData = null;
            try
            {
                foreach (var data in sortedList)
                {
                    currData = data;
                    switch (data.DataType)
                    {
                        case DataType.Structure: this.AddStructureOnPlayer(data); break;
                    }
                }

                foreach (var data in sortedList)
                {
                    currData = data;
                    switch (data.DataType)
                    {
                        case DataType.Resource: this.AddResourcesOnPlayer(data); break;
                        case DataType.Troop: this.AddTroopOnPlayerBuilding(data); break;
                        case DataType.Custom: 
                            if (data.ValueId == 1)//king
                            {
//                                King = JsonConvert.DeserializeObject<UserKingDetails>(data.Value);
            //                    GameService.BPlayerManager.GetAllPlayerData(Int32.Parse(operation.PlayerId)).Result.Data;
                            }
                            else if (data.ValueId == 2)//builder
                            {
        //                        var builder = JsonConvert.DeserializeObject<UserRecordBuilderDetails>(item.Value);
        //                        builder.Id = item.Id;
        //                        Builders.Add(builder);
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                log.InfoFormat("Error generating player data {0}: {1} ", currData.DataType, JsonConvert.SerializeObject(currData));
                log.Info(ex.Message);
                throw ex;
            }
//            if (King == null) King = new UserKingDetails();
        }

        public void AddTroopOnPlayerBuilding(PlayerDataTable data)
        {
            var troops = PlayerData.PlayerDataToUserTroopData(data);
            if (troops != null)
            {
#if DEBUG
//                log.InfoFormat("Player Data Convert to Troop {0} playerData {1} ",
//                  JsonConvert.SerializeObject(troops), JsonConvert.SerializeObject(data));
#endif
                foreach (var troop in troops.Value)
                {
                    if (troop.InTraning == null) continue;

                    foreach (var trainer in troop.InTraning)
                    {
                        var building = GetPlayerBuildingByLocationId(trainer.BuildingLocId);
                        if (building != null)
                        {
                            var bldManager = GameService.GameBuildingManagerInstances[building.StructureType];
                            var gameTroop = bldManager.Troops[troops.ValueId];
                            var troop1 = gameTroop.AddTroopOnPlayerBuilding(troops.ValueId, building, this.player);
                            troop1.Init(troops);
                        }
#if DEBUG
                        else
                            log.InfoFormat("User building not found {0} ", JsonConvert.SerializeObject(trainer));
#endif
                    }
                }

                //troop.
            }
#if DEBUG
            else
                log.InfoFormat("Troop not found when convert player data to troop info {0} ", JsonConvert.SerializeObject(data));
#endif
        }

        public void AddResourcesOnPlayer(PlayerDataTable data)
        {
            var resource = PlayerData.PlayerDataToUserResourceData(data);
            if (resource != null)
            {
#if DEBUG
//                log.InfoFormat("Player Data Convert to resource {0} playerData {1} ",
//                    JsonConvert.SerializeObject(resource), JsonConvert.SerializeObject(data));
#endif
                IReadOnlyResourceTable resInfo = null;
                switch (resource.ValueId)
                {
                    case ResourceType.Food: resInfo = CacheResourceDataManager.Food; break;
                    case ResourceType.Wood: resInfo = CacheResourceDataManager.Wood; break;
                    case ResourceType.Ore: resInfo = CacheResourceDataManager.Ore; break;
                    case ResourceType.Gems: resInfo = CacheResourceDataManager.Gems; break;
                }
                if (resInfo != null) 
                {
#if DEBUG
//                    log.InfoFormat("Add new resources on player account Resource {0} ", resource.ValueId.ToString());
#endif
                    IPlayerResources r = new PlayerResources(resInfo, resource.Value, this.player);
                    PlayerResources.Add(resource.ValueId, r);
                }
            }
#if DEBUG
            else
                log.InfoFormat("Resource not found when convert player data to resourse info {0} ", JsonConvert.SerializeObject(data));
#endif
        }

        public void AddStructureOnPlayer(PlayerDataTable data)
        {
            var structure = PlayerData.PlayerDataToUserStructureData(data);
            if (structure != null)
            {
#if DEBUG
//                log.InfoFormat("Player Data Convert to structure {0} playerData {1} ",
//                    JsonConvert.SerializeObject(structure), JsonConvert.SerializeObject(data));
#endif
                if (!this.PlayerBuildings.ContainsKey(structure.ValueId))
                {
                    var multipleBuildings = GameService.BPlayerStructureManager.GetMultipleBuildings(structure);
                    foreach (var build in multipleBuildings)
                    {
                        IGameBuildingManager gameBuilding = GameService.GameBuildingManagerInstances[structure.ValueId];
                        this.AddStructure(build.Key, build.Value, gameBuilding);
                    }
                }
#if DEBUG
                else
                    log.InfoFormat("that structure is already defined in player structureType {0} ", structure.ValueId.ToString());
#endif
            }
#if DEBUG
            else
                log.InfoFormat("Structure not found when convert player data to structure info {0} ", JsonConvert.SerializeObject(data));
#endif
        }

        public void AddStructure(int locationId, UserStructureData structure, IGameBuildingManager gameBuilding)
        {
            if (!this.PlayerBuildings.ContainsKey(structure.ValueId))
            {
                this.PlayerBuildings.Add(structure.ValueId, new List<IPlayerBuildingManager>());
            }
            var building = this.PlayerBuildings[structure.ValueId].Find(x => (x.Location == locationId));
            if (building != null)
            {
                building.SetStructureData(structure);
                return;
            }

            IPlayerBuildingManager plyBuilding = null;
            switch (structure.ValueId)
            {
                case StructureType.CityCounsel: plyBuilding = new CityCounsilBuilding(gameBuilding, this.player, structure); break;
                case StructureType.WatchTower: plyBuilding = new WatchTower(gameBuilding, this.player, structure); break;
                case StructureType.Blacksmith: plyBuilding = new BlackSmith(gameBuilding, this.player, structure); break;
                case StructureType.Barracks: plyBuilding = new BarracksBuilding(gameBuilding, this.player, structure); break;
                case StructureType.Academy: plyBuilding = new Academy(gameBuilding, this.player, structure); break;
                case StructureType.Embassy: plyBuilding = new Embassy(gameBuilding, this.player, structure); break;
                case StructureType.Farm: plyBuilding = new Farm(gameBuilding, this.player, structure); break;
                case StructureType.Gate: plyBuilding = new Gate(gameBuilding, this.player, structure); break;
                case StructureType.InfantryCamp: plyBuilding = new InfantryCamp(gameBuilding, this.player, structure); break;
                case StructureType.Infirmary: plyBuilding = new Infirmary(gameBuilding, this.player, structure); break;
                case StructureType.Market: plyBuilding = new Market(gameBuilding, this.player, structure); break;
                case StructureType.Mine: plyBuilding = new Mine(gameBuilding, this.player, structure); break;
                case StructureType.Sawmill: plyBuilding = new SawMill(gameBuilding, this.player, structure); break;
                case StructureType.ShootingRange: plyBuilding = new ShootingRange(gameBuilding, this.player, structure); break;
                case StructureType.Warehouse: plyBuilding = new WareHouse(gameBuilding, this.player, structure); break;
                case StructureType.Workshop: plyBuilding = new Workshop(gameBuilding, this.player, structure); break;
                case StructureType.Stable: plyBuilding = new Stable(gameBuilding, this.player, structure); break;
//                case StructureType.TrainingHeroes: plyBuilding = new TrainingHeroes(gameBuilding, this.player, structure); break;
            }
            if (plyBuilding != null)
            {
#if DEBUG
//                log.InfoFormat("Add New Structure On Player Account {0} ", structure.ValueId.ToString());
#endif
                this.PlayerBuildings[structure.ValueId].Add(plyBuilding);
            }
        }

        public (bool succ, string msg) CheckRequirementsAndUpdateValues(IReadOnlyList<IReadOnlyDataRequirement> requirements)
        {
            foreach (var item in requirements)
            {
                if (item.DataType == DataType.Structure)
                {
                    if (this.PlayerBuildings.ContainsKey((StructureType)item.ValueId))
                    {
                        bool isSucc = this.PlayerBuildings[(StructureType)item.ValueId].Any(d => d.HasAvailableRequirement(item));
                        if (!isSucc) return (false, "Requirement not found.");
                    }
                    else return (false, "Player need to create structure.");
                }
                else if (item.DataType == DataType.Resource)
                {
                    if (this.PlayerResources.ContainsKey((ResourceType)item.ValueId))
                    {
                        bool isSucc = this.PlayerResources[(ResourceType)item.ValueId].HasAvailableRequirement(item);
                        if (!isSucc) return (false, "Requirement not found.");
                    }
                    else
                        return (false, "Player have insufficient resources.");
                }
            }
            foreach (var item in requirements)
            {
                if (item.DataType != DataType.Resource) continue;

                this.PlayerResources[(ResourceType)item.ValueId].IncrementResourceValue(-item.Value);
            }

            return (true, "Success");
        }

        public IPlayerBuildingManager GetPlayerBuilding(StructureType structType, int locationId)
        {
            IPlayerBuildingManager resp = null;

            if (this.PlayerBuildings.ContainsKey(structType))
            {
                //TODO: check this logic, dif pulling playerbuildings/internalplayerdata
                resp = player.InternalPlayerDataManager.PlayerBuildings[structType].FirstOrDefault(d => (d.Location == locationId));
            }

            return resp;
        }

        public IPlayerBuildingManager GetPlayerBuilding(int structType, int locationId)
        {
            return this.GetPlayerBuilding((StructureType)structType, locationId);
        }

        public IPlayerBuildingManager GetPlayerBuildingByLocationId(int locationId)
        {
            IPlayerBuildingManager resp = null;

            foreach (var buildings in this.PlayerBuildings)
            {
                var building = buildings.Value.FirstOrDefault(x => (x.Location == locationId));
                if (building == null) continue;

                resp = building;
                break;
            }

            return resp;
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

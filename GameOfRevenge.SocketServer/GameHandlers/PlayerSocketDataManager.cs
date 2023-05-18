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

            var sortedList = playerData.OrderBy(x => x.DataType);
/*            log.Info("user data =");
            foreach (var data in sortedList)
            {
                log.Info(data.DataType.ToString()+"  "+data.ValueId+"  = "+data.Value);
            }
            log.Info("---------");*/
            PlayerDataTable currData = null;
            try
            {
                foreach (var data in sortedList)
                {
                    currData = data;
                    switch (data.DataType)
                    {
                        case DataType.Structure:
                            var structure = PlayerData.PlayerDataToUserStructureData(data);
#if DEBUG
                            log.InfoFormat("Player Data Convert to structure {0} playerData {1} ",
                            JsonConvert.SerializeObject(structure), JsonConvert.SerializeObject(data));
#endif
                            AddStructureOnPlayer(structure);
                            break;
                    }
                }

                foreach (var data in sortedList)
                {
                    currData = data;
                    switch (data.DataType)
                    {
                        case DataType.Resource:
                            var resource = PlayerData.PlayerDataToUserResourceData(data);
#if DEBUG
                            log.InfoFormat("Player Data Convert to resource {0} playerData {1} ",
                            JsonConvert.SerializeObject(resource), JsonConvert.SerializeObject(data));
#endif
                            AddResourcesOnPlayer(resource);
                            break;
                        case DataType.Troop:
                            var troops = PlayerData.PlayerDataToUserTroopData(data);
#if DEBUG
                            log.InfoFormat("Player Data Convert to Troop {0} playerData {1} ",
                              JsonConvert.SerializeObject(troops), JsonConvert.SerializeObject(data));
#endif
                            AddTroopOnPlayerBuilding(troops);
                            break;
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
                log.InfoFormat("Error generating player data {0}: {1}", currData.DataType, JsonConvert.SerializeObject(currData));
                log.Info(ex.GetType().ToString()+"  "+ex.Message);
                throw ex;
            }
//            if (King == null) King = new UserKingDetails();
        }

        public void UpdateData(PlayerCompleteData data)
        {
            log.Info($"UPDATING PLAYER {player.PlayerId} DATA START");
            foreach (var structure in data.Structures)
            {
                AddStructureOnPlayer(structure.Id, structure.StructureType, structure.Buildings);
            }
            foreach (var structure in data.Troops)
            {
//                AddTroopOnPlayerBuilding()
//                AddStructureOnPlayer(structure.Id, structure.StructureType, structure.Buildings);
            }
            log.Info($"UPDATING PLAYER {player.PlayerId} DATA END");
        }

        public void AddTroopOnPlayerBuilding(UserTroopData troops)
        {
            log.Info("AddTroopOnPlayerBuilding: "+JsonConvert.SerializeObject(troops));
            if (troops == null)
            {
#if DEBUG
                log.InfoFormat("Troop not found");
#endif
                return;
            }

            foreach (var troop in troops.Value)
            {
                if (troop.InTraning == null) continue;

                foreach (var trainer in troop.InTraning)
                {
                    var building = GetPlayerBuildingByLocationId(trainer.BuildingLocId);
                    if (building == null)
                    {
#if DEBUG
                        log.InfoFormat("User building not found {0} ", JsonConvert.SerializeObject(trainer));
#endif
                    }

                    if (!GameService.GameBuildingManagerInstances.ContainsKey(building.StructureType))
                    {
                        log.Info("STRUCTURE NOT SUPPORTED> "+building.StructureType.ToString()+" (loc="+trainer.BuildingLocId+")");
                        continue;
                    }

                    var bldManager = GameService.GameBuildingManagerInstances[building.StructureType];
                    var gameTroop = bldManager.Troops[troops.ValueId];
                    var trp = gameTroop.AddTroopOnPlayerBuilding(troops.ValueId, building, player);
                    trp.Init(troops);
                }
            }
        }

        public void AddResourcesOnPlayer(UserResourceData resource)
        {
            if (resource == null)
            {
#if DEBUG
                log.InfoFormat("Resource not found");
#endif
                return;
            }

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
                log.InfoFormat("Add new resources on player account Resource {0} ", resource.ValueId.ToString());
#endif
                IPlayerResources r = new PlayerResources(resInfo, resource.Value, this.player);
                PlayerResources.Add(resource.ValueId, r);
            }
        }

        public void AddStructureOnPlayer(UserStructureData data)
        {
            AddStructureOnPlayer(data.Id, data.ValueId, data.Value);
        }

        public void AddStructureOnPlayer(long id, StructureType structureType, List<StructureDetails> structureDetails)
        {
            if (structureDetails == null)
            {
#if DEBUG
                log.InfoFormat("Structure not found");
#endif
                return;
            }

            if (PlayerBuildings.ContainsKey(structureType))
            {
#if DEBUG
                log.InfoFormat("that structure is already defined in player structureType {0} ", structureType.ToString());
#endif
            }

            try
            {
//                var multipleBuildings = GameService.BPlayerStructureManager.GetMultipleBuildings(structure);
//                List<int> locId = structureDetails.GroupBy(d => d.Location).Select(d => d.Key).ToList();

//                log.Info("multiBuildings ok = "+(multipleBuildings != null));
                foreach (var structure in structureDetails)
                {
                    if (!GameService.GameBuildingManagerInstances.ContainsKey(structureType))
                    {
                        log.Info("STRUCTURE NOT SUPPORTED: " + structureType.ToString());
                        continue;
                    }

                    var gameBuilding = GameService.GameBuildingManagerInstances[structureType];
/*                    foreach (var dicval in GameService.GameBuildingManagerInstances)
                    {
                        log.Info("  "+dicval.Key.ToString()+"  val="+(dicval.Value != null));
                    }
                    log.Info(".......... "+(gameBuilding != null));

                    var structureData = new UserStructureData()
                    {
                        Id = id,
                        DataType = DataType.Structure,
                        ValueId = structureType,
                        Value = structureDetails

                    };*/
                    AddStructure(structure.Location, structureType, structure, gameBuilding);
                }
            }
            catch (Exception ex)
            {
                log.Info("exception2 " + ex.Message);
                throw ex;
            }
        }

        public void AddStructure(int locationId, StructureType structureType, StructureDetails structure, IGameBuildingManager gameBuilding)
        {
            if (!PlayerBuildings.ContainsKey(structureType))
            {
                log.Info("new list of structures "+structureType.ToString());
                PlayerBuildings.Add(structureType, new List<IPlayerBuildingManager>());
            }
            var building = PlayerBuildings[structureType].Find(x => (x.Location == locationId));
            if (building != null)
            {
                log.Info("set structure data for location "+locationId);
                building.SetStructureData(structure);
                return;
            }

            IPlayerBuildingManager plyBuilding = null;
            switch (structureType)
            {
                case StructureType.CityCounsel: plyBuilding = new CityCounsilBuilding(player, structure, gameBuilding); break;
                case StructureType.WatchTower: plyBuilding = new WatchTower(player, structure, gameBuilding); break;
                case StructureType.Blacksmith: plyBuilding = new BlackSmith(player, structure, gameBuilding); break;
                case StructureType.Barracks: plyBuilding = new BarracksBuilding(player, structure, gameBuilding); break;
                case StructureType.Academy: plyBuilding = new Academy(player, structure, gameBuilding); break;
                case StructureType.Embassy: plyBuilding = new Embassy(player, structure, gameBuilding); break;
                case StructureType.Farm: plyBuilding = new Farm(player, structure, gameBuilding); break;
                case StructureType.Gate: plyBuilding = new Gate(player, structure, gameBuilding); break;
                case StructureType.InfantryCamp: plyBuilding = new InfantryCamp(player, structure, gameBuilding); break;
                case StructureType.Infirmary: plyBuilding = new Infirmary(player, structure, gameBuilding); break;
                case StructureType.Market: plyBuilding = new Market(player, structure, gameBuilding); break;
                case StructureType.Mine: plyBuilding = new Mine(player, structure, gameBuilding); break;
                case StructureType.Sawmill: plyBuilding = new SawMill(player, structure, gameBuilding); break;
                case StructureType.ShootingRange: plyBuilding = new ShootingRange(player, structure, gameBuilding); break;
                case StructureType.Warehouse: plyBuilding = new WareHouse(player, structure, gameBuilding); break;
                case StructureType.Workshop: plyBuilding = new Workshop(player, structure, gameBuilding); break;
                case StructureType.Stable: plyBuilding = new Stable(player, structure, gameBuilding); break;
//TRAINING HEROES not requiered, level 1 is max build level
//                case StructureType.TrainingHeroes: plyBuilding = new TrainingHeroes(gameBuilding, this.player, structure); break;
            }
            if (plyBuilding != null)
            {
#if DEBUG
                log.Info($"Added New Structure {structureType} at Location {locationId}");
#endif
                PlayerBuildings[structureType].Add(plyBuilding);
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

        public IPlayerBuildingManager GetPlayerBuilding(StructureType structureType, int location)
        {
            IPlayerBuildingManager resp = null;

            if (this.PlayerBuildings.ContainsKey(structureType))
            {
                //TODO: check this logic, dif pulling playerbuildings/internalplayerdata
                resp = player.InternalPlayerDataManager.PlayerBuildings[structureType].FirstOrDefault(d => (d.Location == location));
            }

            return resp;
        }

        public IPlayerBuildingManager GetPlayerBuilding(int structureType, int location)
        {
            return this.GetPlayerBuilding((StructureType)structureType, location);
        }

        public IPlayerBuildingManager GetPlayerBuildingByLocationId(int location)
        {
            IPlayerBuildingManager resp = null;

            foreach (var buildings in this.PlayerBuildings)
            {
                var building = buildings.Value.Find(x => (x.Location == location));
                if (building == null) continue;

//                log.Info("found bld at location "+location+"  bld="+JsonConvert.SerializeObject(building));
//                log.Info("raw data = " + JsonConvert.SerializeObject(buildings.Value));
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

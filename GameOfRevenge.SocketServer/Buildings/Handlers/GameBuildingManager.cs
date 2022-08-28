using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ExitGames.Logging;
using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.GameHandlers;
using GameOfRevenge.Model;
using GameOfRevenge.Troops;
using GameOfRevenge.GameApplication;
using GameOfRevenge.Common.Interface.Model;

namespace GameOfRevenge.Buildings.Handlers
{
    public class GameBuildingManager : IGameBuildingManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        public IReadOnlyStructureDataRequirementRel BuildingData { get; set; }
        public Dictionary<TroopType, IGameTroop> Troops { get; private set; }
        public StructureType StructureType { get { return BuildingData.Info.Code; } }

        public GameBuildingManager(IReadOnlyStructureDataRequirementRel data, Dictionary<TroopType, IGameTroop> troops)
        {
            BuildingData = data;
            Troops = troops;
        }

        public void RecruitTroops(RecruitTroopRequest request, MmoActor actor)
        {
            if (Troops.ContainsKey((TroopType)request.TroopType)) Troops[(TroopType)request.TroopType].TroopTraining(request, actor);
            else actor.Peer.SendOperation(OperationCode.RecruitTroopRequest, ReturnCode.Failed, null, "Troops not found in game building.");
        }

        public void CreateStructureForPlayer(CreateStructureRequest request, MmoActor actor)
        {
            log.InfoFormat("Create Structure Request CurrentBuilding {0} Data {1} ", this.BuildingData.Info.Code.ToString(), JsonConvert.SerializeObject(request));
            var structureData = BuildingData.GetStructureLevelById(1);
            if (structureData == null) actor.Peer.SendOperation(OperationCode.CreateStructure, ReturnCode.Failed, null, "Structure not found.");
            else
            {
                bool isAlreadyLocated = actor.PlayerDataManager.PlayerBuildings.Values.Any(d => d.Any(f => f.Location == request.StructureLocationId));
                if (isAlreadyLocated) actor.Peer.SendOperation(OperationCode.CreateStructure, ReturnCode.Failed, null, "LocationId is invalid, structure is already located in this locationId.");
                else
                {
                    log.InfoFormat("Get Structure Data for level 1 CurrentBuilding {0} ", JsonConvert.SerializeObject(structureData));
                    var response = CreateAndUpgradeStructure(actor, structureData.Requirements, request.StructureLocationId);
                    if (!response.IsSuccess) actor.Peer.SendOperation(OperationCode.CreateStructure, ReturnCode.Failed, null, response.Message);
                    else
                    {
                        var obj = new StructureCreateUpgradeResponse()
                        {
                            StructureLocationId = request.StructureLocationId,
                            StructureType = request.StructureType,
                            StructureLevel = 1
                        };

                        actor.Peer.SendOperation(OperationCode.CreateStructure, ReturnCode.OK, obj.GetDictionary());
                    }
                }
            }
        }

        public bool UpgradeStructureForPlayer(UpgradeStructureRequest request, MmoActor actor)
        {
            bool success = true;
            log.InfoFormat("Upgrade Structure Request CurrentBuilding {0} Data {1} ", this.BuildingData.Info.Code.ToString(),
                JsonConvert.SerializeObject(request));
            if (actor.PlayerDataManager.PlayerBuildings.ContainsKey((this.StructureType)))
            {
                var building = actor.PlayerDataManager.PlayerBuildings[this.StructureType].Where(d => d.Location == request.StructureLocationId).FirstOrDefault();
                if (building != null)
                {
                    if (!building.IsConstructing)
                    {
                        int upgradeLevel = building.CurrentLevel + 1;
                        log.InfoFormat("Upgrade Structure Request Upgrade Level {0} ", upgradeLevel);
                        var structureData = BuildingData.GetStructureLevelById(upgradeLevel);
                        if (structureData != null)
                        {
                            var response = CreateAndUpgradeStructure(actor, structureData.Requirements, request.StructureLocationId, true);
                            if (response.IsSuccess)
                            {
                                var obj = new StructureCreateUpgradeResponse
                                {
                                    StructureLocationId = request.StructureLocationId,
                                    StructureType = request.StructureType,
                                    StructureLevel = upgradeLevel
                                };

                                actor.Peer.SendOperation(OperationCode.UpgradeStructure, ReturnCode.OK, obj.GetDictionary());
                            }
                            else
                            {
                                actor.Peer.SendOperation(OperationCode.UpgradeStructure, ReturnCode.Failed, debuMsg: response.Message);
                                success = false;
                            }
                        }
                        else
                        {
                            actor.Peer.SendOperation(OperationCode.UpgradeStructure, ReturnCode.Failed, debuMsg: "Structure not found in data.");
                            success = false;
                        }
                    }
                    else
                    {
                        actor.Peer.SendOperation(OperationCode.UpgradeStructure, ReturnCode.Failed, debuMsg: "Building is already in constructing mode for current level. please try after some time. ");
                        success = false;
                    }
                }
                else
                {
                    actor.Peer.SendOperation(OperationCode.UpgradeStructure, ReturnCode.Failed, debuMsg: "Structure not found.");
                    success = false;
                }
            }
            else
            {
                actor.Peer.SendOperation(OperationCode.UpgradeStructure, ReturnCode.Failed, debuMsg: "Structure not found.");
                success = false;
            }
            return success;
        }
        
        private Response<UserStructureData> CreateAndUpgradeStructure(MmoActor player, IReadOnlyList<IReadOnlyDataRequirement> requirments, int locationId, bool isUpgrade = false)
        {
            if (requirments.Count > 0)
            {
                var (rsucc, rmsg) = player.PlayerDataManager.CheckRequirmentsAndUpdateValues(requirments);
                if (!rsucc) new Response<UserStructureData>(rsucc ? 100 : 200, rmsg);
            }
            
            Response<UserStructureData> response;
            if (!isUpgrade)
                response = GameService.BPlayerStructureManager.CreateBuilding(Convert.ToInt32(player.UserId), this.StructureType, locationId).Result;
            else
                response = GameService.BPlayerStructureManager.UpgradeBuilding(Convert.ToInt32(player.UserId), this.StructureType, locationId).Result;

            if (response.Case < 100) return new Response<UserStructureData>(200, response.Message);
            
            log.InfoFormat("Response Of Create/Upgrasde Structure Manager DATA {0} ", JsonConvert.SerializeObject(response.Data));
            UserStructureData structure = GameService.BPlayerStructureManager.GetStructureDataAccLoc(response.Data, locationId);
            if (!isUpgrade)
                player.PlayerDataManager.AddStructure(locationId, structure, this);
            else
            {
                var building = player.PlayerDataManager.GetPlayerBuilding(structure.ValueId, locationId);
                if (building != null)
                    building.UpgradeBuilding(structure);
            }

            return response;
        }

/*        private Response<UserStructureData> HelpBuildStructure(MmoActor player, int locationId, int helpValue = 1)
        {
/*            if (requirments.Count > 0)
            {
                var (rsucc, rmsg) = player.PlayerDataManager.CheckRequirmentsAndUpdateValues(requirments);
                if (!rsucc) new Response<UserStructureData>(rsucc ? 100 : 200, rmsg);
            }
* /
            Response<UserStructureData> response;
//            response = GameService.BPlayerStructureManager.UpgradeBuilding(Convert.ToInt32(player.UserId), this.StructureType, locationId).Result;

            log.InfoFormat("Response Of Create/Upgrasde Structure Manager DATA {0} ", JsonConvert.SerializeObject(response.Data));


            var building = player.PlayerDataManager.GetPlayerBuildingByLocationId(locationId);
            if ((building != null) && building.IsConstructing)
            {
                building.HelpBuild(helpValue);
            }
            var obj = new StructureCreateUpgradeResponse
            {
                StructureLocationId = request.StructureLocationId,
                StructureType = request.StructureType,
                StructureLevel = upgradeLevel
            };

            return response;
        }*/

        int IGameBuildingManager.BuildTime(int level)
        {
            int buildTime = 0;
            var structure = this.BuildingData.GetStructureLevelById(level);
            if (structure != null) buildTime = structure.Data.TimeToBuild;
            return buildTime;
        }
    }
}

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

        public IReadOnlyStructureDataRequirementRel CacheBuildingData { get; set; }

        public Dictionary<TroopType, IGameTroop> Troops { get; private set; }

        public StructureType StructureType => CacheBuildingData.Info.Code;


        public GameBuildingManager(IReadOnlyStructureDataRequirementRel data, Dictionary<TroopType, IGameTroop> troops)
        {
            CacheBuildingData = data;
            Troops = troops;
        }

        public void RecruitTroops(RecruitTroopRequest request, MmoActor actor)
        {
            if (Troops.ContainsKey((TroopType)request.TroopType)) Troops[(TroopType)request.TroopType].TroopTraining(request, actor);
            else actor.Peer.SendOperation(OperationCode.RecruitTroopRequest, ReturnCode.Failed, null, "Troops not found in game building.");
        }

        public void CreateStructureForPlayer(CreateStructureRequest request, MmoActor actor)
        {
            log.InfoFormat("Create Structure Request CurrentBuilding {0} Data {1} ", this.CacheBuildingData.Info.Code.ToString(), JsonConvert.SerializeObject(request));
            var structureData = CacheBuildingData.GetStructureLevelById(1);
            if (structureData == null)
            {
                var str = "Structure not found.";
                actor.Peer.SendOperation(OperationCode.CreateStructure, ReturnCode.Failed, null, str);
                return;
            }

            bool isAlreadyLocated = actor.InternalPlayerDataManager.PlayerBuildings.Values.Any(d => d.Any(f => f.Location == request.StructureLocationId));
            if (isAlreadyLocated)
            {
                var str = "LocationId is invalid, structure is already located in this location.";
                actor.Peer.SendOperation(OperationCode.CreateStructure, ReturnCode.Failed, null, str);
            }
            else
            {
                log.InfoFormat("Get Structure Data for level 1 CurrentBuilding {0} ", JsonConvert.SerializeObject(structureData));
                var response = CreateOrUpgradeStructure(actor, structureData.Requirements, request.StructureLocationId);
                if (!response.IsSuccess)
                {
                    actor.Peer.SendOperation(OperationCode.CreateStructure, ReturnCode.Failed, null, response.Message);
                }
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

        public bool UpgradeStructureForPlayer(UpgradeStructureRequest request, MmoActor actor)
        {
            log.InfoFormat("Upgrade Structure Request CurrentBuilding {0} Data {1} ", this.CacheBuildingData.Info.Code.ToString(),
                JsonConvert.SerializeObject(request));

            bool success = true;
            if (actor.InternalPlayerDataManager.PlayerBuildings.ContainsKey((this.StructureType)))
            {
                var building = actor.InternalPlayerDataManager.PlayerBuildings[this.StructureType].Where(d => d.Location == request.StructureLocationId).FirstOrDefault();
                if (building != null)
                {
                    if (!building.IsConstructing)
                    {
                        int upgradeLevel = building.CurrentLevel + 1;
                        log.InfoFormat("Upgrade Structure Request Upgrade Level {0} ", upgradeLevel);
                        var structureData = CacheBuildingData.GetStructureLevelById(upgradeLevel);
                        if (structureData != null)
                        {
                            var response = CreateOrUpgradeStructure(actor, structureData.Requirements, request.StructureLocationId, true);
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
                            actor.Peer.SendOperation(OperationCode.UpgradeStructure, ReturnCode.Failed, debuMsg: "Structure data not found.");
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
        
        private Response<UserStructureData> CreateOrUpgradeStructure(MmoActor player, IReadOnlyList<IReadOnlyDataRequirement> requirments, int locationId, bool isUpgrade = false)
        {
            if (requirments.Count > 0)
            {
                var (rsucc, rmsg) = player.InternalPlayerDataManager.CheckRequirementsAndUpdateValues(requirments);
                if (!rsucc) new Response<UserStructureData>(rsucc ? 100 : 200, rmsg);
            }
            
            Response<UserStructureData> response;
            if (isUpgrade)
            {
                response = GameService.BPlayerStructureManager.UpgradeBuilding(player.PlayerId, this.StructureType, locationId).Result;
            }
            else
            {
                response = GameService.BPlayerStructureManager.CreateBuilding(player.PlayerId, this.StructureType, locationId).Result;
            }
            if ((response.Case < 100) || (response.Data == null)) return new Response<UserStructureData>(200, response.Message);
            
            log.InfoFormat("Response Of Create/Upgrade Structure Manager DATA {0} ", JsonConvert.SerializeObject(response.Data));
            var structure = new UserStructureData()
            {
                Id = response.Data.Id,
                DataType = response.Data.DataType,
//                StructureId = response.Data.StructureId,
                ValueId = response.Data.ValueId,
                Value = response.Data.Value.Where(d => d.Location == locationId).ToList()
            };

            if (isUpgrade)
            {
                var building = player.InternalPlayerDataManager.GetPlayerBuilding(structure.ValueId, locationId);
                if (building != null) building.AddBuildingUpgrading(structure);
            }
            else
            {
                player.InternalPlayerDataManager.AddStructure(locationId, structure, this);
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
            var structure = this.CacheBuildingData.GetStructureLevelById(level);
            if (structure != null) buildTime = structure.Data.TimeToBuild;
            return buildTime;
        }
    }
}

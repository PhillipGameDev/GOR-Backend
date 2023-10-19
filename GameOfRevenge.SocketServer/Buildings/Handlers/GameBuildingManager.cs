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
using GameOfRevenge.Business;

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

        public void RecruitTroops(RecruitTroopRequest request, PlayerInstance actor)
        {
            if (Troops.ContainsKey((TroopType)request.TroopType))
            {
                Troops[(TroopType)request.TroopType].TroopTraining(request, actor);
            }
            else
            {
                var msg = "Troops not found in game building.";
                actor.Peer.SendOperation(OperationCode.RecruitTroopRequest, ReturnCode.Failed, null, msg);
            }
        }

        public void CreateStructureForPlayer(CreateStructureRequest request, PlayerInstance actor)
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

        public bool UpgradeStructureForPlayer(UpgradeStructureRequest request, PlayerInstance actor)
        {
#if DEBUG
            log.InfoFormat("Upgrade Structure Request CurrentBuilding {0} Data {1} ", this.CacheBuildingData.Info.Code.ToString(),
                JsonConvert.SerializeObject(request));
#endif
            bool success = false;
            var locationId = request.StructureLocationId;
            var building = actor.InternalPlayerDataManager.GetPlayerBuilding(StructureType, locationId);
            if (building != null)
            {
                if (!building.IsConstructing)
                {
                    int upgradeLevel = building.CurrentLevel + 1;
#if DEBUG
                    log.InfoFormat("Upgrade Structure Request Level {0} ", upgradeLevel);
#endif
                    var structureData = CacheBuildingData.GetStructureLevelById(upgradeLevel);
                    if (structureData != null)
                    {
                        var response = CreateOrUpgradeStructure(actor, structureData.Requirements, locationId, true);
                        if (response.IsSuccess)
                        {
                            var obj = new StructureCreateUpgradeResponse
                            {
                                StructureLocationId = locationId,
                                StructureType = request.StructureType,
                                StructureLevel = upgradeLevel
                            };

                            actor.Peer.SendOperation(OperationCode.UpgradeStructure, ReturnCode.OK, obj.GetDictionary());
                            success = true;
                        }
                        else
                        {
                            actor.Peer.SendOperation(OperationCode.UpgradeStructure, ReturnCode.Failed, debuMsg: response.Message);
                        }
                    }
                    else
                    {
                        var dbgMsg = "Structure data not found.";
                        actor.Peer.SendOperation(OperationCode.UpgradeStructure, ReturnCode.Failed, debuMsg: dbgMsg);
                    }
                }
                else
                {
                    var dbgMsg = "Building is already in constructing mode for current level. please try after some time.";
                    actor.Peer.SendOperation(OperationCode.UpgradeStructure, ReturnCode.Failed, debuMsg: dbgMsg);
                }
            }
            else
            {
                var dbgMsg = "Structure not found.";
                actor.Peer.SendOperation(OperationCode.UpgradeStructure, ReturnCode.Failed, debuMsg: dbgMsg);
            }

            return success;
        }
        
        private Response<UserStructureData> CreateOrUpgradeStructure(PlayerInstance player, IReadOnlyList<IReadOnlyDataRequirement> requirments, int location, bool isUpgrade = false)
        {
            if (requirments.Count > 0)
            {
                var (rsucc, rmsg) = player.InternalPlayerDataManager.CheckRequirementsAndUpdateValues(requirments);
                if (!rsucc) new Response<UserStructureData>(rsucc ? 100 : 200, rmsg);
            }
            
            Response<BuildingStructureData> response;
            if (isUpgrade)
            {
                response = GameService.BPlayerStructureManager.UpgradeBuilding(player.PlayerId, this.StructureType, location).Result;
            }
            else
            {
                response = GameService.BPlayerStructureManager.CreateBuilding(player.PlayerId, this.StructureType, location).Result;
            }
            if ((response.Case < 100) || (response.Data == null)) return new Response<UserStructureData>(200, response.Message);

#if DEBUG
            log.InfoFormat("Response Of Create/Upgrade Structure Manager DATA {0} ", JsonConvert.SerializeObject(response.Data));
#endif
            var workerBuildingData = response.Data;
            var structureType = workerBuildingData.StructureData.ValueId;
            var bld = workerBuildingData.StructureData.Value.Find(x => x.Location == location);
/*            var structure = new UserStructureData()
            {
                Id = workerBuildingData.StructureData.Id,
                DataType = workerBuildingData.StructureData.DataType,
                ValueId = workerBuildingData.StructureData.ValueId,
                Value = workerBuildingData.StructureData.Value.Where(d => d.Location == location).ToList()
            };*/

            if (isUpgrade)
            {
                var building = player.InternalPlayerDataManager.GetPlayerBuilding(structureType, location);
                if (building != null) building.AddBuildingUpgrading(bld);
            }
            else
            {
                player.InternalPlayerDataManager.AddStructure(location, structureType, bld, this);
            }

            return new Response<UserStructureData>()
            {
                Case = response.Case,
                Data = workerBuildingData.StructureData,
                Message = response.Message
            };
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

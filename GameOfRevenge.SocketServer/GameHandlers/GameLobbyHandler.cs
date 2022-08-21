using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Photon.SocketServer;
using ExitGames.Logging;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Net;
using GameOfRevenge.GameApplication;
using GameOfRevenge.Helpers;
using GameOfRevenge.Interface;
using GameOfRevenge.Model;

namespace GameOfRevenge.GameHandlers
{
    public class GameLobbyHandler
    {
        public static readonly ILogger log = LogManager.GetCurrentClassLogger();
        public IWorld GridWorld { get { return GameService.WorldHandler.DefaultWorld; } }

        public async Task<SendResult> OnLobbyMessageRecived(IGorMmoPeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            try
            {
                log.InfoFormat("Message Recived OpCode {0} Data {1} ", operationRequest.OperationCode, GlobalHelper.DicToString(operationRequest.Parameters));

                switch ((OperationCode)operationRequest.OperationCode)
                {
                    case OperationCode.PlayerConnectToServer: return HandlPlayerConnectToGameServer(peer, operationRequest);//1
                    case OperationCode.UserTeleport: return HandlePlayerTeleport(peer, operationRequest);//3
                    case OperationCode.JoinKingdomRoom: return HandlePlayerJoinKingdomView(peer, operationRequest);//4
                    case OperationCode.LeaveKingdomRoom: return HandlePlayerLeaveKingdomView(peer, operationRequest);//5
                    case OperationCode.PlayerCameraMove: return HandlePlayerCameraMove(peer, operationRequest);//6
                    case OperationCode.CreateStructure: return await HandleCreateStructure(peer, operationRequest);//7
                    case OperationCode.UpgradeStructure: return await HandleUpgradeStructure(peer, operationRequest);//8
                    case OperationCode.PlayerBuildingStatus: return await HandlePlayerBuildingStatus(peer, operationRequest);//9
                    case OperationCode.RecruitTroopRequest: return await HandleRecruitToopRequest(peer, operationRequest);//10
                    case OperationCode.RecruitTroopStatus: return await HandleRecruitTroopStatus(peer, operationRequest);//11
                    case OperationCode.TroopTrainerTimeBoost: return HandleTroopBoostUp(peer, operationRequest);//12
                    case OperationCode.CollectResourceRequest: return await HandleCollectResourceRequest(peer, operationRequest);//13
                    case OperationCode.BoostResourceTime: return HandleBoostResources(peer, operationRequest);//14
                    case OperationCode.AttackRequest: return HandleAttackRequest(peer, operationRequest);//15
                    case OperationCode.WoundedHealReqeust: return HandleWoundedHealRequest(peer, operationRequest);//16
                    case OperationCode.WoundedHealTimerRequest: return HandleWoundedHealTimerStatus(peer, operationRequest);//17
                    case OperationCode.UpgradeTechnology: return UpgradeTechnologyRequest(peer, operationRequest);//18
                    case OperationCode.RepairGate: return RepairGateRequest(peer, operationRequest);//19
                    case OperationCode.GateHp: return GetGateHpRequest(peer, operationRequest);//20
                    case OperationCode.GlobalChat: return GlobalChat(peer, operationRequest);//21
                    case OperationCode.AllianceChat: return AllianceChat(peer, operationRequest);//21
                    case OperationCode.Ping: return peer.SendOperation(operationRequest.OperationCode, ReturnCode.OK);//2
                    case OperationCode.GetInstantBuildCost: return GetInstantBuildCost(peer, operationRequest);//26
                    case OperationCode.InstantBuild: return await HandleInstantBuild(peer, operationRequest);//23
                    case OperationCode.SpeedUpBuildCost: return await SpeedUpBuildCost(peer, operationRequest);//25
                    case OperationCode.SpeedUpBuild: return await HandleSpeedUpBuild(peer, operationRequest);//24
                    case OperationCode.InstantRecruit: return await HandleInstantRecruit(peer, operationRequest);//27
                    case OperationCode.HelpStructure: return HandleHelpStructure(peer, operationRequest);
//                    OperationCode.CheckUnderAttack = 22
                    default: return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation);
                }
            }
            catch (Exception ex)
            {
                var opCode = (OperationCode)operationRequest.OperationCode;
                log.Error($"************************* Error in {opCode} request **************************************", ex);
                return SendResult.Failed;
            }
        }

        private async Task<SendResult> HandleInstantRecruit(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            var operation = new RecruitTroopRequest(peer.Protocol, operationRequest);
            var recruitResp = await GameService.InstantProgressManager.InstantRecruitTroops(peer.Actor.PlayerId, operation.LocationId, (TroopType)operation.TroopType, operation.TroopLevel, operation.TroopCount);

            if (recruitResp == null)
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: "player not found");
            if (!recruitResp.IsSuccess)
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: recruitResp.Message);
            if (!recruitResp.HasData)
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: "player data not found");

            await GameService.NewRealTimeUpdateManager.UpdatePlayerData(peer.Actor.PlayerId);

            return SendOperationResponse(peer, operationRequest, recruitResp);
        }

        private SendResult GetInstantBuildCost(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            var operation = new InstantBuildRequest(peer.Protocol, operationRequest);

            var costResponse = GameService.InstantProgressManager.GetInstantBuildCost(peer.Actor.PlayerId, (StructureType)operation.StructureType, operation.StructureLevel);
            if (costResponse == null || !costResponse.IsSuccess)
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: costResponse.Message);

            var response = new GemsCostResponse() { GemsCost = costResponse.Data };
            return peer.SendOperation(operationRequest.OperationCode, ReturnCode.OK, response.GetDictionary());
        }

        private async Task<SendResult> SpeedUpBuildCost(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            var operation = new InstantBuildRequest(peer.Protocol, operationRequest);

            var costResponse = await GameService.InstantProgressManager.GetBuildingSpeedUpCost(peer.Actor.PlayerId, operation.StructureLocationId);
            if (costResponse == null || !costResponse.IsSuccess) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: costResponse.Message);

            var response = new GemsCostResponse() { GemsCost = costResponse.Data };
            return peer.SendOperation(operationRequest.OperationCode, ReturnCode.OK, response.GetDictionary());
        }

        private async Task<SendResult> HandleInstantBuild(IGorMmoPeer peer, OperationRequest operationRequest)
        {

            log.Info("**************** HandleInstantBuild Start************************");
            var operation = new InstantBuildRequest(peer.Protocol, operationRequest);
            log.Info($"StructureType: {(StructureType)operation.StructureType}");
            log.Info($"StructureLocationId: {operation.StructureLocationId}");
            log.Info($"StructureLevel: {operation.StructureLevel}");

            var response = await GameService.InstantProgressManager.InstantBuildStructure(peer.Actor.PlayerId, (StructureType)operation.StructureType, operation.StructureLevel, operation.StructureLocationId);

            if (response == null || !response.IsSuccess) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: response.Message);

            await GameService.NewRealTimeUpdateManager.UpdatePlayerData(peer.Actor.PlayerId);

            var structure = response.Data.Value.FirstOrDefault(x => x.Location == operation.StructureLocationId);
            if (structure == null) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: "Structure was null");

            log.Info("**************** HandleInstantBuild End************************");

            await GameService.NewRealTimeUpdateManager.UpdatePlayerData(peer.Actor.PlayerId);

            var obj = new StructureCreateUpgradeResponse()
            {
                StructureLevel = structure.Level,
                StructureLocationId = structure.Location,
                StructureType = operation.StructureType,
            };
            return peer.SendOperation(operationRequest.OperationCode, ReturnCode.OK, obj.GetDictionary());
        }

        private async Task<SendResult> HandleSpeedUpBuild(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            var operation = new InstantBuildRequest(peer.Protocol, operationRequest);

            var response = await GameService.InstantProgressManager.SpeedUpBuildStructure(peer.Actor.PlayerId, operation.StructureLocationId);
            if (response == null || !response.IsSuccess) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: response.Message);

            await GameService.NewRealTimeUpdateManager.UpdatePlayerData(peer.Actor.PlayerId);

            var structure = response.Data.Value.FirstOrDefault(x => x.Location == operation.StructureLocationId);
            if (structure == null) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: "Structure was null");

            var obj = new StructureCreateUpgradeResponse()
            {
                StructureLevel = structure.Level,
                StructureLocationId = structure.Location,
                StructureType = (int)operation.StructureType,
            };

            var result = peer.SendOperation(operationRequest.OperationCode, ReturnCode.OK, obj.GetDictionary());
            SendBuildingCompleteToBuild(peer, (StructureType)operation.StructureType, operation.StructureLevel, operation.StructureLocationId);
            return result;
        }

        private SendResult GlobalChat(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            GameLobbyHandler.log.Info(">>>>>GLOBAL CHAT");
            var operation = new ChatMessageRequest(peer.Protocol, operationRequest);
            ChatMessageRespose response = new ChatMessageRespose()
            {
                ChatMessage = operation.Message,
                UserName = operation.UserName,
                AllianceId = 0,
                CurrentTime = DateTime.UtcNow.ToString("dd/MM/yyyy HH/mm/ss")
            };
            var data = response.GetDictionary();
            GameLobbyHandler.log.Info(">>>>>data ="+data);

            return peer.Broadcast(OperationCode.GlobalChat, ReturnCode.OK, data);
        }

        private SendResult AllianceChat(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            GameLobbyHandler.log.Info(">>>>>ALLIANCE CHAT");
            var operation = new ChatMessageRequest(peer.Protocol, operationRequest);
            ChatMessageRespose response = new ChatMessageRespose()
            {
                ChatMessage = operation.Message,
                UserName = operation.UserName,
                AllianceId = 0,
                CurrentTime = DateTime.UtcNow.ToString("dd/MM/yyyy HH/mm/ss")
            };
            var data = response.GetDictionary();
            GameLobbyHandler.log.Info(">>>>>data =" + data);

            return peer.Broadcast(OperationCode.AllianceChat, ReturnCode.OK, data);
        }

        private SendResult GetGateHpRequest(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            var operation = new GateRequest(peer.Protocol, operationRequest);
            if (!operation.IsValid) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: operation.GetErrorMessage());

            var building = peer.Actor.PlayerDataManager.GetPlayerBuildingByLocationId(operation.BuildingLocationId);
            if (building == null || building.StructureType != StructureType.Gate)
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: "Building not found in server.");

            building.GateHp(operation);
            return SendResult.Ok;
        }

        private SendResult RepairGateRequest(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            var operation = new GateRequest(peer.Protocol, operationRequest);
            if (!operation.IsValid) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: operation.GetErrorMessage());

            var building = peer.Actor.PlayerDataManager.GetPlayerBuildingByLocationId(operation.BuildingLocationId);
            if (building == null || building.StructureType != StructureType.Gate) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: "Building not found in server.");

            building.RepairGate(operation);
            return SendResult.Ok;
        }

        public SendResult UpgradeTechnologyRequest(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            return SendResult.InvalidChannel;

            //var operation = new UpgradeTechnologyRequest(peer.Protocol, operationRequest);
            //if (!operation.IsValid) peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: operation.GetErrorMessage());

            //var building = peer.Actor.PlayerDataManager.GetPlayerBuildingByLocationId(operation.BuildingLocationId);
            //if (building != null && building.StructureType == StructureType.Acadamy) building.HandleUpgradeTechnology(operation);
            //else peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: "Building not found in server.");
        }

        public SendResult HandleWoundedHealRequest(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            var operation = new WoundedTroopHealRequest(peer.Protocol, operationRequest);
            if (!operation.IsValid) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: operation.GetErrorMessage());

            var building = peer.Actor.PlayerDataManager.GetPlayerBuildingByLocationId(operation.BuildingLocationId);
            if (building == null)
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: "building not found in server.");

            building.HandleWoundedTroops(operation);
            return SendResult.Ok;
        }

        public SendResult HandleWoundedHealTimerStatus(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            var operation = new WoundedTroopTimerStatusRequest(peer.Protocol, operationRequest);
            if (!operation.IsValid) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: operation.GetErrorMessage());

            var building = peer.Actor.PlayerDataManager.GetPlayerBuildingByLocationId(operation.BuildingLocationId);
            if (building == null) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: "building not found in server.");

            building.WoundedTroopTimerStatusRequest(operation);
            return SendResult.Ok;
        }

        public async Task<SendResult> HandleRecruitToopRequest(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            var operation = new RecruitTroopRequest(peer.Protocol, operationRequest);
            log.Info("============== HandleRecruitToopRequest >" + JsonConvert.SerializeObject(operation));
            if (!operation.IsValid) return SendOperationResponse(peer, operationRequest, new Response(CaseType.Error, operation.GetErrorMessage()));

            var response = await GameService.BUsertroopManager.TrainTroops(peer.Actor.PlayerId, (TroopType)operation.TroopType, operation.TroopLevel, operation.TroopCount, operation.LocationId);

            await GameService.NewRealTimeUpdateManager.UpdatePlayerData(peer.Actor.PlayerId);

            if (response.IsSuccess && response.HasData)
            {
                //update any task that require training troops
                GameService.NewRealTimeUpdateManager.TrainTroopsUpdate(peer.Actor.PlayerId, ((TroopType)operation.TroopType).ToString(), operation.TroopLevel, operation.TroopCount);
            }

/*            var traningTroopResp = new TroopTrainResponse()
            {
                LocationId = operation.LocationId,
                StructureType = operation.StructureType,

                TotalTime = troopInfo.TotalTime,
                TrainingTime = troopInfo.TimeLeft
            };*/


            if (response.IsSuccess && response.HasData)
            {
                var response2 = await GameService.BUsertroopManager.GetFullPlayerData(peer.Actor.PlayerId);
                Dictionary<byte, object> data = null;

                if (response2.IsSuccess)
                {
                    data = GetTroopTrainingData(operation.LocationId, operation.StructureType, (TroopType)operation.TroopType, response2);
                }
                else
                {
                    response.Case = 0;
                    response.Message = "Invalid troop data";
                }

                //                Dictionary<byte, object> data = GetTroopTrainingData(operation.LocationId, operation.StructureType, (TroopType)operation.TroopType, response2);

                //                return SendOperationResponse(peer, operationRequest, response, response.Data);
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.OK, data, debuMsg: response.Message);
//                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.OK, uresponse.GetDictionary(), debuMsg: response.Message);
            }
            else
            {
                return SendOperationResponse(peer, operationRequest, response);
            }
        }

        public async Task<SendResult> HandleRecruitTroopStatus(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            var operation = new TroopTrainingStatusRequest(peer.Protocol, operationRequest);
            log.Info(">>>>>>>>> HandleRecruitTroopStatus >"+JsonConvert.SerializeObject(operation));

            if (!operation.IsValid) return SendOperationResponse(peer, operationRequest, new Response(CaseType.Error, operation.GetErrorMessage()));

            var response = await GameService.BUsertroopManager.GetFullPlayerData(peer.Actor.PlayerId);
            Dictionary<byte, object> data = null;

            if (response.IsSuccess)
            {
                data = GetTroopTrainingData(operation.LocationId, operation.StructureType, (TroopType)operation.TroopType, response);
            }
            else
            {
                response.Case = 0;
                response.Message = "Invalid troop data";
            }
//            return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: "troop not training on location");

            return SendOperationResponse(peer, operationRequest, response, data);
        }

        private Dictionary<byte, object> GetTroopTrainingData(int locationId, int structureType, TroopType troopType, Response<Common.Models.PlayerCompleteData> response)
        {
            Dictionary<byte, object> data = null;

            var troopData = response.Data.Troops.FirstOrDefault(x => x.TroopType == troopType)?.TroopData;
            if (troopData != null && troopData.Count > 0)
            {
                var breakAll = false;
                var troopTraning = troopData.Select(x => x.InTraning).Where(x => x != null)?.ToList();
                if (troopTraning != null && troopTraning.Count > 0)
                {
                    foreach (var traning in troopTraning)
                    {
                        foreach (var troopInfo in traning)
                        {
                            if (troopInfo.BuildingLocId == locationId)
                            {
                                var traningTroopResp = new TroopTrainingTimeResponse()
                                {
                                    LocationId = locationId,
                                    StructureType = structureType,
                                    TotalTime = troopInfo.TotalTime,
                                    TrainingTime = troopInfo.TimeLeft
                                };

                                data = traningTroopResp.GetDictionary();
                                //                                    return peer.SendOperation(operationRequest.OperationCode, ReturnCode.OK, data, debuMsg: response.Message);

                                breakAll = true;
                            }

                            if (breakAll) break;
                        }
                        if (breakAll) break;
                    }
                }
                else
                {
                    response.Case = 0;
                    response.Message = "Troop not training";
                }
            }
            else
            {
                response.Case = 0;
                response.Message = "Troop not found";
            }

            return data;
        }

        public SendResult HandleTroopBoostUp(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            var operation = new TroopTrainingStatusRequest(peer.Protocol, operationRequest);
            if (!operation.IsValid) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: operation.GetErrorMessage());

            var building = peer.Actor.PlayerDataManager.GetPlayerBuilding((StructureType)operation.StructureType, operation.LocationId);
            if (building == null) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: "building not found in server.");

            if (!building.Troops.ContainsKey((TroopType)operation.TroopType)) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: "troop not found in building.");

            building.Troops[(TroopType)operation.TroopType].TrainingTimeBoostUp();
            return SendResult.Ok;
        }

        public async Task<SendResult> HandleCollectResourceRequest(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            var operation = new CollectResourceRequest(peer.Protocol, operationRequest);
            var response = await GameService.BPlayerStructureManager.CollectResource(peer.Actor.PlayerId, operation.LocationId);
            if (!response.IsSuccess || !response.HasData) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: operation.GetErrorMessage());

            var code = string.Empty;
            if (operation.StructureType == (int)StructureType.Farm) code = "Food";
            else if (operation.StructureType == (int)StructureType.Sawmill) code = "Wood";
            else if (operation.StructureType == (int)StructureType.Mine) code = "Ore";

            CollectResourceResponse uresponse = new CollectResourceResponse()
            {
                StructureType = operation.StructureType,
                LocationId = operation.LocationId,
                ResourceValue = response.Data
            };


            GameService.NewRealTimeUpdateManager.CollectResourceUpdate(peer.Actor.PlayerId, code, response.Data);
            await GameService.NewRealTimeUpdateManager.UpdatePlayerData(peer.Actor.PlayerId);

            return peer.SendOperation(operationRequest.OperationCode, ReturnCode.OK, uresponse.GetDictionary(), debuMsg: response.Message);
        }

        public SendResult HandleBoostResources(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            var operation = new ResourceBoostUpRequest(peer.Protocol, operationRequest);
            if (!operation.IsValid) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: operation.GetErrorMessage());

            var building = peer.Actor.PlayerDataManager.GetPlayerBuilding((StructureType)operation.StructureType, operation.LocationId);
            if (building == null) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: "building not found in server.");

            building.BoostResourceGenerationTime(operation);
            return SendResult.Ok;
        }

        public SendResult HandleAttackRequest(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            log.Debug("@@@@@@@ HANDLE ATTACK REQUEST FROM "+peer.Actor.PlayerId);
            var operation = new AttackRequest(peer.Protocol, operationRequest);
            if (!operation.IsValid)
            {
                log.Debug("@@@@ ATTACK INVALID");
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: operation.GetErrorMessage());
            }

            peer.Actor.PlayerAttackHandler.AttackRequest(operation);
            log.Debug("@@@@ ATTACK OK!!");
            return SendResult.Ok;
        }

        public SendResult HandlPlayerConnectToGameServer(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            var operation = new PlayerConnectRequest(peer.Protocol, operationRequest);
            if (!operation.IsValid) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: operation.GetErrorMessage());

            MmoActor actor = null;
            Region city = null;
            IInterestArea iA = null;

            var playerData = GameService.BPlayerManager.GetAllPlayerData(Int32.Parse(operation.PlayerId)).Result.Data;

            if (playerData == null) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: "Player data not found from db.");

            log.InfoFormat("get Player Data from db {0} ", JsonConvert.SerializeObject(playerData));
            var playerPosData = this.GridWorld.GetPlayerPosition(Convert.ToInt32(operation.PlayerId));
            city = playerPosData.region;
            actor = playerPosData.actor;
            iA = playerPosData.iA;
            actor.StartOnReal();
            IPlayerSocketDataManager plADataManager = new PlayerSocketDataManager(playerData, actor);
            log.InfoFormat("player is added in manager with playerid.");
            log.InfoFormat("player enter in world X {0} Y {1} ", city.X, city.Y);
            actor.PlayerSpawn(iA, peer, plADataManager);
            actor.Peer.Actor = actor;  //vice versa mmoactor into the peer and reverse
            var profile = new UserProfileResponse
            {
                UserName = actor.UserName,
                X = actor.Tile.X,
                Y = actor.Tile.Y
            };
            actor.SendEvent(EventCode.UserProfile, profile);
            log.InfoFormat("Send profile data to client X {0} Y {1} userName {2} ", profile.X, profile.Y, profile.UserName);
            GameService.NewRealTimeUpdateManager.TryPushPlayerData(actor.PlayerId, peer.OnQuestUpdate);

            return peer.SendOperation(operationRequest.OperationCode, ReturnCode.OK);
        }

        public SendResult HandlePlayerTeleport(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            //log.InfoFormat("HandlePlayerTeleport Dict {0} ", Helper.DicToString(operationRequest.Parameters));
            var operation = new TeleportLocation(peer.Protocol, operationRequest);
            if (!operation.IsValid) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: operation.GetErrorMessage());

            if (peer.Actor == null) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: "Invalid Request Actor not found.");

            var region = GridWorld.WorldRegions[(int)operation.X][(int)operation.Y];
            if (region.IsBooked) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: "City is already booked by other player.");

            if (!region.IsWalkable)
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.OK, debuMsg: "tile position is not walkable.");

            peer.Actor.Tile.RemoveUserFromCity(peer.Actor);
            peer.Actor.PlayerTeleport(region);
            peer.Actor.InterestArea.UpdateInterestArea(region);

            return peer.SendOperation(operationRequest.OperationCode, ReturnCode.OK);
        }

        public SendResult HandlePlayerJoinKingdomView(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            if (peer == null || peer.Actor == null || peer.Actor.InterestArea == null)
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: "interest area not found.");

            peer.Actor.InterestArea.JoinKingdomView();
            return peer.SendOperation(operationRequest.OperationCode, ReturnCode.OK);
        }

        public SendResult HandlePlayerLeaveKingdomView(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            if (peer == null || peer.Actor == null || peer.Actor.InterestArea == null)
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: "interest area not found.");

            peer.Actor.InterestArea.LeaveKingdomView();
            return peer.SendOperation(operationRequest.OperationCode, ReturnCode.OK);
        }

        public SendResult HandlePlayerCameraMove(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            // log.InfoFormat("HandlePlayerCameraMove Dict {0} ", Helper.DicToString(operationRequest.Parameters));
            var operation = new CameraMoveRequest(peer.Protocol, operationRequest);
            if (!operation.IsValid) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: operation.GetErrorMessage());

            if (peer.Actor != null && peer.Actor.InterestArea != null)
            {
                var region = this.GridWorld.WorldRegions[(int)operation.X][(int)operation.Y];
                peer.Actor.InterestArea.CameraMove(region);
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.OK);
            }

            return SendResult.Ok;
        }

        public SendResult HandleHelpStructure(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            var operation = new HelpStructureRequest(peer.Protocol, operationRequest);
            if (!operation.IsValid) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: operation.GetErrorMessage());

            HelpStructureRespose response = new HelpStructureRespose()
            {
                PlayerId = operation.PlayerId,
                StructureType = operation.StructureType,
                StructureLocationId = operation.StructureLocationId
            };

            peer.Broadcast(OperationCode.HelpStructure, ReturnCode.OK, response.GetDictionary());

            return SendResult.Ok;
        }

        public async Task<SendResult> HandleCreateStructure(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            var operation = new CreateStructureRequest(peer.Protocol, operationRequest);
            if (!operation.IsValid) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: operation.GetErrorMessage());

            if (!GameService.GameBuildingManager.ContainsKey((StructureType)operation.StructureType)) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.Failed, debuMsg: "structure not found in the game.");

            GameService.GameBuildingManager[(StructureType)operation.StructureType].CreateStructureForPlayer(operation, peer.Actor);

            await GameService.NewRealTimeUpdateManager.UpdatePlayerData(peer.Actor.PlayerId);

            return SendResult.Ok;
        }

        public async Task<SendResult> HandleUpgradeStructure(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            var operation = new UpgradeStructureRequest(peer.Protocol, operationRequest);
            if (!operation.IsValid) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: operation.GetErrorMessage());

            GameService.GameBuildingManager[(StructureType)operation.StructureType].UpgradeStructureForPlayer(operation, peer.Actor);
            await GameService.NewRealTimeUpdateManager.UpdatePlayerData(peer.Actor.PlayerId);

            return SendResult.Ok;
        }

        public async Task<SendResult> HandlePlayerBuildingStatus(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            var operation = new PlayerBuildingBuildingStatusRequest(peer.Protocol, operationRequest);
            if (!operation.IsValid) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: operation.GetErrorMessage());

            var playerStructData = await GameService.BPlayerStructureManager.CheckBuildingStatus(peer.Actor.PlayerId, (StructureType)operation.StructureType);

            if (playerStructData == null)
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: "player not found");
            if (!playerStructData.IsSuccess)
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: playerStructData.Message);
            if (!playerStructData.HasData)
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: "player data not found");

            foreach (var building in playerStructData.Data.Value)
            {
                if (building.Location == operation.StructureLocationId)
                {
                    var response = new PlayerBuildingBuildingStatuResponse()
                    {
                        LocationId = building.Location,
                        BuildTime = (int)building.TotalTime,
                        TotalTime = (int)building.TimeLeft
                    };
                    log.InfoFormat("Send Building status to Client location {0} buildType {1} Time {2} ",
                       response.LocationId, playerStructData.Data.ValueId, building.TimeLeft);

                    var dic = response.GetDictionary();
                    //this.Player.SendOperation((byte)OperationCode.PlayerBuildingStatus, ReturnCode.OK, dict);
                    return peer.SendOperation(operationRequest.OperationCode, ReturnCode.OK, dic, debuMsg: playerStructData.Message);
                }
            }

            return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: "structure not found");

            //var building = peer.Actor.PlayerDataManager.GetPlayerBuilding((StructureType)operation.StructureType, operation.StructureLocationId);
            //if (building == null) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: "structure not found in server.");

            //building.SendBuildTimerToClient();
        }


        private void SendBuildingCompleteToBuild(IGorMmoPeer peer, StructureType type, int currentLevel, int location)
        {
            log.Info("**************************************SendBuildingCompleteToBuild START**************************************");
            var timer = new TimerCompleteResponse
            {
                LocationId = location,
                Level = currentLevel,
                StructureType = (int)type
            };

            peer.Actor.SendEvent(EventCode.CompleteTimer, timer);

            log.Info("************************************** SendBuildingCompleteToBuild END **************************************");
        }


        #region Private
        private static SendResult SendOperationResponse(IGorMmoPeer peer, OperationRequest operationRequest, ReturnCode code) => peer.SendOperation(operationRequest.OperationCode, code);
        private static SendResult SendOperationResponse(IGorMmoPeer peer, OperationRequest operationRequest, ReturnCode code, string debugMessage) => peer.SendOperation(operationRequest.OperationCode, code, debugMessage);
        private static SendResult SendOperationResponse(IGorMmoPeer peer, OperationRequest operationRequest, Response response)
        {
            if (!response.IsSuccess) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: response.Message);
            else return peer.SendOperation(operationRequest.OperationCode, ReturnCode.OK, debuMsg: response.Message);
        }
        private static SendResult SendOperationResponse(IGorMmoPeer peer, OperationRequest operationRequest, Response response, Dictionary<byte, object> dict)
        {
            if (!response.IsSuccess) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: response.Message);
            else if (dict == null) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: response.Message);
            else return peer.SendOperation(operationRequest.OperationCode, ReturnCode.OK, dict, debuMsg: response.Message);
        }
        private static SendResult SendOperationResponse<T>(IGorMmoPeer peer, OperationRequest operationRequest, Response response, T data) where T : DictionaryEncode => SendOperationResponse(peer, operationRequest, response, data.GetDictionary());
        #endregion
    }
}

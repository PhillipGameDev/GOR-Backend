using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Photon.SocketServer;
using ExitGames.Logging;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Quest;
using GameOfRevenge.Common.Models.Quest.Template;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Common.Services;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Common.Email;
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Business.Manager.UserData;
using GameOfRevenge.GameApplication;
using GameOfRevenge.Helpers;
using GameOfRevenge.Interface;
using GameOfRevenge.Model;
using GameOfRevenge.Buildings.Handlers;
using GameOfRevenge.Model.Common;

namespace GameOfRevenge.GameHandlers
{
    public class GameLobbyHandler
    {
        public static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public IWorld GridWorld => GameService.WorldHandler.DefaultWorld;

        private readonly IUserTroopManager _userTroopManager = new UserTroopManager();
        private readonly IUserQuestManager questManager = new UserQuestManager();
        private readonly IUserMailManager mailManager = new UserMailManager();

        private readonly string[][] badWords = new string[][]{
new string[]{
"anal","anus","arse","ass","balls","ballsack","bastard","biatch","bitch","bloody","blow job","blowjob","bollock","bollok","boner","boob","bugger","bum","butt","buttplug","clitoris","cock","coon","crap","cunt","damn","dick","dildo","dyke","f u c k","fag","feck","felching","fellate","fellatio","flange","fuck","fudge packer","fudgepacker","God damn","Goddamn","hell","homo","jerk","jizz","knob end","knobend","labia","lmao","lmfao","muff","nigga","nigger","omg","penis","piss","poop","prick","pube","pussy","queer","s hit","scrotum","sex","sh1t","shit","slut","smegma","spunk","tit","tosser","turd","twat","vagina","wank","whore","wtf"
},
new string[]{
"سكس","طيز","شرج","لعق","لحس","مص","تمص","بيضان","ثدي","بز","بزاز","حلمة","مفلقسة","بظر","كس","فرج","شهوة","شاذ","مبادل","عاهرة","جماع","قضيب","زب","لوطي","لواط","سحاق","سحاقية","اغتصاب","خنثي","احتلام","نيك","متناك","متناكة","شرموطة","عرص","خول","قحبة","لبوة"
}};

        public async Task<SendResult> OnLobbyMessageRecived(IGorMmoPeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            try
            {
                log.InfoFormat("Message Recived OpCode {0} Data {1} ", operationRequest.OperationCode, GlobalHelper.DicToString(operationRequest.Parameters));
                if ((OperationCode)operationRequest.OperationCode != OperationCode.PlayerConnectToServer)
                {
                    if ((peer.Actor == null) || (peer.Actor.Fiber == null))
                    {
                        log.Info("*** Message discarded, user offline");
                        return SendResult.Failed;
                    }
                }

                switch ((OperationCode)operationRequest.OperationCode)
                {
                    case OperationCode.PlayerConnectToServer: return await HandlPlayerConnectToGameServer(peer, operationRequest);//1
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

                    case OperationCode.AttackRequest: return await HandleAttackRequest(peer, operationRequest);//15
                    case OperationCode.SendReinforcementsRequest: return await HandleSendReinforcementsRequest(peer, operationRequest); 

                    case OperationCode.WoundedHealReqeust: return HandleWoundedHealRequest(peer, operationRequest);//16
                    case OperationCode.WoundedHealTimerRequest: return HandleWoundedHealTimerStatus(peer, operationRequest);//17
                    case OperationCode.UpgradeTechnology: return UpgradeTechnologyRequest(peer, operationRequest);//18
                    case OperationCode.RepairGate: return RepairGateRequest(peer, operationRequest);//19
                    case OperationCode.GateHp: return GetGateHpRequest(peer, operationRequest);//20

                    case OperationCode.GlobalChat: return await GlobalChat(peer, operationRequest);//21
                    case OperationCode.AllianceChat: return AllianceChat(peer, operationRequest);//29
                    case OperationCode.DeleteChat: return await DeleteChat(peer, operationRequest);

                    case OperationCode.Ping: return peer.SendOperation(operationRequest.OperationCode, ReturnCode.OK);//2
                    case OperationCode.GetInstantBuildCost: return GetInstantBuildCost(peer, operationRequest);//26
                    case OperationCode.InstantBuild: return await HandleInstantBuild(peer, operationRequest);//23
                    case OperationCode.SpeedUpBuildCost: return await SpeedUpBuildCost(peer, operationRequest);//25
                    case OperationCode.SpeedUpBuild: return await HandleSpeedUpBuild(peer, operationRequest);//24
                    case OperationCode.InstantRecruit: return await HandleInstantRecruit(peer, operationRequest);//27
                    case OperationCode.HelpStructureRequest: return await HandleHelpStructure(peer, operationRequest);//28

                    case OperationCode.UpdatePlayerData: return await HandleUpdatePlayerData(peer, operationRequest);//30
                    case OperationCode.UpdateMarchingArmy: return await HandleUpdateMarchingArmy(peer, operationRequest);//36
                    case OperationCode.RecallMarchingArmy: return await HandleRecallMarchingArmy(peer, operationRequest);//37

                    case OperationCode.SendFriendRequest: return await HandleSendFriendRequest(peer, operationRequest);//34
                    case OperationCode.RespondToFriendRequest: return await HandleRespondToFriendRequest(peer, operationRequest);//35
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

        async Task<PlayerUserQuestData> UpdatePlayerData(MmoActor actor)
        {
            log.Info("player " + actor.PlayerId + " data changed - ENTER");
            var plyData = GameService.RealTimeUpdateManagerQuestValidator.GetCachedPlayerData(actor.PlayerId);
            if (plyData == null)
            {
                log.Info("player " + actor.PlayerId + " offline - EXIT");
                return null;
            }

            var data = await GameService.RealTimeUpdateManagerQuestValidator.UpdatePlayerData(plyData);
            if (data != null)
            {
                actor.InternalPlayerDataManager.UpdateData(data);
                UpdateMarchingArmies(actor, data);
            }

            return plyData;
        }

        void UpdateMarchingArmies(MmoActor actor, PlayerCompleteData completeData)
        {
            var marchingArmies = completeData.MarchingArmies;
            if (marchingArmies == null) return;

            foreach (var item in marchingArmies)
            {
                if (!GameService.BRealTimeUpdateManager.UpdateMarchingArmy(item)) continue;

                var target = actor.World.PlayersManager.GetPlayer(item.TargetId);
                var updateMarching = new UpdateMarchingArmyEvent(item);
                actor.SendEvent(EventCode.UpdateMarchingArmyEvent, updateMarching);
                target.SendEvent(EventCode.UpdateMarchingArmyEvent, updateMarching);
            }
        }

        private async Task<SendResult> HandleUpdatePlayerData(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            var plyData = await UpdatePlayerData(peer.Actor);

            return peer.SendOperation(operationRequest.OperationCode, (plyData != null)? ReturnCode.OK : ReturnCode.Failed);
        }

        private async Task<SendResult> HandleUpdateMarchingArmy(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            var plyData = await UpdatePlayerData(peer.Actor);

            return peer.SendOperation(operationRequest.OperationCode, (plyData != null) ? ReturnCode.OK : ReturnCode.Failed);
        }

        private async Task<SendResult> HandleRecallMarchingArmy(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            log.Info("**************** HandleRecallMarchingArmy Start************************");
            var operation = new RetreatMarchingArmyRequest(peer.Protocol, operationRequest);
            if (!operation.IsValid) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: operation.GetErrorMessage());

            var marchingId = operation.MarchingId;
            var marchingArmy = GameService.BRealTimeUpdateManager.GetMarchingArmy(marchingId);
            if (marchingArmy == null) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation);

            if (!marchingArmy.IsRecalling)
            {
                var army = marchingArmy.Base();
                army.ReturnReduction = (int)marchingArmy.TimeLeftForTask;
                army.Recall = marchingArmy.Distance - army.ReturnReduction;

                var armyJson = JsonConvert.SerializeObject(army);
                var resp = await GameService.BPlayerManager.UpdatePlayerDataID(peer.Actor.PlayerId, marchingId, armyJson);
                if (!resp.IsSuccess)
                {
                    return peer.SendOperation(operationRequest.OperationCode, ReturnCode.Failed, debuMsg: resp.Message);
                }
                marchingArmy.ReturnReduction = army.ReturnReduction;
                marchingArmy.Recall = army.Recall;
            }

            GameService.BRealTimeUpdateManager.UpdateMarchingArmy(marchingArmy);
//            var updateResp = new UpdateMarchingArmyEvent(marchingArmy)
//            {
//                MarchingId = operation.MarchingId
//            };
            log.Info("**************** HandleRecallMarchingArmy End************************");
            var updateMarching = new UpdateMarchingArmyEvent(marchingArmy);
            peer.Actor.SendEvent(EventCode.UpdateMarchingArmyEvent, updateMarching);

            return peer.SendOperation(operationRequest.OperationCode, ReturnCode.OK);
        }

        async Task<SendResult> HandleSendFriendRequest(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            log.Info("**************** HandleSendFriendRequest Start************************");
            var operation = new FriendRequest(peer.Protocol, operationRequest);
            if (!operation.IsValid) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: operation.GetErrorMessage());

            var playerId = peer.Actor.PlayerId;
            var targetPlayerId = operation.TargetPlayerId;
            var response = await GameService.BUserFriendsManager.SendFriendRequest(playerId, targetPlayerId);
            log.Info("friend request response = " + Newtonsoft.Json.JsonConvert.SerializeObject(response));
            if (!response.IsSuccess)
            {
                var retCode = ReturnCode.InvalidOperation;
                if (response.Case == 200)
                {
                    retCode = ReturnCode.UserNotFoundInInterestArea;
                }
                else if (response.Case == 201)
                {
                    retCode = ReturnCode.InvalidOperationParameter;
                }
                return peer.SendOperation(operationRequest.OperationCode, retCode, debuMsg: response.Message);
            }

            var resp = new FriendRequestResponse()
            {
                RequestId = (response.Data != null) ? response.Data.RequestId : 0,
                FromPlayerId = playerId,
                ToPlayerId = targetPlayerId
            };

            if (response.Case == 100)
            {
                var targetActor = peer.Actor.World.PlayersManager.GetPlayer(targetPlayerId);
                if (targetActor != null)
                {
                    await UpdatePlayerData(targetActor);
                    if (targetActor.Peer != null)
                    {
                        targetActor.Peer.SendOperation(OperationCode.RespondToFriendRequest, ReturnCode.OK, resp.GetDictionary());
                    }
                }
            }
            log.Info("**************** HandleSendFriendRequest End************************");

            return peer.SendOperation(operationRequest.OperationCode, ReturnCode.OK, resp.GetDictionary());
        }

        async Task<SendResult> HandleRespondToFriendRequest(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            log.Info("**************** HandleRespondToFriendRequest Start************************");
            var operation = new RespondToFriendRequest(peer.Protocol, operationRequest);
            if (!operation.IsValid) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: operation.GetErrorMessage());

            var playerId = peer.Actor.PlayerId;
            var targetPlayerId = operation.TargetPlayerId;
            var response = await GameService.BUserFriendsManager.RespondToFriendRequest(targetPlayerId, playerId, operation.Value);
            log.Info("respond friend request = " + Newtonsoft.Json.JsonConvert.SerializeObject(response));
            if (!response.IsSuccess)
            {
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: response.Message);
            }

            var resp = new RespondToFriendRequestResponse()
            {
                RequestId = operation.RequestId,
                FromPlayerId = targetPlayerId,
                ToPlayerId = playerId,
                Flags = operation.Value
            };

            var targetActor = peer.Actor.World.PlayersManager.GetPlayer(targetPlayerId);
            if (targetActor != null)
            {
                await UpdatePlayerData(targetActor);
                if (targetActor.Peer != null)
                {
                    targetActor.Peer.SendOperation(OperationCode.RespondToFriendRequest, ReturnCode.OK, resp.GetDictionary());
                }
            }
            log.Info("**************** HandleRespondToFriendRequest End************************");

            return peer.SendOperation(operationRequest.OperationCode, ReturnCode.OK, resp.GetDictionary());
        }


        private async Task<SendResult> HandleInstantRecruit(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            var operation = new RecruitTroopRequest(peer.Protocol, operationRequest);
            var troopType = (TroopType)operation.TroopType;
            var troopLevel = operation.TroopLevel;
            var troopCount = operation.TroopCount;
            var recruitResp = await GameService.InstantProgressManager.InstantRecruitTroops(peer.Actor.PlayerId,
                                                    operation.LocationId, troopType, troopLevel, troopCount);

            string msg = null;
            if (!recruitResp.IsSuccess)
            {
                msg = recruitResp.Message;
            }
            else if (!recruitResp.HasData)
            {
                msg = "player data not found";
            }

            if (msg != null)
            {
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: msg);
            }

            var playerQuestData = await UpdatePlayerData(peer.Actor);
            _ = GameService.BUserQuestManager.CheckQuestProgressForTrainTroops(playerQuestData, troopType, troopLevel, troopCount);

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
            var structureType = (StructureType)operation.StructureType;
            var level = operation.StructureLevel;
            var location = operation.StructureLocationId;
            log.Info($"StructureType: {structureType}");
            log.Info($"StructureLocationId: {location}");
            log.Info($"StructureLevel: {level}");

            var response = await GameService.InstantProgressManager.InstantBuildStructure(peer.Actor.PlayerId, structureType, level, location);
            if (!response.IsSuccess)
            {
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: response.Message);
            }

            await UpdatePlayerData(peer.Actor);

            var building = response.Data.StructureData.Value.Find(x => (x.Location == location));
            if (building == null)
            {
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: "Structure not supported");
            }

/*            IPlayerBuildingManager buildingManager = null;
            var internalDataManager = peer.Actor.InternalPlayerDataManager;
            if (internalDataManager.PlayerBuildings.ContainsKey(structureType))
            {
                buildingManager = internalDataManager.PlayerBuildings[structureType].Find(x => (x.Location == location));
            }

            if (buildingManager != null)
            {
                buildingManager.PlayerStructureData.Duration = 0;
            }
            else
            {
                var structure = new UserStructureData()
                {
                    Id = response.Data.StructureData.Id,
                    DataType = response.Data.StructureData.DataType,
                    ValueId = response.Data.StructureData.ValueId,
                    Value = response.Data.StructureData.Value.Where(x => (x.Location == location)).ToList()
                };

                internalDataManager.AddStructureOnPlayer(structure);
            }*/

            log.Info("**************** HandleInstantBuild End************************");

//            await GameService.NewRealTimeUpdateManager.PlayerDataChanged(peer.Actor.PlayerId);//TODO: required?

            var obj = new StructureCreateUpgradeResponse()
            {
                WorkerId = response.Data.WorkerId,
                StructureLevel = building.Level,
                StructureLocationId = building.Location,
                StructureType = (int)structureType,
            };
            return peer.SendOperation(operationRequest.OperationCode, ReturnCode.OK, obj.GetDictionary());
        }

        private async Task<SendResult> HandleSpeedUpBuild(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            log.Info("**************** HandleSpeedUpBuild Start************************");
            var operation = new InstantBuildRequest(peer.Protocol, operationRequest);

            var structureType = (StructureType)operation.StructureType;
            var location = operation.StructureLocationId;
            var response = await GameService.InstantProgressManager.SpeedUpBuildStructure(peer.Actor.PlayerId, location);
            if (!response.IsSuccess)
            {
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: response.Message);
            }

            await UpdatePlayerData(peer.Actor);

            var respDetails = response.Data.Value.Find(x => (x.Location == location));
            if (respDetails == null)
            {
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: "Structure was null");
            }

/*            IPlayerBuildingManager buildingManager = null;
            var internalDataManager = peer.Actor.InternalPlayerDataManager;
            if (internalDataManager.PlayerBuildings.ContainsKey(structureType))
            {
                buildingManager = internalDataManager.PlayerBuildings[structureType].Find(x => (x.Location == location));
            }

            if (buildingManager != null)
            {
                buildingManager.PlayerStructureData.Duration = 0;
            }
            else
            {
                var structure = new UserStructureData()
                {
                    Id = response.Data.Id,
                    DataType = response.Data.DataType,
                    ValueId = response.Data.ValueId,
                    Value = response.Data.Value.Where(x => (x.Location == location)).ToList()
                };

                internalDataManager.AddStructureOnPlayer(structure);
            }*/

            log.Info("**************** HandleSpeedUpBuild End************************");

            var obj = new StructureCreateUpgradeResponse()
            {
                WorkerId = 0,
                StructureLevel = respDetails.Level,
                StructureLocationId = location,
                StructureType = (int)structureType,
            };

            var result = peer.SendOperation(operationRequest.OperationCode, ReturnCode.OK, obj.GetDictionary());
            SendBuildingCompleteToBuild(peer, structureType, respDetails.Level, location);
            return result;
        }

        private async Task<SendResult> DeleteChat(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            GameLobbyHandler.log.Info(">>>>>DELETE CHAT");
            var operation = new DeleteChatMessageRequest(peer.Protocol, operationRequest);
            if (!operation.IsValid)
            {
                var msg = operation.GetErrorMessage();
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: msg);
            }

            var resp = await GameService.BChatManager.DeleteMessage(peer.Actor.PlayerId, operation.ChatId);
            if (!resp.IsSuccess || !resp.HasData)
            {
                var dbgMsg = "Failed to delete message";
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.Failed, debuMsg: dbgMsg);
            }

            var obj = new DeleteChatMessageRespose()
            {
                ChatId = operation.ChatId,
                Flags = resp.Data.Flags
            };

            return peer.Broadcast(operationRequest.OperationCode, ReturnCode.OK, obj.GetDictionary());
        }

        private async Task<SendResult> GlobalChat(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            GameLobbyHandler.log.Info(">>>>>GLOBAL CHAT");
            var operation = new ChatMessageRequest(peer.Protocol, operationRequest);
            if (!operation.IsValid)
            {
                var msg = operation.GetErrorMessage();
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: msg);
            }

            var message = operation.ChatMessage;
            foreach (var group in badWords)
            {
                foreach (var word in group)
                {
                    string pattern = @"\b" + Regex.Escape(word) + @"\b";
                    if (Regex.IsMatch(message, pattern, RegexOptions.IgnoreCase))
                    {
                        message = Regex.Replace(message, pattern, new string('*', word.Length), RegexOptions.IgnoreCase);
                    }
                }
            }

            var resp = await GameService.BChatManager.CreateMessage(peer.Actor.PlayerId, message);
            GameLobbyHandler.log.Info("success = "+resp.IsSuccess+"  "+Newtonsoft.Json.JsonConvert.SerializeObject(resp.Data));
            if (resp.IsSuccess && resp.HasData)
            {
                var response = new ChatMessageRespose()
                {
                    ChatId = resp.Data.ChatId,
                    PlayerId = peer.Actor.PlayerData.PlayerId, //operation.PlayerId,
                    Username = peer.Actor.PlayerData.Name, //operation.Username,
                    VIPPoints = peer.Actor.PlayerData.VIPPoints,
                    AllianceId = 0,//global chat alliance is zero. //operation.AllianceId,
                    Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds(), //ToString("dd/MM/yyyy HH:mm:ss"),
                    ChatMessage = message
                };
                var data = response.GetDictionary();
                GameLobbyHandler.log.Info(">>>>>data =" + data);

                var questUpdated = await CompleteCustomTaskQuest(peer.Actor.PlayerId, CustomTaskType.SendGlobalChat);
                if (questUpdated)
                {
                    peer.Actor.SendEvent(EventCode.UpdateQuest, new Dictionary<byte, object>());
                }

                return peer.Broadcast(OperationCode.GlobalChat, ReturnCode.OK, data);
            }

            var dbgMsg = "Failed delivery message";
            return peer.SendOperation(operationRequest.OperationCode, ReturnCode.Failed, debuMsg: dbgMsg);
        }

        private SendResult AllianceChat(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            GameLobbyHandler.log.Info(">>>>>ALLIANCE CHAT");
            var operation = new ChatMessageRequest(peer.Protocol, operationRequest);
            ChatMessageRespose response = new ChatMessageRespose()
            {
                PlayerId = peer.Actor.PlayerData.PlayerId, //operation.PlayerId,
                Username = peer.Actor.PlayerData.Name, //operation.Username,
                VIPPoints = peer.Actor.PlayerData.VIPPoints,
                AllianceId = operation.AllianceId,
                Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds(),//ToString("dd/MM/yyyy HH:mm:ss"),
                ChatMessage = operation.ChatMessage,
            };
            var data = response.GetDictionary();
            GameLobbyHandler.log.Info(">>>>>data =" + data);

            return peer.Broadcast(OperationCode.AllianceChat, ReturnCode.OK, data);
        }

        private SendResult GetGateHpRequest(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            var operation = new GateRequest(peer.Protocol, operationRequest);
            if (!operation.IsValid) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: operation.GetErrorMessage());

            var building = peer.Actor.InternalPlayerDataManager.GetPlayerBuildingByLocationId(operation.BuildingLocationId);
            if (building == null || building.StructureType != StructureType.Gate)
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: "Building not found in server.");

            building.GateHp(operation);
            return SendResult.Ok;
        }

        private SendResult RepairGateRequest(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            log.Debug("@@@@@@@ HANDLE REPAIR GATE FROM " + peer.Actor.PlayerId);

            var operation = new GateRequest(peer.Protocol, operationRequest);
            if (!operation.IsValid)
            {
                log.Debug("@@@@@@@ HANDLE REPAIR GATE INVALID REQUEST");
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: operation.GetErrorMessage());
            }

            log.Debug("internal player data exists ="+(peer.Actor.InternalPlayerDataManager != null));
            var building = peer.Actor.InternalPlayerDataManager.GetPlayerBuildingByLocationId(operation.BuildingLocationId);
            if ((building == null) || (building.StructureType != StructureType.Gate))
            {
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: "Building not found in server.");
            }

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

            var building = peer.Actor.InternalPlayerDataManager.GetPlayerBuildingByLocationId(operation.BuildingLocationId);
            if (building == null)
            {
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: "building not found in server.");
            }

            building.HandleWoundedTroops(operation);
            return SendResult.Ok;
        }

        public SendResult HandleWoundedHealTimerStatus(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            var operation = new WoundedTroopTimerStatusRequest(peer.Protocol, operationRequest);
            if (!operation.IsValid) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: operation.GetErrorMessage());

            var building = peer.Actor.InternalPlayerDataManager.GetPlayerBuildingByLocationId(operation.BuildingLocationId);
            if (building == null) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: "building not found in server.");

            building.WoundedTroopTimerStatusRequest(operation);
            return SendResult.Ok;
        }

        public async Task<SendResult> HandleRecruitToopRequest(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            var operation = new RecruitTroopRequest(peer.Protocol, operationRequest);
            log.Info("============== HandleRecruitToopRequest >" + JsonConvert.SerializeObject(operation));
            if (!operation.IsValid)
            {
                log.Info("INVALID OPERATION : " + operation.GetErrorMessage());
                return SendOperationResponse(peer, operationRequest, new Response(CaseType.Error, operation.GetErrorMessage()));
            }

            var troopType = (TroopType)operation.TroopType;
            var troopLevel = operation.TroopLevel;
            var troopCount = operation.TroopCount;
            var locationId = operation.LocationId;
            var resp = await GameService.BUsertroopManager.TrainTroops(peer.Actor.PlayerId, troopType, troopLevel, troopCount, locationId);
            if (!resp.IsSuccess)
            {
                log.Info("TRAIN TROOP FAIL " + resp.Message);
                return SendOperationResponse(peer, operationRequest, resp);
            }

            var (response, data) = await GetTroopStatus(locationId, operation.StructureType, troopType, peer.Actor.PlayerId);
            if ((data == null) || (data.Keys.Count == 0))
            {
                var trainingTroopResp = new TroopTrainingTimeResponse()
                {
                    LocationId = locationId,
                    StructureType = operation.StructureType
                };
                data = trainingTroopResp.GetDictionary();
            }

            var playerQuestData = await UpdatePlayerData(peer.Actor);
            _ = GameService.BUserQuestManager.CheckQuestProgressForTrainTroops(playerQuestData, troopType, troopLevel, troopCount);

            return SendOperationResponse(peer, operationRequest, response, data);
        }

        private async Task<(Response<PlayerCompleteData>, Dictionary<byte, object>)> GetTroopStatus(int locationId, int structureType, TroopType troopType, int playerId)
        {
            Dictionary<byte, object> data = null;
            var response = await BaseUserDataManager.GetFullPlayerData(playerId);
            if (response.IsSuccess)
            {
                data = GetTroopTrainingData(locationId, structureType, troopType, response.Data.Troops);
            }

            if (data == null)
            {
                response.Case = 0;
                response.Message = "Invalid troop data";
            }
            //            return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: "troop not training on location");

            return (response, data);
        }

        public async Task<SendResult> HandleRecruitTroopStatus(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            var operation = new TroopTrainingStatusRequest(peer.Protocol, operationRequest);
            log.Info(">>>>>>>>> HandleRecruitTroopStatus >"+JsonConvert.SerializeObject(operation));

            if (!operation.IsValid)
            {
                return SendOperationResponse(peer, operationRequest, new Response(CaseType.Error, operation.GetErrorMessage()));
            }

            var (response, data) = await GetTroopStatus(operation.LocationId, operation.StructureType, (TroopType)operation.TroopType, peer.Actor.PlayerId);
            return SendOperationResponse(peer, operationRequest, response, data);
        }

        private Dictionary<byte, object> GetTroopTrainingData(int locationId, int structureType, TroopType troopType, List<TroopInfos> troops)
        {
            if (!CacheTroopDataManager.TroopTypes.Contains(troopType)) return null;

            Dictionary<byte, object> data = null;
            List<List<UnavaliableTroopInfo>> troopTraining = null;
            var troopData = troops.FirstOrDefault(x => x.TroopType == troopType)?.TroopData;
            if (troopData?.Count > 0)
            {
                troopTraining = troopData.Select(x => x.InTraning).Where(x => x != null)?.ToList();
            }
            if (troopTraining?.Count > 0)
            {
                foreach (var training in troopTraining)
                {
                    var troopInfo = training.Find(x => (x.BuildingLocId == locationId));
                    if (troopInfo == null) continue;

                    var trainingTroopResp = new TroopTrainingTimeResponse()
                    {
                        LocationId = locationId,
                        StructureType = structureType,
                        TotalTime = troopInfo.Duration,
                        TrainingTime = (int)troopInfo.TimeLeft
                    };

                    data = trainingTroopResp.GetDictionary();
                    //                                    return peer.SendOperation(operationRequest.OperationCode, ReturnCode.OK, data, debuMsg: response.Message);

                    break;
                }
            }
            else
            {
                data = new Dictionary<byte, object>();
            }

            return data;
        }

        public SendResult HandleTroopBoostUp(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            var operation = new TroopTrainingStatusRequest(peer.Protocol, operationRequest);
            if (!operation.IsValid) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: operation.GetErrorMessage());

            var building = peer.Actor.InternalPlayerDataManager.GetPlayerBuilding((StructureType)operation.StructureType, operation.LocationId);
            if (building == null) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: "building not found in server.");

            if (!building.Troops.ContainsKey((TroopType)operation.TroopType)) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: "troop not found in building.");

            building.Troops[(TroopType)operation.TroopType].TrainingTimeBoostUp();
            return SendResult.Ok;
        }

        public async Task<SendResult> HandleCollectResourceRequest(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            var operation = new CollectResourceRequest(peer.Protocol, operationRequest);
            log.Info(">>>>>>>>> collect resource >" + JsonConvert.SerializeObject(operation));

            var building = peer.Actor.InternalPlayerDataManager.GetPlayerBuilding(operation.StructureType, operation.LocationId);
            if ((building == null) || !(building is ResourceGenerator))
            {
                var msg = "Building not found in server.";
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: msg);
            }

            var bldResource = (ResourceGenerator)building;
            var extraMultiplier = (bldResource.BoostUp.Percentage / 100f);
            var response = await GameService.BPlayerStructureManager.CollectResource(peer.Actor.PlayerId, operation.LocationId, extraMultiplier);
            if (!response.IsSuccess || !response.HasData)
            {
                var msg = operation.GetErrorMessage();
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: msg);
            }

            await UpdatePlayerData(peer.Actor);

            var resourceType = bldResource.Resource.ResourceInfo.Code;
            var resourceCollected = response.Data.CollectedResource;
            var uresponse = new CollectResourceResponse()
            {
                StructureType = operation.StructureType,
                LocationId = operation.LocationId,
                ResourceCollected = resourceCollected.Collected,
                ResourceTotal = resourceCollected.Value
            };
            log.Info(">>>>>>>>> data to send >" + JsonConvert.SerializeObject(uresponse));

            var playerData = GameService.RealTimeUpdateManagerQuestValidator.GetCachedPlayerData(peer.Actor.PlayerId);
            _ = GameService.BUserQuestManager.CheckQuestProgressForCollectResourceAsync(playerData, resourceType, resourceCollected.Collected);

            return peer.SendOperation(operationRequest.OperationCode, ReturnCode.OK, uresponse.GetDictionary(), debuMsg: response.Message);
        }

        public SendResult HandleBoostResources(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            log.Debug("@@@@@@@ HANDLE BOOST RESOURCES FROM " + peer.Actor.PlayerId);
            string errMsg = null;
            var operation = new ResourceBoostUpRequest(peer.Protocol, operationRequest);
            if (operation.IsValid)
            {
                log.Debug("internal player data exists =" + (peer.Actor.InternalPlayerDataManager != null));
                var building = peer.Actor.InternalPlayerDataManager.GetPlayerBuilding((StructureType)operation.StructureType, operation.LocationId);
                if ((building != null) && (building is ResourceGenerator bldResource))
                {
                    log.Debug(">> building resource = "+Newtonsoft.Json.JsonConvert.SerializeObject(bldResource));
                    if (operation.SetBoost)
                    {
                        bldResource.BoostResourceGenerationTime();
                    }
                    else if (bldResource.BoostUp.TimeLeft == 0)
                    {
                        log.Debug("@@@@@@@ HANDLE BOOST RESOURCES STARTED");
                        return peer.SendOperation(operationRequest.OperationCode, ReturnCode.OK, null, debuMsg: "OK");
                    }

                    log.Debug("creating response");
                    var response = new ResourceBoostUpResponse()
                    {
                        StructureType = operation.StructureType,
                        LocationId = operation.LocationId,
                        StartTime = bldResource.BoostUp.StartTime.ToString("s") + "Z",
                        Duration = bldResource.BoostUp.Duration,
                        Percentage = bldResource.BoostUp.Percentage
                    };

                    log.Debug("@@@@@@@ HANDLE BOOST RESOURCES RESTARTED");
                    return peer.SendOperation(operationRequest.OperationCode, ReturnCode.OK, response.GetDictionary(), debuMsg: "OK");
                }

                errMsg = "Building not found in server.";
            }
            if (errMsg == null) errMsg = operation.GetErrorMessage();

            return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: errMsg);
        }

        public async Task<SendResult> HandleAttackRequest(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            log.Debug("@@@@@@@ HANDLE ATTACK REQUEST FROM " + peer.Actor.PlayerId);
            string errMsg = null;
            var operation = new SendArmyRequest(peer.Protocol, operationRequest);
            if (operation.IsValid)
            {
                var success = await peer.Actor.PlayerAttackHandler.AttackRequestAsync(operation);
                if (!success) return SendResult.Failed;

                var questUpdated = await CompleteCustomTaskQuest(peer.Actor.PlayerId, CustomTaskType.AttackPlayer);
                if (questUpdated)
                {
                    peer.Actor.SendEvent(EventCode.UpdateQuest, new Dictionary<byte, object>());
                }

                return SendResult.Ok;
            }
            if (errMsg == null) errMsg = operation.GetErrorMessage();

            log.Debug("@@@@ ATTACK INVALID");
            return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: errMsg);
        }

        public async Task<SendResult> HandleSendReinforcementsRequest(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            log.Debug("@@@@@@@ HANDLE SEND REINFORCEMENTS REQUEST FROM " + peer.Actor.PlayerId);
            string errMsg = null;
            var operation = new SendArmyRequest(peer.Protocol, operationRequest);
            if (operation.IsValid)
            {
                var success = await peer.Actor.PlayerAttackHandler.SendReinforcementsAsync(operation);
                if (!success) return SendResult.Failed;

                return SendResult.Ok;
            }
            if (errMsg == null) errMsg = operation.GetErrorMessage();

            log.Debug("@@@@ SEND REINFORCEMENTS INVALID");
//            var msg = operation.GetErrorMessage();
            return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: errMsg);
        }

        public async Task<SendResult> HandlPlayerConnectToGameServer(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            var operation = new PlayerConnectRequest(peer.Protocol, operationRequest);
            if (!operation.IsValid) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: operation.GetErrorMessage());

            log.Debug("@@@@@@@ HANDLE PLAYER CONNECT FROM " + operation.PlayerId);
            var playerData = GameService.BPlayerManager.GetAllPlayerData(operation.PlayerId).Result.Data;
            if (playerData == null)
            {
                var dbgMsg = "Player data not found from db.";
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: dbgMsg);
            }
            log.InfoFormat("get Player Data from db {0} ", JsonConvert.SerializeObject(playerData));

            var playerId = operation.PlayerId;
            var playerInfo = GameService.BAccountManager.GetAccountInfo(playerId).Result.Data;
            log.InfoFormat("get Player Info from db {0} ", JsonConvert.SerializeObject(playerInfo));

            (MmoActor actor, IInterestArea interestArea) = await GridWorld.GetPlayerPositionAsync(playerId, playerInfo);
            if (actor == null)
            {
                var dbgMsg = "can't locate player in world map";
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: dbgMsg);
            }

            log.InfoFormat("player is added in manager with playerid {0}", playerId);
            log.InfoFormat("player enter in world X {0} Y {1} ", actor.WorldRegion.X, actor.WorldRegion.Y);

            actor.StartOnReal(); //init fiber scheduler class before instantiate any building.
            PlayerSocketDataManager playerDataManager = null;
            try
            {
                playerDataManager = new PlayerSocketDataManager(playerData, actor);
            }
            catch
            {
                var msg = "Error generating player data.";
                log.Info("******** " + msg + " " + playerId);
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: msg);
            }

            actor.PlayerSpawn(interestArea, peer, playerDataManager);
            actor.Peer.Actor = actor;  //vice versa mmoactor into the peer and reverse
//            var vipPts = peer.Actor.PlayerData.VIPPoints;//playerInfo.VIPPoints;
            var profile = new UserProfileResponse
            {
                AllianceId = playerInfo.AllianceId,
                PlayerId = playerId,
                UserName = playerInfo.Name,
                X = actor.WorldRegion.X,
                Y = actor.WorldRegion.Y,
                VIPPoints = playerInfo.VIPPoints,
                KingLevel = playerInfo.KingLevel,
                CastleLevel = playerInfo.CastleLevel,
                LastLogin = playerInfo.LastLogin.ToUniversalTime().ToString("s") + "Z"
            };
            actor.UpdatePlayerInfo(playerInfo);

            actor.SendEvent(EventCode.UserProfile, profile);
            log.InfoFormat("Send profile data to client X {0} Y {1} userName {2} castle {3} ", profile.X, profile.Y, profile.UserName, profile.CastleLevel);

            GameService.RealTimeUpdateManagerQuestValidator.TryAddPlayerQuestData(actor.PlayerId, questUpdated =>
            {
                if (questUpdated == null) return;

                var objResp = new QuestUpdateResponse()
                {
                    IsSuccess = true,
                    Message = "Update quest data",

                    QuestId = questUpdated.QuestId,
                    Completed = questUpdated.Completed,
                    ProgressData = questUpdated.Completed? null : questUpdated.ProgressData
                };

                new DelayedAction().WaitForCallBack(() =>
                {
                    try
                    {
                        actor.SendEvent(EventCode.UpdateQuest, objResp);
                    }catch (Exception ex)
                    {
                        log.Info("EXCEPTION sending event " + ex.Message);
                    }
                }, 100);
            });


            var attackList = GameService.BRealTimeUpdateManager.GetAllAttackDataForDefender(actor.PlayerId);
            if ((attackList != null) && (attackList.Count > 0))
            {
                var watchLevel = KingdomPvPManager.GetWatchLevel(playerData);
                foreach (var item in attackList)
                {
                    item.AttackData.WatchLevel = watchLevel;
                    var attackResponse = new AttackResponse(item.AttackData);//attack / under attack event
                    actor.SendEvent(EventCode.UnderAttack, attackResponse);
                }
            }
            else
            {
                log.InfoFormat("Under attack data not found for this user when join game {0} ", actor.PlayerId);
            }

            log.Debug("@@@@ END PLAYER CONNECT "+actor.PlayerId);
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

            peer.Actor.WorldRegion.RemovePlayerFromRegion(peer.Actor);
            peer.Actor.PlayerTeleport(region);
            peer.Actor.InterestArea.UpdateInterestArea(region);

            return peer.SendOperation(operationRequest.OperationCode, ReturnCode.OK);
        }

        public SendResult HandlePlayerJoinKingdomView(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            if (peer == null || peer.Actor == null || peer.Actor.InterestArea == null)
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: "interest area not found.");

            peer.Actor.InterestArea.JoinKingdomView();
            var joinResp = new JoinKingdomResponse()
            {
                WorldSizeX = (short)peer.Actor.World.Area.Max.X,
                WorldSizeY = (short)peer.Actor.World.Area.Max.Y
            };
            var data = joinResp.GetDictionary();

            return peer.SendOperation(operationRequest.OperationCode, ReturnCode.OK, data);
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
            log.InfoFormat("HandlePlayerCameraMove Dict {0} ", GlobalHelper.DicToString(operationRequest.Parameters));
            var operation = new CameraMoveRequest(peer.Protocol, operationRequest);
            if (!operation.IsValid) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: operation.GetErrorMessage());

            if ((peer.Actor != null) && (peer.Actor.InterestArea != null))
            {
                var x = (int)operation.X;
                var y = (int)operation.Y;
                var regions = GridWorld.WorldRegions;
                log.Info(" world =" + GridWorld.WorldId + "  " + GridWorld.Name);
                log.Info("Regions = " + regions.Length + " x " + regions[0].Length);
                var outOfBounds = (x >= regions.Length) || (y >= regions[0].Length) || (x < 0) || (y < 0);
                if (outOfBounds)
                {
                    log.Info("camera out of bounds");
                    return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation);
                }
                else
                {
                    log.Info($"Move to x:{x} y:{y}");
                    peer.Actor.InterestArea.CameraMove(regions[x][y]);
                    return peer.SendOperation(operationRequest.OperationCode, ReturnCode.OK);
                }
            }

            return SendResult.Ok;
        }

/*        public SendResult HandleHelpStructure(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            var operation = new HelpStructureRequest(peer.Protocol, operationRequest);
            if (!operation.IsValid) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: operation.GetErrorMessage());

            var response = new HelpStructureRespose()
            {
                TargetPlayerId = operation.TargetPlayerId,
                StructureType = operation.StructureType,
                StructureLocationId = operation.StructureLocationId,
                Duration = 60
            };

            peer.Broadcast(OperationCode.HelpStructure, ReturnCode.OK, response.GetDictionary());

            return SendResult.Ok;
        }*/

        private async Task<SendResult> HandleHelpStructure(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            log.Info("**************** HandleHelpStructure Start************************");
            var operation = new HelpStructureRequest(peer.Protocol, operationRequest);
            if (!operation.IsValid) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: operation.GetErrorMessage());

            var targetPlayerId = operation.TargetPlayerId;
            var structureType = (StructureType)operation.StructureType;
            var location = operation.StructureLocationId;
            var seconds = operation.TotalTime;

            var response = await GameService.BPlayerStructureManager.HelpBuilding(peer.Actor.PlayerId, targetPlayerId, structureType, location, seconds);
            if (!response.IsSuccess)
            {
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: response.Message);
            }

            var targetBuilding = response.Data.Value.Find(x => (x.Location == location));
            var targetActor = peer.Actor.World.PlayersManager.GetPlayer(targetPlayerId);
            if (targetActor != null)
            {
                await UpdatePlayerData(targetActor);
/*                try
                {
                    var targetInternalBuildings = targetActor.InternalPlayerDataManager.PlayerBuildings;
                    if (targetInternalBuildings.ContainsKey(structureType))
                    {
                        var structuresManager = targetInternalBuildings[structureType].Find(x => (x.Location == location));
                        var buildingLoc = structuresManager.PlayerStructureData;//.Value.Find(x => (x.Location == location));
                        buildingLoc.Duration = targetBuilding.Duration;

                        if (targetBuilding.Duration == 0)
                        {
                            SendBuildingCompleteToBuild(targetActor.Peer, structureType, targetBuilding.Level, location);
                        }
                    }
                }
                catch { }*/
            }

            var resp = new HelpStructureRespose()
            {
                PlayerId = peer.Actor.PlayerId,
                TargetPlayerId = targetPlayerId,
                StructureType = (int)structureType,
                StructureLocationId = location,
                Duration = targetBuilding.Duration,
                TotalTime = seconds
            };
            //TODO:broadcast to alliance only
            peer.Broadcast(OperationCode.HelpStructure, ReturnCode.OK, resp.GetDictionary());

            log.Info("**************** HandleHelpStructure End************************");

            return peer.SendOperation(operationRequest.OperationCode, ReturnCode.OK, debuMsg: response.Message);
        }

        public async Task<SendResult> HandleCreateStructure(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            log.Info("**************** HandleCreateStructure Start************************");
            var operation = new CreateStructureRequest(peer.Protocol, operationRequest);
            if (!operation.IsValid)
            {
                log.Info("**************** HandleCreateStructure End invalid************************");
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: operation.GetErrorMessage());
            }

            var structureType = (StructureType)operation.StructureType;
            if (!GameService.GameBuildingManagerInstances.ContainsKey(structureType))
            {
                log.Info("**************** HandleCreateStructure End not found************************");
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.Failed, debuMsg: "structure not found in the game.");
            }

            GameService.GameBuildingManagerInstances[structureType].CreateStructureForPlayer(operation, peer.Actor);

            await UpdatePlayerData(peer.Actor);

            log.Info("**************** HandleCreateStructure End************************");
            return SendResult.Ok;
        }

        public async Task<SendResult> HandleUpgradeStructure(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            log.Debug("@@@@@@@ HANDLE UPGRADE STRUCTURE REQUEST FROM " + peer.Actor.PlayerId);

            var operation = new UpgradeStructureRequest(peer.Protocol, operationRequest);
            if (!operation.IsValid)
            {
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: operation.GetErrorMessage());
            }

            var success = false;
            if (GameService.GameBuildingManagerInstances.ContainsKey((StructureType)operation.StructureType))
            {
                success = GameService.GameBuildingManagerInstances[(StructureType)operation.StructureType].UpgradeStructureForPlayer(operation, peer.Actor);
            }
            log.Debug(success ? "@@@@ UPGRADE OK!!" : "@@@@ UPGRADE FAIL!");

            if (success) await UpdatePlayerData(peer.Actor);

            return success ? SendResult.Ok : SendResult.Failed;
        }

        public async Task<SendResult> HandlePlayerBuildingStatus(IGorMmoPeer peer, OperationRequest operationRequest)
        {
            var operation = new PlayerBuildingBuildingStatusRequest(peer.Protocol, operationRequest);
            if (!operation.IsValid) return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: operation.GetErrorMessage());

            var playerStructData = await GameService.BPlayerStructureManager.CheckBuildingStatus(peer.Actor.PlayerId, (StructureType)operation.StructureType);

            string msg = null;
            StructureDetails building = null;
            if (!playerStructData.IsSuccess)
            {
                msg = playerStructData.Message;
            }
            else if (!playerStructData.HasData)
            {
                msg = "player data not found";
            }
            else
            {
                building = playerStructData.Data.Value.Find(x => (x.Location == operation.StructureLocationId));
                if (building == null) msg = "structure not found";
            }

            if (msg != null)
            {
                return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: msg);
            }

/*            var bld = peer.Actor.InternalPlayerDataManager.GetPlayerBuilding((StructureType)operation.StructureType, operation.StructureLocationId);
            if ((bld != null) && (bld is ResourceGenerator bldResource) && (bldResource.BoostUp.TimeLeft > 0))
            {
                var boostResponse = new ResourceBoostUpResponse()
                {
                    StructureType = operation.StructureType,
                    LocationId = operation.StructureLocationId,
                    StartTime = bldResource.BoostUp.StartTime.ToString("s") + "Z",
                    Duration = bldResource.BoostUp.Duration,
                    Percentage = bldResource.BoostUp.Percentage
                };

                peer.SendOperation(OperationCode.BoostResourceTime, ReturnCode.OK, boostResponse.GetDictionary(), debuMsg: "OK");
            }*/

            var response = new PlayerBuildingBuildingStatuResponse()
            {
                LocationId = building.Location,
                TimeLeft = (int)building.TimeLeft,
                TotalTime = building.Duration
            };
            log.InfoFormat("Send Building status to Client location {0} buildType {1} Time {2} ",
                response.LocationId, playerStructData.Data.ValueId, building.TimeLeft);

            var dic = response.GetDictionary();

            return peer.SendOperation(operationRequest.OperationCode, ReturnCode.OK, dic, debuMsg: playerStructData.Message);
        }

        private async Task<bool> CompleteCustomTaskQuest(int playerId, CustomTaskType taskType)
        {
            var quests = CacheQuestDataManager.AllQuestRewards
                                .Where(x => (x.Quest.QuestType == QuestType.Custom))?.ToList();
            if (quests == null) return false;

            var questUpdated = false;
            List<PlayerQuestDataTable> allUserQuests = null;
            foreach (var quest in quests)
            {
                QuestCustomData questData = null;
                try
                {
                    questData = JsonConvert.DeserializeObject<QuestCustomData>(quest.Quest.DataString);
                }
                catch { }
                if ((questData == null) || (questData.CustomTaskType != taskType)) continue;

                if (allUserQuests == null)
                {
                    var response = await questManager.GetAllQuestProgress(playerId);
                    if (response.IsSuccess && response.HasData) allUserQuests = response.Data;
                    else break;
                }
                var userQuest = allUserQuests.Find(x => (x.QuestId == quest.Quest.QuestId));
                if ((userQuest == null) || !userQuest.Completed)
                {
                    string initialString = (userQuest == null) ? quest.Quest.DataString : null;
                    var resp = await questManager.UpdateQuestData(playerId, quest.Quest.QuestId, true, initialString);
                    if (resp.IsSuccess) questUpdated = true;
                }
            }

            return questUpdated;
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

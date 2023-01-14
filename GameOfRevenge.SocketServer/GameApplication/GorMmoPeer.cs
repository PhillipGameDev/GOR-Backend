using System;
using System.Collections.Generic;
using PhotonHostRuntimeInterfaces;
using ExitGames.Logging;
using GameOfRevenge.GameHandlers;
using GameOfRevenge.Helpers;
using GameOfRevenge.Interface;
using Photon.SocketServer;
using GameOfRevenge.Model;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Quest;
using GameOfRevenge.Common.Services;

namespace GameOfRevenge.GameApplication
{
    public class GorMmoPeer : ClientPeer, IGorMmoPeer, IDisposable
    {
        public static readonly List<IGorMmoPeer> Clients = new List<IGorMmoPeer>();
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        public MmoActor Actor { get; set; }

        public GorMmoPeer(InitRequest initRequest) : base(initRequest)
        {
            Clients.Add(this);
            log.Debug($"Client connected from: {RemoteIP}, with connection state: {ConnectionState}");
        }

        public void OnQuestUpdate(UserQuestProgressData obj)
        {
            QuestUpdateResponse objResp = null;

            if (obj != null)
            {
                objResp = new QuestUpdateResponse()
                {
                    Completed = obj.Completed,
                    InitialData = obj.InitialData,
                    IsSuccess = true,
                    Message = "Update quest data",
                    MilestoneId = obj.MilestoneId,
                    //                Name = obj.Name,
                    ProgressData = obj.ProgressData,
                    QuestId = obj.QuestId,
                    QuestType = obj.QuestType
                };
            }

            Actor.SendEvent(EventCode.UpdateQuest, objResp);
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            if (Actor != null)
            {
                try
                {
                    GameService.RealTimeUpdateManagerQuestValidator.DeletePlayerQuestData(Actor.PlayerId);
                }
                catch { }
            }
            Clients.Remove(this);
            log.Debug($"Client disconnected with code: {reasonCode}, reason in details: {reasonDetail}");
            if (Actor != null) Actor.StopOnReal();
            Dispose();
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            var task = GameService.GameLobby.OnLobbyMessageRecived(this, operationRequest, sendParameters);
            task.Wait();
        }

        public SendResult SendOperation(OperationCode opCode, ReturnCode returnCode) => SendOperation((byte)opCode, returnCode, null, string.Empty);
        public SendResult SendOperation(OperationCode opCode, ReturnCode returnCode, string debuMsg) => SendOperation((byte)opCode, returnCode, null, debuMsg);
        public SendResult SendOperation(OperationCode opCode, ReturnCode returnCode, Dictionary<byte, object> data) => SendOperation((byte)opCode, returnCode, data, string.Empty);
        public SendResult SendOperation(OperationCode opCode, ReturnCode returnCode, Dictionary<byte, object> data, string debuMsg) => SendOperation((byte)opCode, returnCode, data, debuMsg);
        public SendResult SendOperation(byte opCode, ReturnCode returnCode) => SendOperation(opCode, returnCode, null, string.Empty);
        public SendResult SendOperation(byte opCode, ReturnCode returnCode, string debuMsg) => SendOperation(opCode, returnCode, null, debuMsg);
        public SendResult SendOperation(byte opCode, ReturnCode returnCode, Dictionary<byte, object> data) => SendOperation(opCode, returnCode, data, string.Empty);
        public SendResult SendOperation(byte opCode, ReturnCode returnCode, Dictionary<byte, object> data, string debuMsg)
        {
            if (string.IsNullOrWhiteSpace(debuMsg)) debuMsg = string.Empty;

            var operationResponse = new OperationResponse(opCode)
            {
                ReturnCode = (short)returnCode,
                Parameters = data,
                DebugMessage = debuMsg
            };

            var result = SendOperationResponse(operationResponse, new SendParameters());

            log.Debug($"Send Operation response to ply'{this.Actor.PlayerId}' - opCode: {opCode}, returnCode: {returnCode}, data: {GlobalHelper.DicToString(data)}, debugMsg: {debuMsg}");

            return result;
        }

        public SendResult Broadcast(OperationCode opCode, ReturnCode returnCode) => Broadcast((byte)opCode, returnCode, null, string.Empty);
        public SendResult Broadcast(OperationCode opCode, ReturnCode returnCode, string debuMsg) => Broadcast((byte)opCode, returnCode, null, debuMsg);
        public SendResult Broadcast(OperationCode opCode, ReturnCode returnCode, Dictionary<byte, object> data) => Broadcast((byte)opCode, returnCode, data, string.Empty);
        public SendResult Broadcast(OperationCode opCode, ReturnCode returnCode, Dictionary<byte, object> data, string debuMsg) => Broadcast((byte)opCode, returnCode, data, debuMsg);
        public SendResult Broadcast(byte opCode, ReturnCode returnCode) => Broadcast(opCode, returnCode, null, string.Empty);
        public SendResult Broadcast(byte opCode, ReturnCode returnCode, string debuMsg) => Broadcast(opCode, returnCode, null, debuMsg);
        public SendResult Broadcast(byte opCode, ReturnCode returnCode, Dictionary<byte, object> data) => Broadcast(opCode, returnCode, data, string.Empty);
        public SendResult Broadcast(byte opCode, ReturnCode returnCode, Dictionary<byte, object> data, string debuMsg)
        {
            log.Debug(">>>>>BROADCAST "+data);

            SendResult result = SendResult.Ok;

            if (string.IsNullOrWhiteSpace(debuMsg)) debuMsg = string.Empty;

//            log.Debug("CLIENTS = " + Clients);
//            log.Debug("CLIENTS count = " + Clients.Count);
            foreach (var client in Clients)
            {
//                if (client == this || client == null) continue;
                if (client == null) continue;

//                log.Debug("client = " + client+"  oc="+opCode+"  rc="+returnCode);
                try
                {
                    client.SendOperation(opCode, returnCode, data);
                }
                catch (Exception ex)
                {
                    log.Debug("Exception = " + ex.Message);
                }
            }

            log.Debug($"Send Operation to all clients opCode: {opCode}, returnCode: {returnCode}, data: {GlobalHelper.DicToString(data)}, debugMsg: {debuMsg}");

            return result;
        }


        public new SendResult SendEvent(IEventData eventData, SendParameters sendParameters)
        {
            return base.SendEvent(eventData, sendParameters);
        }
    }
}
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ExitGames.Concurrency.Fibers;
using ExitGames.Logging;
using Photon.SocketServer;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Model;
using GameOfRevenge.Interface;
using GameOfRevenge.GameApplication;

namespace GameOfRevenge.GameHandlers
{
    public abstract class UserProfile 
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public ConcurrentDictionary<int, PlayerInstance> InterestUsers { get; protected set; } = new ConcurrentDictionary<int, PlayerInstance>();
        public List<IGorMmoPeer> IntrestedPeers
        {
            get
            {
                return InterestUsers.Where(d => d.Value.Peer != null && d.Value.IsInKingdomView).Select(f => f.Value.Peer).ToList();
            }
        }
        public int PlayerId { get; private set; }
        public PlayerInfo PlayerInfo { get; protected set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public IGorMmoPeer Peer { get; set; }
        public IFiber Fiber { get; set; }
//        public double Health { get; protected set; }
        public bool IsInKingdomView { get; protected set; }

        public UserProfile(int playerId, PlayerInfo playerInfo)
        {
            PlayerId = playerId;
            PlayerInfo = playerInfo;
        }

        public void SendOperation(OperationCode opCode, ReturnCode returnCode, Dictionary<byte, object> data = null, string debuMsg = null)
        {
            if (Peer != null) Peer.SendOperation(opCode, returnCode, data, debuMsg);
        }

        public void SendOperation(byte opCode, ReturnCode returnCode, Dictionary<byte, object> data = null, string debuMsg = null)
        {
            if (Peer != null) Peer.SendOperation(opCode, returnCode, data, debuMsg);
        }

        public void SendEvent(byte evCode, object data)
        {
            SendEvent((EventCode)evCode, data);
        }

        public void SendEvent(EventCode eventCode, object data)
        {
            if (Peer == null) return;

            switch (eventCode)
            {
                case EventCode.IaEnter:
                case EventCode.IaExit:
                case EventCode.EntityExit:
                    break;
                default:
                    var json = JsonConvert.SerializeObject(data);
                    if ((eventCode != EventCode.EntityEnter) || (json.IndexOf("EntityType\":3") != -1))
                    {
                        log.Info($"Send Event to User: {PlayerId} EventCode: {eventCode} Data: {json}");
                    }
                    break;
            }
            Peer.SendEvent(new EventData((byte)eventCode, data));
        }

//        public void BroadcastWithMe(EventCode eventCode, object data) => BroadcastToAllUsers(eventCode, data, true);

/*        public void BroadcastToInterestUsers(EventCode eventCode, object data, bool includeOwner = false)
        {
            BroadcastToUsers(IntrestedPeers, eventCode, data, includeOwner);
        }*/

        public void BroadcastEventToAllUsers(EventCode eventCode, object data, bool includeOwner = false)
        {
            var peers = GorMmoPeer.Clients.Where(x => (x.PlayerInstance != null) &&
                                                (x.PlayerInstance.IsInKingdomView) &&
                                                (x.PlayerInstance.Peer != null)).ToList();
            SendEventToUsers(peers, eventCode, data, includeOwner);
        }

        public void SendEventToUsers(List<int> users, EventCode eventCode, object data, bool includeOwner = false)
        {
            var peers = new List<IGorMmoPeer>();
            foreach (var id in users)
            {
                var client = GorMmoPeer.Clients.Find(x => (x.PlayerInstance != null) &&
                                                        (x.PlayerInstance.IsInKingdomView) &&
                                                        (x.PlayerInstance.PlayerId == id));
                if ((client != null) && (client.PlayerInstance.Peer != null))
                {
                    peers.Add(client.PlayerInstance.Peer);
                }
            }
            SendEventToUsers(peers, eventCode, data, includeOwner);
        }

        void SendEventToUsers(List<IGorMmoPeer> peers, EventCode eventCode, object data, bool includeOwner = false)
        {
            var eventData = new EventData((byte)eventCode, data);
            if (includeOwner && (Peer != null) && !peers.Exists(x => x == Peer)) Peer.SendEvent(eventData);
            var ids = includeOwner ? PlayerId.ToString() : "";
            foreach (var item in peers)
            {
                if (item != null)
                {
                    item.SendEvent(eventData);
                    ids += "," + item.PlayerInstance.PlayerId;
                }
            }
            log.Info($"Send - Event: {eventCode} Data: {JsonConvert.SerializeObject(data)}");
            log.Info("        To Users: " + ids);
        }
    }
}

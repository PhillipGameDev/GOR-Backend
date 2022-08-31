using ExitGames.Concurrency.Fibers;
using ExitGames.Logging;
using GameOfRevenge.Common;
using GameOfRevenge.GameApplication;
using GameOfRevenge.Interface;
using GameOfRevenge.Model;
using Newtonsoft.Json;
using Photon.SocketServer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfRevenge.GameHandlers
{
    public abstract class UserProfile 
    {
        public UserProfile(string playerId)
        {
            UserName = playerId;
            UserId = playerId;
        }

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        public ConcurrentDictionary<string, MmoActor> InterestUsers { get; protected set; } = new ConcurrentDictionary<string, MmoActor>();
        public List<IGorMmoPeer> IntrestedPeers { get { return InterestUsers.Where(d => d.Value.Peer != null && d.Value.IsInKingdomView).Select(f => f.Value.Peer).ToList(); } }
        public string UserName { get; private set; }
        public string UserId { get; private set; }
        public int PlayerId { get { return Convert.ToInt32(this.UserId); } }
        public string Email { get; set; }
        public string FullName { get; set; }
        public IGorMmoPeer Peer { get; set; }
        public IFiber Fiber { get; set; }
        public double Health { get; protected set; }
        public bool IsInKingdomView { get; protected set; }

     
        public void SendOperation(byte opCode, ReturnCode returnCode, Dictionary<byte, object> data = null, string debuMsg = "")
        {
            if (Peer != null)
                Peer.SendOperation(opCode, returnCode, data, debuMsg);
        }
        public void SendOperation(OperationCode opCode, ReturnCode returnCode, Dictionary<byte, object> data = null, string debuMsg = "")
        {
            if (Peer != null)
                Peer.SendOperation(opCode, returnCode, data, debuMsg);
        }
        public void SendEvent(byte evCode, object data)
        {
            SendEvent((EventCode)evCode, data);
        }
        public void SendEvent(EventCode evCode, object data)
        {
            if (Peer != null)
            {
                log.Info($"Send Event to User: {PlayerId}; EvCode: {evCode}; Data: {JsonConvert.SerializeObject(data)};");
                var ev = new EventData((byte)evCode, data);
                Peer.SendEvent(ev, new SendParameters());
            }
        }
        public void BoradCastInterestUsers(EventCode evCode, object data)
        {
            var ev = new EventData((byte)evCode, data);
            var peers = IntrestedPeers;
            foreach (var item in peers)
            {
                log.Info($"Send Event to User: {item.Actor.PlayerId}; EvCode: {evCode}; Data: {JsonConvert.SerializeObject(data)};");
                item.SendEvent(ev, new SendParameters());
            }
        }
        public void BroadcastWithMe(EventCode evCode, object data)
        {
            log.Info($"Broadcast Event with me: {PlayerId}; EvCode: {evCode}; Data: {JsonConvert.SerializeObject(data)};");
            var peers = IntrestedPeers;
            var ev = new EventData((byte)evCode, data);
            foreach (var item in peers)
                item.SendEvent(ev, new SendParameters());
            if (Peer != null)
                Peer.SendEvent(ev, new SendParameters());
        }
    }
}

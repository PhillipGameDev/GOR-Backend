// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Region.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Represents a region used for region-based interest management.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace GameOfRevenge.GameHandlers
{
    using ExitGames.Logging;
    using GameOfRevenge.GameApplication;
    using GameOfRevenge.Helpers;
    using GameOfRevenge.Model;
    using Photon.SocketServer;
    using Photon.SocketServer.Concurrency;

    public class Region
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        public int X { get; private set; }
        // grid cell Y (debug only)
        public int Y { get; private set; }
        public bool IsWalkable { get; private set; } = true;  // that means, this full tiles is non walkable
//        public Vector Area { get; private set; } //tiles area
        public bool IsBooked => (Owner != null);
        public PlayerInstance Owner { get; set; }
        public int WorldId => World.WorldId;//{ get; private set; }

        public readonly WorldGrid World;
        public MessageChannel<RequestSendMeassageChannel> regionSendMessageChannel;

        public MessageChannel<RequestItemEnterMessage> RegionEnterChannel;
        public MessageChannel<RequestItemEnterMessage> JoinKingdomChannel;
        public MessageChannel<RequestItemExitMessage> RegionExitChannel;
        public MessageChannel<RequestEnterCurrentRegionChannel> RegionCurrentEnterChannel;
        public MessageChannel<RequestExitCurrentRegionChannel> RegionCurrentExitChannel;

        public Region(int x, int y, WorldGrid worldGrid)
        {
            regionSendMessageChannel = new MessageChannel<RequestSendMeassageChannel>(MessageCounters.CounterSend);
            JoinKingdomChannel = new MessageChannel<RequestItemEnterMessage>(MessageCounters.CounterSend);
            RegionEnterChannel = new MessageChannel<RequestItemEnterMessage>(MessageCounters.CounterSend);
            RegionExitChannel = new MessageChannel<RequestItemExitMessage>(MessageCounters.CounterSend);
            RegionCurrentEnterChannel = new MessageChannel<RequestEnterCurrentRegionChannel>(MessageCounters.CounterSend);
            RegionCurrentExitChannel = new MessageChannel<RequestExitCurrentRegionChannel>(MessageCounters.CounterSend);
            X = x;
            Y = y;
            World = worldGrid;
//            Area = area;
//            WorldId = worldGrid.worldId;
        }

        public void SetPlayerInRegion(PlayerInstance actor)
        {
            if (!IsBooked) Owner = actor;
        }

        public void RemovePlayerFromRegion(PlayerInstance actor)
        {
            Owner = null;
        }

        public void OnEnterPlayer(PlayerInstance actor)
        {
            if (!IsBooked || (actor == Owner)) return;

            Owner.SendEvent(EventCode.IaEnter, new IaEnterResponse(actor));

            var attackList = GameService.BRealTimeUpdateManager.GetAllAttackerData(actor.PlayerId);
            foreach (var item in attackList)
            {
                var attkResp = new AttackResponse(item.AttackData);
                var attacker = World.WorldPlayers.Find(x => (x.PlayerId == item.AttackData.AttackerId));
                if (attacker != null)
                {
                    attkResp.X = attacker.X;
                    attkResp.Y = attacker.Y;
                }
                Owner.SendEvent(EventCode.AttackEvent, attkResp);
            }
        }

        public void OnExitPlayer(PlayerInstance actor)
        {
            if (!IsBooked || (actor == Owner) || !Owner.IsInKingdomView) return;

            Owner.SendEvent(EventCode.IaExit, new IaExitResponse(actor.PlayerId));
        }
    }

    public class RequestItemEnterMessage
    {
        public PlayerInterestArea InterestArea { get; private set; }

        public RequestItemEnterMessage(PlayerInterestArea interestArea)
        {
            InterestArea = interestArea;
        }
    };

    public class RequestSendMeassageChannel
    {
        public EventData Evdata { get; private set; }

        public RequestSendMeassageChannel(EventData evData)
        {
            Evdata = evData;
        }
    };

    /// <summary>
    /// Interest area requests all items in exited region.
    /// </summary>
    public class RequestItemExitMessage
    {
        public PlayerInterestArea InterestArea { get; private set; }

        public RequestItemExitMessage(PlayerInterestArea interestArea)
        {
            InterestArea = interestArea;
        }
    }

    public class ItemRegionChangedMessage
    {
        public Region Region0 { get; private set; }
        public Region Region1 { get; private set; }

        public ItemRegionChangedMessage(Region r0, Region r1)
        {
            Region0 = r0;
            Region1 = r1;
        }
    };

    public class RequestEnterCurrentRegionChannel
    {
        public PlayerInterestArea InterestArea;

        public RequestEnterCurrentRegionChannel(PlayerInterestArea interestArea)
        {
            InterestArea = interestArea;
        }
    }

    public class RequestExitCurrentRegionChannel
    {
        public PlayerInterestArea InterestArea;

        public RequestExitCurrentRegionChannel(PlayerInterestArea interestArea)
        {
            InterestArea = interestArea;
        }
    }
}
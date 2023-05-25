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
        public Vector Area { get; private set; } //tiles area
        public bool IsBooked => (Owner != null);
        public MmoActor Owner { get; set; }
        public int CountryId { get; private set; }

        public readonly CountryGrid Country;
        public MessageChannel<RequestSendMeassageChannel> regionSendMessageChannel;

        public MessageChannel<RequestItemEnterMessage> RegionEnterChanel;
        public MessageChannel<RequestItemEnterMessage> JoinKingdomChannel;
        public MessageChannel<RequestItemExitMessage> RegionExitChannel;
        public MessageChannel<RequestEnterCurrentRegionChannel> RegionCurrentEnterChannel;
        public MessageChannel<RequestExitCurrentRegionChannel> RegionCurrentExitChannel;

        public Region(int x, int y, Vector area, int worldId, CountryGrid country)
        {
            regionSendMessageChannel = new MessageChannel<RequestSendMeassageChannel>(MessageCounters.CounterSend);
            JoinKingdomChannel = new MessageChannel<RequestItemEnterMessage>(MessageCounters.CounterSend);
            RegionEnterChanel = new MessageChannel<RequestItemEnterMessage>(MessageCounters.CounterSend);
            RegionExitChannel = new MessageChannel<RequestItemExitMessage>(MessageCounters.CounterSend);
            RegionCurrentEnterChannel = new MessageChannel<RequestEnterCurrentRegionChannel>(MessageCounters.CounterSend);
            RegionCurrentExitChannel = new MessageChannel<RequestExitCurrentRegionChannel>(MessageCounters.CounterSend);
            X = x;
            Y = y;
            Country = country;
            Area = area;
            CountryId = worldId;
        }

        public void SetPlayerInRegion(MmoActor actor)
        {
            if (!IsBooked) Owner = actor;
        }

        public void RemovePlayerFromRegion(MmoActor actor)
        {
            Owner = null;
        }

        public void OnEnterPlayer(MmoActor actor)
        {
            if (IsBooked && (this.Owner != actor))
            {
                var response = new IaEnterResponse(actor);

                Owner.SendEvent(EventCode.IaEnter, response);
                var attackData = GameService.BRealTimeUpdateManager.GetAttackerData(actor.PlayerId);
                if (attackData != null)
                {
                    var attackResponse = new AttackResponse(attackData.AttackData);
                    Owner.SendEvent(EventCode.AttackEvent, attackResponse);
                }
            }
        }

        public void OnExitPlayer(MmoActor actor)
        {
            if (IsBooked && (Owner != actor) && Owner.IsInKingdomView)
            {
                var response = new IaExitResponse
                {
                    playerId = actor.PlayerId
                };
                Owner.SendEvent(EventCode.IaExit, response);
            }
        }
    }

    public class RequestItemEnterMessage
    {
        public InterestArea InterestArea { get; private set; }

        public RequestItemEnterMessage(InterestArea interestArea)
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
        public InterestArea InterestArea { get; private set; }

        public RequestItemExitMessage(InterestArea interestArea)
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
        public InterestArea InterestArea;

        public RequestEnterCurrentRegionChannel(InterestArea interestArea)
        {
            InterestArea = interestArea;
        }
    }

    public class RequestExitCurrentRegionChannel
    {
        public InterestArea InterestArea;

        public RequestExitCurrentRegionChannel(InterestArea interestArea)
        {
            InterestArea = interestArea;
        }
    }
}
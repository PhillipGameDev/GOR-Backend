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
        public bool IsBooked { get; set; }
        public MmoActor Owner { get; set; }
        public int CountryId { get; private set; }

        public readonly CountryGrid country;
        public MessageChannel<RequestSendMeassageChannel> regionSendMessageChannel;

        public MessageChannel<RequestItemEnterMessage> RegionEnterChanel;
        public MessageChannel<RequestItemEnterMessage> JoinKingdomChannel;
        public MessageChannel<RequestItemExitMessage> RegionExitChannel;
        public MessageChannel<RequestEnterCurrentRegionChannel> RegionCurrentEnterChannel;
        public MessageChannel<RequestExitCurrentRegionChannel> RegionCurrentExitChannel;

        public Region(int x, int y, Vector area, int worldId, CountryGrid country)
        {
            this.regionSendMessageChannel = new MessageChannel<RequestSendMeassageChannel>(MessageCounters.CounterSend);
            this.JoinKingdomChannel = new MessageChannel<RequestItemEnterMessage>(MessageCounters.CounterSend);
            this.RegionEnterChanel = new MessageChannel<RequestItemEnterMessage>(MessageCounters.CounterSend);
            this.RegionExitChannel = new MessageChannel<RequestItemExitMessage>(MessageCounters.CounterSend);
            this.RegionCurrentEnterChannel = new MessageChannel<RequestEnterCurrentRegionChannel>(MessageCounters.CounterSend);
            this.RegionCurrentExitChannel = new MessageChannel<RequestExitCurrentRegionChannel>(MessageCounters.CounterSend);
            this.X = x;
            this.Y = y;
            this.country = country;
            this.Area = area;
            this.CountryId = worldId;
        }

        public void SpawnPlayerInRegion(MmoActor actor)
        {
            if (!this.IsBooked)
            {
                this.IsBooked = true;
                this.Owner = actor;
            }
        }

        public void RemovePlayerFromRegion(MmoActor actor)
        {
            this.IsBooked = false;
            this.Owner = null;
        }

        public void OnEnterPlayer(MmoActor actor)
        {
            if (this.IsBooked && (this.Owner != null) && (this.Owner != actor))
            {
                var response = new IaEnterResponse(actor);

                this.Owner.SendEvent(EventCode.IaEnter, response);
                var attackData = GameService.BRealTimeUpdateManager.GetAttackerData(actor.PlayerId);
                if (attackData != null)
                {
                    var attackResponse = new AttackResponse(attackData.AttackData);
                    this.Owner.SendEvent(EventCode.AttackResponse, attackResponse);
                }
            }
        }

        public void OnExitPlayer(MmoActor actor)
        {
            if (this.IsBooked && (this.Owner != null) && (this.Owner != actor) && this.Owner.IsInKingdomView)
            {
                var response = new IaExitResponse
                {
                    playerId = actor.PlayerId
                };
                this.Owner.SendEvent(EventCode.IaExit, response);
            }
        }
    }

    public class RequestItemEnterMessage
    {
        public RequestItemEnterMessage(InterestArea interestArea)
        {
            this.InterestArea = interestArea;
        }
        public InterestArea InterestArea { get; private set; }
    };
    public class RequestSendMeassageChannel
    {
        public RequestSendMeassageChannel(EventData evData)
        {
            this.Evdata = Evdata;
        }
        public EventData Evdata { get; private set; }
    };

    /// <summary>
    /// Interest area requests all items in exited region.
    /// </summary>
    public class RequestItemExitMessage
    {
        public RequestItemExitMessage(InterestArea interestArea)
        {
            this.InterestArea = interestArea;
        }
        public InterestArea InterestArea { get; private set; }
    }
    public class ItemRegionChangedMessage
    {
        public ItemRegionChangedMessage(Region r0, Region r1)
        {
            this.Region0 = r0;
            this.Region1 = r1;

        }
        public Region Region0 { get; private set; }
        public Region Region1 { get; private set; }
    };
    public class RequestEnterCurrentRegionChannel
    {
        public InterestArea IA;
        public RequestEnterCurrentRegionChannel(InterestArea IA)
        {
            this.IA = IA;

        }
    }
    public class RequestExitCurrentRegionChannel
    {
        public InterestArea IA;
        public RequestExitCurrentRegionChannel(InterestArea IA)
        {
            this.IA = IA;
        }
    }


}
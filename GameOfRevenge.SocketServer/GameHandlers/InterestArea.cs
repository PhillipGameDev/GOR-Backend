using ExitGames.Concurrency.Fibers;
using ExitGames.Logging;
using GameOfRevenge.Common;
using GameOfRevenge.GameApplication;
using GameOfRevenge.Interface;
using GameOfRevenge.Model;
using Photon.SocketServer.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfRevenge.GameHandlers
{
    public class InterestArea : IInterestArea, IDisposable
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        public IWorld World { get { return this.CastleRegion.country; } }
        public MmoActor Owner { get; private set; }
        public Region CastleRegion { get; private set; }
        public Region CameraRegion { get; private set; }
        public readonly HashSet<Region> regions;

        private readonly RequestItemEnterMessage requestItemEnterMessage;
        private readonly RequestItemExitMessage requestItemExitMessage;

        private readonly Dictionary<Region, IDisposable> regionSubscriptions;
        public IFiber Fiber { get { return this.Owner.Fiber; } }
        public InterestArea(Region region, MmoActor actor, bool isLocatedNewLocation = false)
        {
            this.CastleRegion = region;
            this.CameraRegion = region;
            this.Owner = actor;
            this.regions = new HashSet<Region>();
            this.regionSubscriptions = new Dictionary<Region, IDisposable>();
            this.requestItemEnterMessage = new RequestItemEnterMessage(this);
            this.requestItemExitMessage = new RequestItemExitMessage(this);
            this.AddUpdateIntArea(isLocatedNewLocation);
        }
        public void AddUpdateIntArea(bool isLocatedNewLocation = false)
        {
            List<Region> regions = new List<Region>();
            var maxPosX = this.World.Area.Size.X / this.World.TileDimensions.X;   //max position of X cordinate in our world
            var maxPosY = this.World.Area.Size.Y / this.World.TileDimensions.Y;   //max position of Y cordinate in our world
            var ecXPos = this.CameraRegion.X + (1 * (GlobalConst.TilesIaX));
            var ecYPos = this.CameraRegion.Y + (1 * (GlobalConst.TilesIaY));
            var scXPos = this.CameraRegion.X + (-1 * (GlobalConst.TilesIaX));
            var scYPos = this.CameraRegion.Y + (-1 * (GlobalConst.TilesIaY));
            var eC = new Vector(Math.Min(maxPosX - 1, ecXPos), Math.Min(maxPosX - 1, ecYPos));    // last cordinate position of tiles which is part of IA
            var sC = new Vector(Math.Max(0, scXPos), Math.Max(0, scYPos));   //start cordinate position of tiles which is part of IA
            for (int i = (int)sC.X; i <= eC.X && i >= 0 && i < maxPosX; i++)   //loop for x cordinate
            {
                for (int j = (int)sC.Y; j <= eC.Y && j >= 0 && j < maxPosY; j++)   // loop for y cordinate
                {
                    var r = this.World.WorldRegions[i][j];
                    regions.Add(r);
                    if (!this.regions.Contains(r))
                    {
                        this.regions.Add(r);
                        if (r.IsBooked && r.Owner != null && !this.Owner.InterestUsers.ContainsKey(r.Owner.UserId))
                        {
                            this.Owner.InterestUsers.TryAdd(r.Owner.UserId, r.Owner);
                            if (isLocatedNewLocation)
                                r.OnEnterPlayer(this.Owner); // new player instanstiate
                            if (this.Owner.IsInKingdomView)
                                this.SendOnEnterEvent(r);
                        }
                    }
                }
            }
            if (regions != null && regions.Count() > 0 && this.regions != null && this.regions.Count() > 0)
            {
                List<Region> extraRegion = this.regions.Except(regions).ToList();
                if (extraRegion != null && extraRegion.Count() > 0)
                {
                    foreach (Region r in extraRegion)
                    {
                        if (this.regions.Contains(r))
                        {
                            this.regions.Remove(r);
                            if (r.IsBooked && r.Owner != null && this.Owner.InterestUsers.ContainsKey(r.Owner.UserId))
                            {
                                log.InfoFormat("Send Exit Tiles to ");
                                MmoActor o;
                                this.Owner.InterestUsers.TryRemove(r.Owner.UserId, out o);
                                if (isLocatedNewLocation)
                                    r.OnExitPlayer(r.Owner);
                                if (this.Owner.IsInKingdomView)
                                    this.SendOnExitEvent(r);
                            }
                        }
                    }
                }
            }
            log.InfoFormat("Add regions acc current positition Count {0} ", this.regions.Count);
        }
        public void UpdateInterestArea(Region newRegion)
        {
            if (this.CastleRegion == null || this.CastleRegion != newRegion)
            {
                log.InfoFormat("Update Interest Area X {0} y {1} ", newRegion.X, newRegion.Y);
                this.CastleRegion = newRegion;
                this.CameraRegion = newRegion;
                this.AddUpdateIntArea(true);
            }
        }
        public void CameraMove(Region r)
        {
            log.InfoFormat("Camera Move Request {0} ", this.Owner.UserName);
            if (this.CameraRegion == null || this.CameraRegion != r)
            {
                this.CameraRegion = r;
                this.AddUpdateIntArea();
            }
        }
        public void JoinKingdomView()
        {
            if (!this.Owner.IsInKingdomView)
            {
                this.Owner.JoinKingdomView();
                foreach (var r in this.regions)
                    this.SendOnEnterEvent(r);
                var attackData = GameService.BRealTimeUpdateManager.GetAttackerData(this.Owner.PlayerId);
                if (attackData != null)
                {
                    var attackResponse = new AttackResponse(attackData.AttackData);
                    this.Owner.SendEvent(EventCode.AttackResponse, attackResponse);
                }
                else
                    log.InfoFormat("Attack data not found for this user when join kingdom view {0} ", this.Owner.PlayerId);
            }
        }
        public void LeaveKingdomView()
        {
            if (this.Owner.IsInKingdomView)
            {
                this.Owner.LeaveKingdomView();
                foreach (var r in this.regions)
                    this.SendOnExitEvent(r);
            }
        }
        public void SendOnEnterEvent(Region r)
        {
            if (r.IsBooked && r.Owner != null && r.Owner != this.Owner)
            {
                var response = r.GetAttackerIaResponse(r.Owner);
                this.Owner.SendEvent(EventCode.IaEnter, response);
                var attackData = GameService.BRealTimeUpdateManager.GetAttackerData(r.Owner.PlayerId);
                if (attackData != null)
                {
                    var attackResponse = new AttackResponse(attackData.AttackData);
                    this.Owner.SendEvent(EventCode.AttackResponse, attackResponse);
                }
            }
        }
        public void SendOnExitEvent(Region r)
        {
            if (r.IsBooked && r.Owner != null && r.Owner != this.Owner)
            {
                var response = new IaExitResponse
                {
                    UserName = r.Owner.UserName
                };
                this.Owner.SendEvent(EventCode.IaExit, response);
            }
        }
        public void Dispose()
        {
            this.LeaveKingdomView();
            this.regionSubscriptions.Clear();
            GC.SuppressFinalize(this);
        }
    }
}

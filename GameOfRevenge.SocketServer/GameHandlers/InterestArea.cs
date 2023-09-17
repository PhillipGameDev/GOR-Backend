using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Concurrency.Fibers;
using ExitGames.Logging;
using GameOfRevenge.GameApplication;
using GameOfRevenge.Interface;
using GameOfRevenge.Model;

namespace GameOfRevenge.GameHandlers
{
    public class InterestArea : IInterestArea, IDisposable
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        public IWorld World { get { return this.CastleRegion.Country; } }
        public MmoActor Owner { get; private set; }
        public Region CastleRegion { get; private set; }
        public Region CameraRegion { get; private set; }
        public readonly HashSet<Region> Regions;

        private readonly RequestItemEnterMessage requestItemEnterMessage;
        private readonly RequestItemExitMessage requestItemExitMessage;

        private readonly Dictionary<Region, IDisposable> regionSubscriptions;
        public IFiber Fiber { get { return this.Owner.Fiber; } }

        public InterestArea(Region region, MmoActor actor, bool isLocatedNewLocation = false)
        {
            this.CastleRegion = region;
            this.CameraRegion = region;
            this.Owner = actor;
            this.Regions = new HashSet<Region>();
            this.regionSubscriptions = new Dictionary<Region, IDisposable>();
            this.requestItemEnterMessage = new RequestItemEnterMessage(this);
            this.requestItemExitMessage = new RequestItemExitMessage(this);
            this.AddUpdateIntArea(isLocatedNewLocation);
        }

        public void AddUpdateIntArea(bool isLocatedNewLocation = false)
        {
            var regions = new List<Region>();
            var maxPosX = World.SizeX / World.TileDimensions.X;   //max position of X cordinate in our world
            var maxPosY = World.SizeY / World.TileDimensions.Y;   //max position of Y cordinate in our world
            var ecXPos = CameraRegion.X + (1 * (GlobalConst.TilesIaX));
            var ecYPos = CameraRegion.Y + (1 * (GlobalConst.TilesIaY));
            var scXPos = CameraRegion.X + (-1 * (GlobalConst.TilesIaX));
            var scYPos = CameraRegion.Y + (-1 * (GlobalConst.TilesIaY));
            var eC = new Vector(Math.Min(maxPosX - 1, ecXPos), Math.Min(maxPosX - 1, ecYPos));    // last cordinate position of tiles which is part of IA
            var sC = new Vector(Math.Max(0, scXPos), Math.Max(0, scYPos));   //start cordinate position of tiles which is part of IA

            var attackList = GameService.BRealTimeUpdateManager.GetAllAttackerData(this.Owner.PlayerId);
            if ((attackList != null) && (attackList.Count > 0))
            {
                foreach (var item in attackList)
                {
                    var targetId = item.AttackData.TargetId;
                    var found = false;
                    for (int i = 0; i < maxPosX; i++)   //loop for x coordinate
                    {
                        for (int j = 0; j < maxPosY; j++)   // loop for y coordinate
                        {
                            var r = this.World.WorldRegions[i][j];
                            if ((r.Owner == null) || (r.Owner.PlayerId != targetId)) continue;

                            if (!this.Regions.Contains(r))
                            {
                                this.Regions.Add(r);
                                if (r.IsBooked && (r.Owner != null) && !this.Owner.InterestUsers.ContainsKey(r.Owner.PlayerId))
                                {
                                    this.Owner.InterestUsers.TryAdd(r.Owner.PlayerId, r.Owner);
                                    if (isLocatedNewLocation) r.OnEnterPlayer(this.Owner); // new player instanstiate
                                    if (this.Owner.IsInKingdomView) this.SendOnEnterEvent(r);
                                }
                            }
                            found = true;
                            break;
                        }
                        if (found) break;
                    }
                }
            }

            for (int i = (int)sC.X; i <= eC.X && i >= 0 && i < maxPosX; i++)   //loop for x cordinate
            {
                for (int j = (int)sC.Y; j <= eC.Y && j >= 0 && j < maxPosY; j++)   // loop for y cordinate
                {
                    var r = this.World.WorldRegions[i][j];
                    regions.Add(r);
                    if (this.Regions.Contains(r)) continue;

                    this.Regions.Add(r);
                    if (r.IsBooked && (r.Owner != null) && !this.Owner.InterestUsers.ContainsKey(r.Owner.PlayerId))
                    {
                        this.Owner.InterestUsers.TryAdd(r.Owner.PlayerId, r.Owner);
                        if (isLocatedNewLocation) r.OnEnterPlayer(this.Owner); // new player instanstiate
                        if (this.Owner.IsInKingdomView) this.SendOnEnterEvent(r);
                    }
                }
            }

            if (((regions != null) && (regions.Count() > 0)) &&
                ((Regions != null) && (Regions.Count() > 0)))
            {
                var outRegions = Regions.Except(regions).ToList();
                foreach (Region r in outRegions)
                {
                    if (!Regions.Contains(r)) continue;
                    if (r.Owner != null)
                    {
                        var tilePlayerId = r.Owner.PlayerId;
                        if (tilePlayerId == this.Owner.PlayerId) continue;
                        if ((attackList != null) && attackList.Exists(x => (x.AttackData.TargetId == tilePlayerId))) continue;
                    }

                    Regions.Remove(r);
                    if (r.IsBooked && (r.Owner != null) && Owner.InterestUsers.ContainsKey(r.Owner.PlayerId))
                    {
                        log.InfoFormat("Send Exit Tiles to ");
                        this.Owner.InterestUsers.TryRemove(r.Owner.PlayerId, out MmoActor o);
                        if (isLocatedNewLocation) r.OnExitPlayer(r.Owner);
                        if (this.Owner.IsInKingdomView) this.SendOnExitEvent(r);
                    }
                }
            }
            log.InfoFormat("Add regions acc current positition Count {0} ", this.Regions.Count);
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

/*        public void AddUpdateIntArea2()
        {
            List<Region> regions = new List<Region>();
            var maxPosX = this.World.Area.Size.X / this.World.TileDimensions.X;   //max position of X coordinate in our world
            var maxPosY = this.World.Area.Size.Y / this.World.TileDimensions.Y;   //max position of Y coordinate in our world

            for (int i = 0; i < maxPosX; i++)   //loop for x coordinate
            {
                for (int j = 0; j < maxPosY; j++)   // loop for y coordinate
                {
                    log.Info("region " + i + " " + j);
                    var r = this.World.WorldRegions[i][j];
                    regions.Add(r);
                    if (this.Regions.Contains(r)) continue;

                    this.Regions.Add(r);
                    if (r.IsBooked && (r.Owner != null) && !this.Owner.InterestUsers.ContainsKey(r.Owner.PlayerId))
                    {
                        this.Owner.InterestUsers.TryAdd(r.Owner.PlayerId, r.Owner);
                        r.OnEnterPlayer(this.Owner); // new player instantiate
                        if (this.Owner.IsInKingdomView) this.SendOnEnterEvent(r);
                    }
                }
            }
            if (((regions != null) && (regions.Count() > 0)) &&
                ((Regions != null) && (Regions.Count() > 0)))
            {
                var extraRegion = Regions.Except(regions);
                if (extraRegion != null)
                {
                    foreach (Region r in extraRegion)
                    {
                        if (!Regions.Contains(r)) continue;

                        Regions.Remove(r);
                        if (r.IsBooked && (r.Owner != null) &&
                            Owner.InterestUsers.ContainsKey(r.Owner.PlayerId))
                        {
                            log.InfoFormat("Send Exit Tiles to ");
                            this.Owner.InterestUsers.TryRemove(r.Owner.PlayerId, out MmoActor o);
                            r.OnExitPlayer(r.Owner);
                            if (this.Owner.IsInKingdomView) this.SendOnExitEvent(r);
                        }
                    }
                }
            }
            log.InfoFormat("Add regions acc current position Count {0} ", this.Regions.Count);
        }*/

        public void CameraMove(Region r)
        {
//            log.InfoFormat("Camera Move Request {0} ", this.Owner.PlayerId);
            try
            {
                if (this.CameraRegion == null || this.CameraRegion != r)
                {
                    this.CameraRegion = r;
                    this.AddUpdateIntArea();
                }
            }
            catch (Exception ex)
            {
                log.Info("Exception!! " + ex.Message);
            }
        }
        public void JoinKingdomView()
        {
            if (this.Owner.IsInKingdomView) return;

            this.Owner.JoinKingdomView();
            foreach (var r in this.Regions)
            {
/*                if (r != null)
                {
                    try
                    {
                        log.Info("region " + r.X + " " + r.Y + "   " + r.IsBooked + "  " + r.Owner);
                        if (r.Owner != null) log.Info("ply=" + r.Owner.PlayerId);
                    }
                    catch (Exception ex)
                    {
                        log.Info("exception " + ex.Message);
                    }
                }*/
                this.SendOnEnterEvent(r);
            }
            var attackList = GameService.BRealTimeUpdateManager.GetAllAttackerData(this.Owner.PlayerId);
            if ((attackList != null) && (attackList.Count > 0))
            {
                foreach (var item in attackList)
                {
                    var attackResponse = new AttackResponse(item.AttackData);
                    this.Owner.SendEvent(EventCode.AttackEvent, attackResponse);
                }
            }
            else
            {
                log.InfoFormat("Attack data not found for this user when join kingdom view {0} ", this.Owner.PlayerId);
            }
        }
        public void LeaveKingdomView()
        {
            if (!this.Owner.IsInKingdomView) return;

            this.Owner.LeaveKingdomView();
            foreach (var r in this.Regions)
            {
                this.SendOnExitEvent(r);
            }
        }
        public void SendOnEnterEvent(Region region)
        {
            if (region.IsBooked && (region.Owner != null) &&
                (region.Owner != this.Owner) && this.Owner.IsInKingdomView)
            {
                var response = new IaEnterResponse(region.Owner);
                this.Owner.SendEvent(EventCode.IaEnter, response);

/*                var attackData = GameService.BRealTimeUpdateManager.GetAttackerData(region.Owner.PlayerId);
                if (attackData != null)
                {
                    var attackResponse = new AttackResponse(attackData.AttackData);
                    this.Owner.SendEvent(EventCode.AttackEvent, attackResponse);
                }*/
            }
        }
        public void SendOnExitEvent(Region r)
        {
            if (r.IsBooked && (r.Owner != null) && (r.Owner != this.Owner) && this.Owner.IsInKingdomView)
            {
                var response = new IaExitResponse
                {
                    playerId = r.Owner.PlayerId
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

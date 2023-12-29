using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Concurrency.Fibers;
using ExitGames.Logging;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Kingdom;
using GameOfRevenge.GameApplication;
using GameOfRevenge.Interface;
using GameOfRevenge.Model;

namespace GameOfRevenge.GameHandlers
{
    public class PlayerInterestArea : IInterestArea, IDisposable
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public IWorld World => CastleRegion.World;

        public PlayerInstance PlayerInstance { get; private set; }
        public Region CastleRegion { get; private set; }
        public Region CameraRegion { get; private set; }
        public HashSet<Region> AreaRegions = new HashSet<Region>();
        public List<ZoneFortressTable> AreaForts = new List<ZoneFortressTable>();

//        private readonly RequestItemEnterMessage requestItemEnterMessage;
//        private readonly RequestItemExitMessage requestItemExitMessage;

//        private readonly Dictionary<Region, IDisposable> regionSubscriptions;
        public IFiber Fiber => PlayerInstance.Fiber;

        public PlayerInterestArea(Region region, PlayerInstance playerInstance, bool isLocatedNewLocation = false)
        {
            CastleRegion = region;
            CameraRegion = region;
            PlayerInstance = playerInstance;
//            PlayerInterestRegions = new HashSet<Region>();
//            this.regionSubscriptions = new Dictionary<Region, IDisposable>();
//            this.requestItemEnterMessage = new RequestItemEnterMessage(this);
//            this.requestItemExitMessage = new RequestItemExitMessage(this);
//            AddUpdateIntArea(isLocatedNewLocation);
            if (isLocatedNewLocation)
            {
                region.OnEnterPlayer(PlayerInstance);
            }
        }

        public void AddUpdateIntArea()//bool isLocatedNewLocation = false)
        {
            //            log.Info("Dimensions = " + World.TileDimensions.X+" , "+World.TileDimensions.Y);

            var maxPosX = World.TilesX;// / World.TileDimensions.X;   //max position of X cordinate in our world
            var maxPosY = World.TilesY;// / World.TileDimensions.Y;   //max position of Y cordinate in our world
            var ecXPos = CameraRegion.X + GlobalConst.PadTilesX;
            var ecYPos = CameraRegion.Y + GlobalConst.PadTilesY;

            var scXPos = CameraRegion.X - GlobalConst.PadTilesX / 2;
            var scYPos = CameraRegion.Y - GlobalConst.PadTilesY / 2;
            var eC = new Vector(Math.Min(maxPosX - 1, ecXPos), Math.Min(maxPosX - 1, ecYPos));    // last cordinate position of tiles which is part of IA
            var sC = new Vector(Math.Max(0, scXPos), Math.Max(0, scYPos));   //start cordinate position of tiles which is part of IA

            var attackList = AddPlayersConfronted(PlayerInstance.PlayerId);//, isLocatedNewLocation);

            var regionsInArea = new List<Region>();
            for (int x = (int)sC.X; x <= eC.X && x >= 0 && x < maxPosX; x++)   //loop for x cordinate
            {
                for (int y = (int)sC.Y; y <= eC.Y && y >= 0 && y < maxPosY; y++)   // loop for y cordinate
                {
                    var region = World.WorldRegions[x][y];
                    regionsInArea.Add(region);
                    if (AreaRegions.Contains(region)) continue;

                    AreaRegions.Add(region);
                    if (region.IsBooked && (region.Owner.PlayerId != PlayerInstance.PlayerId) &&
                        !PlayerInstance.InterestUsers.ContainsKey(region.Owner.PlayerId))
                    {
                        PlayerInstance.InterestUsers.TryAdd(region.Owner.PlayerId, region.Owner);
                        if (PlayerInstance.IsInKingdomView)
                        {
                            PlayerInstance.SendEvent(EventCode.IaEnter, new IaEnterResponse(region.Owner));
                        }
                    }

                    var monsters = World.WorldMonsters.FindAll(e => e.X == x && e.Y == y);
                    foreach (var monster in monsters)
                    {
                        var monsterId = monster.Id;
                        var seed = monsterId;
                        var enterEvent = new MonsterEnterResponse(x, y, seed, EntityType.Monster, monsterId, monster.Health, monster.Level, monster.MonsterType, monster.Attack, monster.Defense);
                        PlayerInstance.SendEvent(EventCode.EntityEnter, enterEvent);
                    }
                }
            }

            if ((regionsInArea.Count > 0) && (AreaRegions.Count > 0))
            {
                var outRegions = AreaRegions.Except(regionsInArea).ToList();
                foreach (Region region in outRegions)
                {
                    if (!AreaRegions.Contains(region)) continue;
                    if (region.Owner != null)
                    {
                        var tilePlayerId = region.Owner.PlayerId;
                        if (tilePlayerId == PlayerInstance.PlayerId) continue;
                        if (attackList.Exists(x => (x.AttackData.TargetId == tilePlayerId))) continue;

                        if (region.IsBooked && PlayerInstance.InterestUsers.ContainsKey(tilePlayerId))
                        {
//                            log.InfoFormat("Send Exit Tiles to ");
                            PlayerInstance.InterestUsers.TryRemove(tilePlayerId, out PlayerInstance o);
//                            if (isLocatedNewLocation) region.OnExitPlayer(region.Owner);
                            if (PlayerInstance.IsInKingdomView) SendOnExitEvent(region);
                        }
                    }

                    var monsters = World.WorldMonsters.FindAll(e => e.X == region.X && e.Y == region.Y);
                    foreach (var monster in monsters)
                    {
                        var exitEvent = new EntityExitResponse((byte)EntityType.Monster, monster.Id);
                        PlayerInstance.SendEvent(EventCode.EntityExit, exitEvent);
                    }

                    AreaRegions.Remove(region);
                }
            }
            log.InfoFormat("Add regions acc current positition Count {0} ", this.AreaRegions.Count);

            if (PlayerInstance.IsInKingdomView)
            {
                var totalZonesX = (World.TilesX / World.ZoneSize);
                var centerZone = (World.ZoneSize / 2);
    //            var fortsInArea = new List<ZoneFortress>();
                foreach (var fortress in World.WorldForts)
                {
                    if (AreaForts.Contains(fortress)) continue;

                    AreaForts.Add(fortress);
                    var x = (fortress.ZoneIndex % totalZonesX) * World.ZoneSize + centerZone;
                    var y = (fortress.ZoneIndex / totalZonesX) * World.ZoneSize + centerZone;
                    var enterEvent = new FortressEnterResponse(x, y, 0, EntityType.Fortress, fortress.ZoneFortressId, fortress.HitPoints, fortress.Attack, fortress.Defense);
                    enterEvent.ClanId = fortress.ClanId;
                    enterEvent.Name = fortress.Name;
//                    enterEvent.PlayerId = fortress.PlayerId;
    //                enterEvent.PlayerTroops
                    PlayerInstance.SendEvent(EventCode.EntityEnter, enterEvent);
                }
            }
        }

        List<AttackStatusData> AddPlayersConfronted(int playerId)//, bool isLocatedNewLocation)
        {
            var maxPosX = World.TilesX;// / World.TileDimensions.X;   //max position of X cordinate in our world
            var maxPosY = World.TilesY;// / World.TileDimensions.Y;   //max position of Y cordinate in our world
            var attackList = GameService.BRealTimeUpdateManager.GetAllAttackerData(playerId);
            //TODO: invert for
            foreach (var item in attackList)
            {
                var targetId = item.AttackData.TargetId;
                var found = false;
                for (int x = 0; x < maxPosX; x++)   //loop for x coordinate
                {
                    for (int y = 0; y < maxPosY; y++)   // loop for y coordinate
                    {
                        var region = World.WorldRegions[x][y];
                        if ((region.Owner == null) || (region.Owner.PlayerId != targetId)) continue;

                        if (!AreaRegions.Contains(region))
                        {
                            AreaRegions.Add(region);
                            if (region.IsBooked && !PlayerInstance.InterestUsers.ContainsKey(region.Owner.PlayerId))
                            {
                                PlayerInstance.InterestUsers.TryAdd(region.Owner.PlayerId, region.Owner);
//                                if (isLocatedNewLocation) region.OnEnterPlayer(PlayerInstance); // new player instantiate
                                if (PlayerInstance.IsInKingdomView)
                                {
                                    PlayerInstance.SendEvent(EventCode.IaEnter, new IaEnterResponse(region.Owner));
//                                    SendOnEnterEvent(region);
                                }
                            }
                        }
                        found = true;
                        break;
                    }
                    if (found) break;
                }
            }

            return attackList;
        }

        public void UpdateInterestArea(Region newRegion)
        {
            if (newRegion == CastleRegion) return;

            log.InfoFormat("Update Interest Area X {0} y {1} ", newRegion.X, newRegion.Y);
            CastleRegion = newRegion;
            CameraRegion = newRegion;
            AddUpdateIntArea();// true);
        }

        public void CameraMove(Region region)
        {
//            log.InfoFormat("Camera Move Request {0} ", this.Owner.PlayerId);
            if (region == CameraRegion) return;

            CameraRegion = region;
            try
            {
                AddUpdateIntArea();
            }
            catch (Exception ex)
            {
                log.Info("Exception!! " + ex.Message);
            }
        }

        public void JoinKingdomView()
        {
            if (PlayerInstance.IsInKingdomView) return;

            PlayerInstance.JoinKingdomView();
            AddUpdateIntArea();

/*            foreach (var region in AreaRegions)
            {
                if (region.IsBooked && (region.Owner != PlayerInstance))
                {
                    PlayerInstance.SendEvent(EventCode.IaEnter, new IaEnterResponse(region.Owner));
                }
//                    SendOnEnterEvent(region);
            }*/

            var attackList = GameService.BRealTimeUpdateManager.GetAllAttackerData(PlayerInstance.PlayerId);
            if (attackList.Count > 0)
            {
                log.Info(attackList.Count + "Attack data found for user " + PlayerInstance.PlayerId);
                foreach (var item in attackList)
                {
                    PlayerInstance.SendEvent(EventCode.AttackEvent, new AttackResponse(item.AttackData));
                }
            }
        }

        public void LeaveKingdomView()
        {
            if (!PlayerInstance.IsInKingdomView) return;

            PlayerInstance.LeaveKingdomView();
            AreaRegions.Clear();
            AreaForts.Clear();
//            foreach (var region in AreaRegions)
//            {
//                SendOnExitEvent(region, false);
//            }
        }

/*        public void SendOnEnterEvent(Region region)
        {
            if (!PlayerInstance.IsInKingdomView) return;

            if (region.IsBooked && (region.Owner != PlayerInstance))
            {
                PlayerInstance.SendEvent(EventCode.IaEnter, new IaEnterResponse(region.Owner));

/ *                var attackData = GameService.BRealTimeUpdateManager.GetAttackerData(region.Owner.PlayerId);
                if (attackData != null)
                {
                    var attackResponse = new AttackResponse(attackData.AttackData);
                    this.Owner.SendEvent(EventCode.AttackEvent, attackResponse);
                }* /
            }
        }*/

        public void SendOnExitEvent(Region region, bool validate = true)
        {
            if (validate && !PlayerInstance.IsInKingdomView) return;

            if (region.IsBooked && (region.Owner != PlayerInstance))
            {
                PlayerInstance.SendEvent(EventCode.IaExit, new IaExitResponse(region.Owner.PlayerId));
            }
        }

        public void Dispose()
        {
            this.LeaveKingdomView();
//            this.regionSubscriptions.Clear();
            GC.SuppressFinalize(this);
        }
    }
}

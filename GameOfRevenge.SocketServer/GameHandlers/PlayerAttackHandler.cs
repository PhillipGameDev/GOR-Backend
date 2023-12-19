using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using ExitGames.Logging;
using Newtonsoft.Json;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Hero;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Common.Models.Kingdom.AttackAlertReport;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.GameApplication;
using GameOfRevenge.Interface;
using GameOfRevenge.Model;
using GameOfRevenge.Business;

namespace GameOfRevenge.GameHandlers
{
    public class PlayerAttackHandler : IPlayerAttackHandler
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public readonly PlayerInstance attacker;
        public PlayerInstance Enemy { get; private set; }

        public PlayerAttackHandler(PlayerInstance attacker)
        {
            this.attacker = attacker;
        }

        public async Task<bool> AttackRequestAsync(SendArmyRequest request)
        {
            return await SendMarchingArmyAsync(request);
        }

        public async Task<bool> SendReinforcementsAsync(SendArmyRequest request)
        {
            return await SendMarchingArmyAsync(request, true);
        }

        private async Task<bool> SendMarchingArmyAsync(SendArmyRequest request, bool reinforcement = false)
        {
            log.Debug("@@@@@@@@!!! Send Army Request " + JsonConvert.SerializeObject(request));
            var attackList = GameService.BRealTimeUpdateManager.GetAllAttackerData(attacker.PlayerId);
            if (attackList.Count >= 10)
            {
                var msg = "Maximum number of marching troops reached.";
                attacker.SendOperation(OperationCode.AttackRequest, ReturnCode.Failed, null, msg);

                log.Debug("@@@@@@@@ PROCCESS END - Max 10 marching troops.");
                return false;
            }

            var world = attacker.World;
            var dist = 0;
            var targetId = 0;
            var targetName = "";
            switch ((EntityType)request.TargetType)
            {
                case EntityType.Default:
                case EntityType.Player:
                    Enemy = world.PlayersManager.GetPlayer(request.TargetId);
                    if (Enemy == null)
                    {
                        var msg = "Destination not found.";
                        attacker.SendOperation(OperationCode.AttackRequest, ReturnCode.Failed, null, msg);

                        log.Debug("@@@@@@@@ PROCCESS END - Destination not found.");
                        return false;
                    }
                    else
                    {
                        var sameZone = world.CheckSameZone(attacker.WorldRegion, Enemy.WorldRegion);
                        if (!sameZone)
                        {
                            var msg = "Destination is not in the same zone.";
                            attacker.SendOperation(OperationCode.AttackRequest, ReturnCode.Failed, null, msg);

                            log.Debug("@@@@@@@@ PROCCESS END - Destination is not in the same zone.");
                            return false;
                        }
                    }

                    targetId = Enemy.PlayerId;
                    targetName = Enemy.PlayerInfo.Name;
                    dist = world.GetDistance(attacker.WorldRegion, Enemy.WorldRegion);
                    break;
                case EntityType.Monster:
                    var monster = world.WorldMonsters.Find(e => e.Id == request.TargetId);
                    targetId = monster.Id;//TODO: set monster id from database
                    var targetX = monster.X;
                    var targetY = monster.Y;
                    log.Debug("@@@@@@@@ MONSTER x:"+targetX+"  y:"+targetY);
                    dist = world.GetDistance(attacker.WorldRegion.X, attacker.WorldRegion.Y, targetX, targetY);
                    break;
                case EntityType.Fortress:
                    targetId = request.TargetId;
                    var fortress = world.WorldForts.Find(x => x.ZoneFortressId == targetId);
                    if (fortress == null)
                    {
                        var msg = "";
                        attacker.SendOperation(OperationCode.AttackRequest, ReturnCode.Failed, null, msg);
                        return false;
                    }

                    var totalZonesX = world.TilesX / world.ZoneSize;
                    var fortressX = (fortress.ZoneIndex % totalZonesX);
                    var fortressY = (fortress.ZoneIndex - fortressX) / totalZonesX;
                    fortressX = (fortressX * world.ZoneSize) + (world.ZoneSize / 2);
                    fortressY = (fortressY * world.ZoneSize) + (world.ZoneSize / 2);
                    dist = world.GetDistance(attacker.WorldRegion.X, attacker.WorldRegion.Y, fortressX, fortressY);
                    break;
            }

            var timestart = DateTime.UtcNow.AddSeconds(2);
            var success = true;

            //apply HeroType.AlyamamahEyes - Troop Marching Time
            /*            if (request.HeroIds != null)
                        {
            //                    if (request.HeroIds.Contains((int)Common.Models.Hero.HeroType.AlyamamahEyes))
                        {
            //                        attacker.PlayerDataManager.GetPlayerBuilding(Common.Models.Structure.StructureType.Other, 0);
            //                        attacker.World.PlayersManager.GetPlayer("").
                        }
                        }*/
            try
            {
                var marchingArmy = new MarchingArmy()
                {
                    Troops = new List<TroopInfos>()
                };
                float delay = 0;
                var len = request.Troops.Length;
                for (var idx = 0; idx < len; idx += 3)
                {
                    var troopType = (TroopType)request.Troops[idx];
                    var troopInfo = marchingArmy.Troops.Find(x => (x.TroopType == troopType));
                    if (troopInfo == null)
                    {
                        troopInfo = new TroopInfos()
                        {
                            TroopType = troopType,
                            TroopData = new List<TroopDetails>()
                        };
                        marchingArmy.Troops.Add(troopInfo);
                    }
                    var troop = new TroopDetails()
                    {
                        Level = request.Troops[idx + 1],
                        Count = request.Troops[idx + 2]
                    };
                    troopInfo.TroopData.Add(troop);

                    delay += troop.Count / (float)((troop.Level > 0) ? troop.Level : 1);
                }
                if ((request.HeroIds != null) && (request.HeroIds.Length > 0))
                {
                    marchingArmy.Heroes = Array.ConvertAll(request.HeroIds, x => (HeroType)x).ToList();
                    delay -= marchingArmy.Heroes.Count * 500;
                }
                if (reinforcement)
                {
                    delay = 0;
                }
                else
                {
                    delay /= 100;
                    if (delay < 10) delay = 10;
                }

                marchingArmy.StartTime = timestart;
                marchingArmy.Distance = dist;
                marchingArmy.Duration = (int)delay;

                marchingArmy.TargetId = targetId;

                var resp = await BaseUserDataManager.GetFullPlayerData(attacker.PlayerId);
                if (!resp.IsSuccess || !resp.HasData) throw new DataNotExistExecption(resp.Message);

                AttackResponseData attackData;
                var attackerCompleteData = resp.Data;
                if (reinforcement)
                {
                    await GameService.BkingdomePvpManager.SendReinforcement(attackerCompleteData, marchingArmy);
                    attackData = new AttackResponseData(attackerCompleteData, marchingArmy, targetId);
                }
                else
                {
                    switch ((EntityType)request.TargetType)
                    {
                        default://SendArmyType.Default:
                        case EntityType.Player:
                            var location = new MapLocation() { X = attacker.WorldRegion.X, Y = attacker.WorldRegion.Y };
                            var watchLevel = await GameService.BkingdomePvpManager.AttackOtherPlayer(attackerCompleteData, marchingArmy, location, targetId);

                            attackData = new AttackResponseData(attackerCompleteData, marchingArmy, targetId, targetName, watchLevel);
                            break;
                        case EntityType.Monster:
                            await GameService.BkingdomePvpManager.AttackMonster(attackerCompleteData, marchingArmy);
                            attackData = new AttackResponseData(attackerCompleteData, marchingArmy, targetId);
                            break;
                        case EntityType.Fortress:
                            await GameService.BkingdomePvpManager.AttackGloryKingdom(attackerCompleteData, marchingArmy);
                            attackData = new AttackResponseData(attackerCompleteData, marchingArmy, targetId);
                            break;
                    }
                }
                var str = "Passing Data attackerId {0} Data {1} targetId {2} ";
                log.InfoFormat(str, attacker.PlayerId, JsonConvert.SerializeObject(marchingArmy), targetId);

                var attackResponse = new AttackResponse(attackData);
                attackResponse.X = attacker.WorldRegion.X;
                attackResponse.Y = attacker.WorldRegion.Y;

                attacker.SendEvent(EventCode.AttackEvent, attackResponse);

                if (marchingArmy.MarchingType == MarchingType.AttackPlayer)
                {
                    Enemy.SendEvent(EventCode.UnderAttack, attackResponse);
                }

                var attackStatus = new AttackStatusData()
                {
                    MarchingArmy = marchingArmy,
                    AttackData = attackData
                };
                GameService.BRealTimeUpdateManager.AddNewMarchingArmy(attackStatus);
            }
            catch (Exception ex)
            {
                log.InfoFormat("Exception in Collect Army and Call War {0} {1} ", new object[2] { ex.Message, ex.StackTrace });
                attacker.SendOperation(OperationCode.AttackRequest, ReturnCode.Failed, null, ex.Message);
                success = false;
            }

            log.Debug("@@@@@@@@ PROCCESS END = (" + success + ") " + (DateTime.UtcNow - timestart).TotalSeconds);
            return success;
        }
    }
}

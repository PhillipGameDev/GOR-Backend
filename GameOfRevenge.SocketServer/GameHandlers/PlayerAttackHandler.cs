using ExitGames.Concurrency.Core;
using ExitGames.Concurrency.Fibers;
using ExitGames.Logging;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Kingdom.AttackAlertReport;
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.GameApplication;
using GameOfRevenge.Interface;
using GameOfRevenge.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameOfRevenge.Business;
using GameOfRevenge.Common.Models.Hero;

namespace GameOfRevenge.GameHandlers
{
    public class PlayerAttackHandler : IPlayerAttackHandler
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        public readonly MmoActor attacker;
        public MmoActor Enemy { get; private set; }

        public PlayerAttackHandler(MmoActor attacker)
        {
            this.attacker = attacker;
        }

        public async Task<bool> AttackRequestAsync(AttackRequest request)
        {
            log.Debug("@@@@@@@@!!! Attack Request " + JsonConvert.SerializeObject(request));
            if (GameService.BRealTimeUpdateManager.GetAttackerData(this.attacker.PlayerId) != null)
            {
                attacker.SendOperation(OperationCode.AttackRequest, ReturnCode.Failed, null, "Multiple attack is not supported.");

                log.Debug("@@@@@@@@ PROCCESS END - Multiple attack is not supported.");
                return false;
            }

            Enemy = attacker.World.PlayersManager.GetPlayer(request.EnemyId);
            if (Enemy == null)
            {
                attacker.SendOperation(OperationCode.AttackRequest, ReturnCode.Failed, null, "Enemy not found.");

                log.Debug("@@@@@@@@ PROCCESS END - Enemy not found.");
                return false;
            }

            DateTime timestart = DateTime.UtcNow.AddSeconds(2);
            bool success = true;
            double dist = attacker.World.GetDistanceBw2Points(Enemy.WorldRegion, attacker.WorldRegion) * 100;// / 0.2f;
            int reachedTime = (int)dist;

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
                var marchingRequest = new MarchingArmy()
                {
                    Troops = new List<TroopInfos>()
                };
                float delay = 0;
                var idx = 0;
                var len = request.Troops.Length / 3;
                for (var num = 0; num < len; num++)
                {
                    var troopType = (TroopType)request.Troops[idx];
                    TroopInfos troopInfo = marchingRequest.Troops.Find(x => (x.TroopType == troopType));
                    if (troopInfo == null)
                    {
                        troopInfo = new TroopInfos()
                        {
                            TroopType = troopType,
                            TroopData = new List<TroopDetails>()
                        };
                        marchingRequest.Troops.Add(troopInfo);
                    }
                    var troop = new TroopDetails()
                    {
                        Level = request.Troops[idx + 1],
                        Count = request.Troops[idx + 2]
                    };
                    troopInfo.TroopData.Add(troop);

                    delay += troop.Count / (float)((troop.Level > 0)? troop.Level : 1);
                    idx += 3;
                }
                if (request.HeroIds != null)
                {
                    marchingRequest.Heroes = Array.ConvertAll(request.HeroIds, x => (HeroType)x).ToList();
                    delay -= marchingRequest.Heroes.Count * 500;
                }
                delay /= 100;
                if (delay < 5) delay = 5;
                int battleDuration = (int)delay;

                marchingRequest.StartTime = timestart;
                marchingRequest.ReachedTime = reachedTime;
                marchingRequest.BattleDuration = battleDuration;
                marchingRequest.TargetPlayer = Enemy.PlayerId;

                log.InfoFormat("Passing Data attackerId {0} Data {1} defenderId {2} ", attacker.PlayerId, JsonConvert.SerializeObject(marchingRequest), Enemy.PlayerId);


                var location = new MapLocation() { X = attacker.WorldRegion.X, Y = attacker.WorldRegion.Y };

                var attackResp = await GameService.BkingdomePvpManager.AttackOtherPlayer(attacker.PlayerId, marchingRequest, location, Enemy.PlayerId);
                if ((attackResp.Case >= 100) && (attackResp.Case < 200))
                {
                    var attackStatus = attackResp.Data;
                    var response = new AttackSocketResponse()
                    {
                        AttackerId = attacker.PlayerId,
                        AttackerUsername = attacker.PlayerData.Name,
                        EnemyId = Enemy.PlayerId,
                        EnemyUsername = Enemy.PlayerData.Name,

                        KingLevel = attacker.PlayerData.KingLevel,
                        WatchLevel = attackStatus.Report.WatchLevel,

                        LocationX = location.X,
                        LocationY = location.Y,

                        StartTime = timestart,
                        ReachedTime = reachedTime,
                        BattleDuration = battleDuration
                    };
                    response.Troops = request.Troops;

                    if (attackStatus.Report.Heroes != null)
                    {
                        idx = 0;
                        len = request.HeroIds.Length;
                        var heroes = new int[len * 2];
                        for (int num = 0; num < len; num++)
                        {
                            heroes[idx] = request.HeroIds[num];
                            heroes[idx + 1] = attackStatus.Report.Heroes[num].Level;
                            idx += 2;
                        }
                        response.Heroes = heroes;
                    }
                    attackStatus.AttackData = response;

                    var attackResponse = new AttackResponse(response);
                    attacker.SendEvent(EventCode.AttackResponse, attackResponse);

                    Enemy.SendEvent(EventCode.UnderAttack, attackResponse);

                    GameService.BRealTimeUpdateManager.AddNewAttackOnWorld(attackStatus);
                }
                else
                {
                    attacker.SendOperation(OperationCode.AttackRequest, ReturnCode.Failed, null, attackResp.Message);
                    success = false;
                }
            }
            catch (CacheDataNotExistExecption ex)
            {
                attacker.SendOperation(OperationCode.AttackRequest, ReturnCode.Failed, null, ex.Message);
                success = false;
            }
            catch (DataNotExistExecption ex)
            {
                attacker.SendOperation(OperationCode.AttackRequest, ReturnCode.Failed, null, ex.Message);
                success = false;
            }
            catch (Exception ex)
            {
                log.InfoFormat("Exception in Collect Army and Call War {0} {1} ", new object[2] { ex.Message, ex.StackTrace });
                attacker.SendOperation(OperationCode.AttackRequest, ReturnCode.Failed, null, "Error collecting army");
                success = false;
            }

            log.Debug("@@@@@@@@ PROCCESS END = (" + success + ") " + (DateTime.UtcNow - timestart).TotalSeconds);
            return success;
        }

    }
}

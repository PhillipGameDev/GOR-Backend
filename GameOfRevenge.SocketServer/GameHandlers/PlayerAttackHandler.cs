using ExitGames.Concurrency.Core;
using ExitGames.Concurrency.Fibers;
using ExitGames.Logging;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Kingdom.AttackAlertReport;
using GameOfRevenge.GameApplication;
using GameOfRevenge.Interface;
using GameOfRevenge.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public void AttackRequest(AttackRequest request)
        {
            log.InfoFormat("Attack Request {0} ", new object[1] { JsonConvert.SerializeObject((object)request) });
            if (GameService.BRealTimeUpdateManager.GetAttackerData(this.attacker.PlayerId) == null)
            {
                Enemy = attacker.World.PlayersManager.GetPlayer(request.EnemyUserName);
                if (Enemy != null)
                {
                    DateTime utcNow = DateTime.UtcNow;
                    double num = 0.2;
                    double num2 = attacker.World.GetDistanceBw2Points(Enemy.Tile, attacker.Tile) / num;
                    var socketResponse = new AttackSocketResponse
                    {
                        StartTime = utcNow,
                        AttckerUserName = attacker.UserName,
                        EnemyUserName = request.EnemyUserName,
                        ReachedTime = (int)(num2 * 1000.0),
                        TroopType = request.TroopType,
                        TroopCount = request.TroopCount,
                        Heros = request.HeroIds
                    };
                    var attackResponse = new AttackResponse(socketResponse);
                    try
                    {
                        MarchingArmy val = new MarchingArmy
                        {
                            StartTime = utcNow,
                            TaskTime = utcNow.AddMilliseconds(socketResponse.ReachedTime),
                        };
                        val.EndTime = val.TaskTime.AddMilliseconds(socketResponse.ReachedTime);
                        val.Troops = (new List<TroopInfos>());
                        int i;
                        for (i = 0; i < request.TroopType.Count(); i++)
                        {
                            TroopInfos val2 = val.Troops.Find((TroopInfos d) => (int)d.TroopType == request.TroopType[i]);
                            if (val2 == null)
                            {
                                val2 = new TroopInfos
                                {
                                    TroopType = ((TroopType)request.TroopType[i]),
                                    TroopData = (new List<TroopDetails>())
                                };
                                val.Troops.Add(val2);
                            }
                            List<TroopDetails> troopData = val2.TroopData;
                            TroopDetails val3 = new TroopDetails
                            {
                                Count = request.TroopCount[i],
                                Level = (request.TroopLevel[i])
                            };
                            troopData.Add(val3);
                        }
                        try
                        {
                            val.Heros = request.HeroIds != null ? request.HeroIds.ToList() : new List<int>();
                            log.InfoFormat("Passing Data attackerId {0} Data {1} defenderId {2} ", attacker.PlayerId, JsonConvert.SerializeObject(val), Enemy.PlayerId);
                            var location = new MapLocation() { X = attacker.Tile.X, Y = attacker.Tile.Y };
                            var attackBattle = GameService.BkingdomePvpManager.AttackOtherPlayer(attacker.PlayerId, Enemy.PlayerId, val, location);
                            attackBattle.Wait();
                            GorMmoPeer.Clients.FirstOrDefault(x => x.Actor.PlayerId == Enemy.PlayerId)?.Actor.SendEvent(EventCode.UnderAttack, new object());
                            log.InfoFormat("Attack bAttle response {0} ", JsonConvert.SerializeObject(attackBattle));
                            if (attackBattle.Result.Case > 99 && attackBattle.Result.Case < 200)
                            {
                                attacker.BroadcastWithMe((EventCode)6, attackResponse);
                                attackBattle.Result.Data.AttackData = socketResponse;
                                var defenderId = attackBattle.Result.Data.Report.Defenderid;
                                GameService.BRealTimeUpdateManager.AddNewAttackOnWorld(attackBattle.Result.Data);
                            }
                            else
                            {
                                attackResponse.IsSuccess = false; attackResponse.Message = attackBattle.Result.Message;
                                attacker.SendEvent((EventCode)6, attackResponse);
                            }

                        }
                        catch (Exception ex)
                        {
                            log.InfoFormat("Excpetion in AttackOtherPlayer {0} {1} ", new object[2] { ex.Message, ex.StackTrace });
                        }

                    }
                    catch (Exception ex)
                    {
                        log.InfoFormat("Excpetion in Collect Army and Call War {0} {1} ", new object[2] { ex.Message, ex.StackTrace });
                    }
                }
                else
                {
                    attacker.SendOperation(OperationCode.AttackRequest, (ReturnCode.Failed), null, "enemy not found.");
                }
            }
            else
                attacker.SendOperation(OperationCode.AttackRequest, (ReturnCode.Failed), null, "Multiple attack is not supported.");

        }

    }
}

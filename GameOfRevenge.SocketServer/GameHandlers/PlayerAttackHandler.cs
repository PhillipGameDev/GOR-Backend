using ExitGames.Concurrency.Core;
using ExitGames.Concurrency.Fibers;
using ExitGames.Logging;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Kingdom.AttackAlertReport;
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

        public bool AttackRequest(AttackRequest request)
        {
            bool success = true;
            DateTime timestart = DateTime.UtcNow;
            log.InfoFormat("@@@@@@@@!!! Attack Request {0} ", new object[1] { JsonConvert.SerializeObject((object)request) });
            if (GameService.BRealTimeUpdateManager.GetAttackerData(this.attacker.PlayerId) == null)
            {
                Enemy = attacker.World.PlayersManager.GetPlayer(request.EnemyUserName);
                if (Enemy != null)
                {
                    double num = 0.2;
                    double num2 = attacker.World.GetDistanceBw2Points(Enemy.Tile, attacker.Tile) / num;
                    //apply HeroType.AlyamamahEyes - Troop Marching Time
                    if (request.HeroIds != null)
                    {
//                    if (request.HeroIds.Contains((int)Common.Models.Hero.HeroType.AlyamamahEyes))
                    {
//                        attacker.PlayerDataManager.GetPlayerBuilding(Common.Models.Structure.StructureType.Other, 0);
//                        attacker.World.PlayersManager.GetPlayer("").
                    }
                    }

                    int reachedTime = (int)(num2 * 1000) + (300 * 1000);

                    try
                    {
                        MarchingArmy marching = new MarchingArmy();
//                        val.EndTime = val.TaskTime.AddMilliseconds(socketResponse.ReachedTime);
                        marching.Troops = new List<TroopInfos>();
//                        int delay = 0;
                        int i;
                        for (i = 0; i < request.TroopType.Length; i++)
                        {
                            TroopInfos val2 = marching.Troops.Find((TroopInfos d) => (int)d.TroopType == request.TroopType[i]);
                            if (val2 == null)
                            {
                                val2 = new TroopInfos
                                {
                                    TroopType = (TroopType)request.TroopType[i],
                                    TroopData = new List<TroopDetails>()
                                };
                                marching.Troops.Add(val2);
                            }
                            List<TroopDetails> troopData = val2.TroopData;
                            TroopDetails val3 = new TroopDetails
                            {
                                Count = request.TroopCount[i],
                                Level = request.TroopLevel[i]
                            };
                            troopData.Add(val3);
//                            delay += val3.Count / ((val3.Level > 0)? val3.Level : 1);
                        }
//                        delay = 2000;

                        if (request.HeroIds != null) marching.Heroes = request.HeroIds.ToList();
                        var location = new MapLocation() { X = attacker.Tile.X, Y = attacker.Tile.Y };

                        DateTime utcNow = DateTime.UtcNow;
                        marching.StartTime = utcNow;
                        marching.TaskTime = utcNow.AddMilliseconds(reachedTime);
                        marching.EndTime = utcNow.AddMilliseconds(reachedTime * 2);

                        log.InfoFormat("Passing Data attackerId {0} Data {1} defenderId {2} ", attacker.PlayerId, JsonConvert.SerializeObject(marching), Enemy.PlayerId);
                        try
                        {
                            var task = GameService.BPlayerStructureManager.GetFullPlayerData(Enemy.PlayerId);
                            task.Wait();
                            if (!task.Result.IsSuccess || !task.Result.HasData)
                            {
                                throw new DataNotExistExecption(task.Result.Message);
                            }
                            var defender = task.Result;

                            int watchLevel = 0;
                            var watchTowers = defender.Data.Structures.Where(x => x.StructureType == StructureType.WatchTower)?.FirstOrDefault()?.Buildings;
                            if (watchTowers != null)
                            {
                                foreach (var bldg in watchTowers)
                                {
                                    int lvl = bldg.Level;
                                    if (bldg.TimeLeft > 0) lvl--;

                                    if (lvl > watchLevel) watchLevel = bldg.Level;
                                }
                            }
                            watchLevel = 100;

                            var attackBattle = GameService.BkingdomePvpManager.AttackOtherPlayer(attacker.PlayerId, defender, watchLevel, marching, location);
                            attackBattle.Wait();
                            if ((attackBattle.Result.Case >= 100) && (attackBattle.Result.Case < 200))
                            {
//                                IGorMmoPeer enemyPeer = GorMmoPeer.Clients.Find(x => x.Actor.PlayerId == Enemy.PlayerId);
//                                if (enemyPeer != null) enemyPeer.Actor.SendEvent(EventCode.UnderAttack, new object());//UNDERATTACK NOT SUPPORTED ON CLIENT

                                var socketResponse = new AttackSocketResponse
                                {
                                    StartTime = marching.StartTime
                                };

//                                if (watchLevel >= 1)
                                {//TODO: improve data send username/playerID
                                    socketResponse.AttackerUserId = attacker.PlayerId;
                                    socketResponse.AttckerUserName = attacker.UserName;
                                }
//                                socketResponse.EnemyUserId = request.EnemyId;
                                socketResponse.EnemyUserName = request.EnemyUserName;

                                if (watchLevel >= 3)
                                {
                                    socketResponse.LocationX = attackBattle.Result.Data.Report.Location.X;
                                    socketResponse.LocationY = attackBattle.Result.Data.Report.Location.Y;
                                }

                                if (watchLevel >= 7)
                                {
                                    socketResponse.StartTime = marching.StartTime;
                                }
                                socketResponse.ReachedTime = reachedTime;

                                if (watchLevel >= 17)
                                {
                                    socketResponse.KingLevel = attacker.PlayerDataManager.King.Level;
                                }

                                if (watchLevel >= 23)
                                {
                                    socketResponse.TroopCount = request.TroopCount;
                                }
                                else if (watchLevel >= 11)
                                {
                                    int sum = 0;
                                    Array.ForEach(request.TroopCount, x => sum += x);
                                    socketResponse.TroopCount = new int[1]{ sum };
                                }

                                if (watchLevel >= 19)
                                {
                                    socketResponse.TroopType = request.TroopType;
                                }

                                if (request.HeroIds != null)
                                {
                                    if (watchLevel >= 30)
                                    {
                                        socketResponse.HeroIds = request.HeroIds;
                                    }
                                    else if (watchLevel >= 25)
                                    {
                                        socketResponse.HeroIds = new int[request.HeroIds.Length];
                                    }
                                }

                                var attackResponse = new AttackResponse(socketResponse);
                                if (attackBattle.Result.Case >= 100 && attackBattle.Result.Case < 200)//success <---not necessary, always true
                                {
                                    attacker.BroadcastWithMe(EventCode.AttackResponse, attackResponse);
                                    attackBattle.Result.Data.AttackData = socketResponse;
                                    var defenderId = attackBattle.Result.Data.Report.Defenderid;
                                    GameService.BRealTimeUpdateManager.AddNewAttackOnWorld(attackBattle.Result.Data);
                                }
                                else//fail<--- never reach
                                {
                                    attackResponse.IsSuccess = false; attackResponse.Message = attackBattle.Result.Message;
                                    attacker.SendEvent(EventCode.AttackResponse, attackResponse);
                                }
                            }
                            else
                            {
                                attacker.SendOperation(OperationCode.AttackRequest, (ReturnCode.Failed), null, attackBattle.Result.Message);
                                success = false;
                            }
                        }
                        catch (DataNotExistExecption ex)
                        {
                            attacker.SendOperation(OperationCode.AttackRequest, (ReturnCode.Failed), null, ex.Message);
                            success = false;
                        }
                        catch (Exception ex)
                        {
                            log.InfoFormat("Excpetion in AttackOtherPlayer {0} {1} ", new object[2] { ex.Message, ex.StackTrace });
                            attacker.SendOperation(OperationCode.AttackRequest, (ReturnCode.Failed), null, "Error generating attack");
                            success = false;
                        }

                    }
                    catch (Exception ex)
                    {
                        log.InfoFormat("Excpetion in Collect Army and Call War {0} {1} ", new object[2] { ex.Message, ex.StackTrace });
                        attacker.SendOperation(OperationCode.AttackRequest, (ReturnCode.Failed), null, "Error collecting army");
                        success = false;
                    }
                }
                else
                {
                    attacker.SendOperation(OperationCode.AttackRequest, (ReturnCode.Failed), null, "Enemy not found.");
                    success = false;
                }
            }
            else
            {
                attacker.SendOperation(OperationCode.AttackRequest, (ReturnCode.Failed), null, "Multiple attack is not supported.");
                success = false;
            }

            log.Debug("@@@@@@@@ PROCCESS END =" + (DateTime.UtcNow - timestart).TotalSeconds);
            return success;
        }

    }
}

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

            DateTime timestart = DateTime.UtcNow;
            bool success = true;
            double dist = attacker.World.GetDistanceBw2Points(Enemy.Tile, attacker.Tile) * 3;// / 0.2f;
            int reachedTime = (int)dist;
            int battleDuration = 0;

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
                try
                {
                    var task1 = GameService.BPlayerStructureManager.GetFullPlayerData(attacker.PlayerId);
                    task1.Wait();
                    if (!task1.Result.IsSuccess || !task1.Result.HasData) throw new DataNotExistExecption(task1.Result.Message);

                    var attackerData = task1.Result.Data;
                    var userTroops = attackerData.Troops;
                    if ((userTroops == null) || (userTroops.Count == 0)) throw new DataNotExistExecption("User troop unavailable");

                    var task2 = GameService.BPlayerStructureManager.GetAllPlayerData(Enemy.PlayerId);
                    task2.Wait();
                    if (!task2.Result.IsSuccess || !task2.Result.HasData) throw new DataNotExistExecption(task2.Result.Message);

                    var enemyAllData = task2.Result.Data;

                    var marching = new MarchingArmy()
                    {
                        Troops = new List<TroopInfos>()
                    };
                    //            val.EndTime = val.TaskTime.AddMilliseconds(socketResponse.ReachedTime);
                    //            int delay = 0;

                    for (int i = 0; i < request.TroopType.Length; i++)
                    {
                        TroopType troopType = (TroopType)request.TroopType[i];
                        TroopInfos troopInfo = marching.Troops.Find(x => (x.TroopType == troopType));
                        if (troopInfo == null)
                        {
                            troopInfo = new TroopInfos
                            {
                                TroopType = troopType,
                                TroopData = new List<TroopDetails>()
                            };
                            marching.Troops.Add(troopInfo);
                        }
                        troopInfo.TroopData.Add(new TroopDetails
                        {
                            Count = request.TroopCount[i],
                            Level = request.TroopLevel[i]
                        });

//                            delay += val3.Count / ((val3.Level > 0)? val3.Level : 1);
                    }
                    //            delay = 2000;

                    if (request.HeroIds != null) marching.Heroes = request.HeroIds.ToList();
                    var location = new MapLocation() { X = attacker.Tile.X, Y = attacker.Tile.Y };

                    log.InfoFormat("Passing Data attackerId {0} Data {1} defenderId {2} ", attacker.PlayerId, JsonConvert.SerializeObject(marching), Enemy.PlayerId);



                    DateTime utcNow = DateTime.Now.AddSeconds(2);
                    marching.StartTime = utcNow;
                    marching.ReachedTime = reachedTime;
                    marching.BattleDuration = battleDuration;
                    marching.TargetPlayer = Enemy.PlayerId;






//                    var defender = task.Result;
                    //DEFENDER BOOSTS

                    //TODO: improve logic with find groups
                    int watchLevel = 0;
                    var defenderStructures = enemyAllData.Where(x => x.DataType == DataType.Structure).ToList();
                    foreach (var item in defenderStructures)
                    {
                        if (item == null) continue;

                        var structureTable = CacheStructureDataManager.GetStructureTable(item.ValueId);
                        if (structureTable.Code != StructureType.WatchTower) continue;

                        var watchTowers = JsonConvert.DeserializeObject<List<StructureDetails>>(item.Value);
                        if (watchTowers?.Count > 0)
                        {
                            watchLevel = watchTowers.Max(x => (x.TimeLeft > 0)? (x.Level - 1) : x.Level);
                        }
                        break;
                    }

                    var defenderActiveBoosts = new List<UserNewBoost>();
                    var defenderBoosts = enemyAllData.Where(x => x.DataType == DataType.ActiveBoost).ToList();
                    foreach (var item in defenderBoosts)
                    {
//                        var bufdData = GameService.BUserActiveBoostManager.GetAllPlayerActiveBoostData();//  PlayerDataToUserBoostData(item);
                        var activeBoost = JsonConvert.DeserializeObject<UserNewBoost>(item.Value);
                        if (activeBoost?.TimeLeft > 0)
                        {
                            defenderActiveBoosts.Add(new UserRecordNewBoost(item.Id, activeBoost));
                        }
                    }

                    var attackBattle = GameService.BkingdomePvpManager.AttackOtherPlayer(attacker.PlayerId, attackerData, Enemy.PlayerId, defenderActiveBoosts, watchLevel, marching, location);
                    attackBattle.Wait();
                    if ((attackBattle.Result.Case >= 100) && (attackBattle.Result.Case < 200))
                    {
//                                IGorMmoPeer enemyPeer = GorMmoPeer.Clients.Find(x => x.Actor.PlayerId == Enemy.PlayerId);
//                                if (enemyPeer != null) enemyPeer.Actor.SendEvent(EventCode.UnderAttack, new object());

                        var socketResponse = new AttackSocketResponse();
//                        {
//                            StartTime = marching.StartTime
//                        };

//                                if (watchLevel >= 1)
                        {
                            socketResponse.AttackerId = attacker.PlayerId;
                            socketResponse.AttackerUsername = attacker.Username;
                        }
                        socketResponse.EnemyId = Enemy.PlayerId;
                        socketResponse.EnemyUsername = Enemy.Username;// (enemyPeer != null)? enemyPeer.Actor.Username : string.Empty;

                        if (attackBattle.Result.Data.Report.Location != null)//watchLevel >= 3)
                        {
                            socketResponse.LocationX = attackBattle.Result.Data.Report.Location.X;
                            socketResponse.LocationY = attackBattle.Result.Data.Report.Location.Y;
                        }

//                        if (attackBattle.Result.Data.Report.StartTime != null)//watchLevel >= 7)
                        {
                            socketResponse.StartTime = attackBattle.Result.Data.Report.StartTime;
                            socketResponse.ReachedTime = reachedTime;
                            socketResponse.BattleDuration = battleDuration;
                        }

                        if (watchLevel >= 17)
                        {
                            socketResponse.KingLevel = attacker.InternalPlayerDataManager.King.Level;
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
//                                if (attackBattle.Result.Case >= 100 && attackBattle.Result.Case < 200)//success <---not necessary, always true
                        {
                            attacker.SendEvent(EventCode.AttackResponse, attackResponse);
                            //                                    var enemyPeer = GorMmoPeer.Clients.Find(x => x.Actor.PlayerId == Enemy.PlayerId);
                            //if (enemyPeer != null) enemyPeer.Actor.SendEvent(EventCode.UnderAttack, attackResponse);
                            Enemy.SendEvent(EventCode.UnderAttack, attackResponse);

//                                    attacker.BroadcastWithMe(EventCode.AttackResponse, attackResponse);

                            attackBattle.Result.Data.AttackData = socketResponse;
//                                    var defenderId = attackBattle.Result.Data.Report.DefenderId;
                            GameService.BRealTimeUpdateManager.AddNewAttackOnWorld(attackBattle.Result.Data);
                        }
/*                                else//fail<--- never reach
                        {
                            attackResponse.IsSuccess = false; attackResponse.Message = attackBattle.Result.Message;
                            attacker.SendEvent(EventCode.AttackResponse, attackResponse);
                        }*/
                    }
                    else
                    {
                        attacker.SendOperation(OperationCode.AttackRequest, ReturnCode.Failed, null, attackBattle.Result.Message);
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
                    log.InfoFormat("Excpetion in AttackOtherPlayer {0} {1} ", new object[2] { ex.Message, ex.StackTrace });
                    attacker.SendOperation(OperationCode.AttackRequest, ReturnCode.Failed, null, "Error generating attack");
                    success = false;
                }

            }
            catch (Exception ex)
            {
                log.InfoFormat("Excpetion in Collect Army and Call War {0} {1} ", new object[2] { ex.Message, ex.StackTrace });
                attacker.SendOperation(OperationCode.AttackRequest, ReturnCode.Failed, null, "Error collecting army");
                success = false;
            }

            log.Debug("@@@@@@@@ PROCCESS END = (" + success + ") " + (DateTime.UtcNow - timestart).TotalSeconds);
            return success;
        }

    }
}

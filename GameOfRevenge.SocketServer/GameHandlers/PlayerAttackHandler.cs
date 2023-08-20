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
            var attackList = GameService.BRealTimeUpdateManager.GetAllAttackerData(this.attacker.PlayerId);
            if ((attackList != null) && (attackList.Count >= 10))
            {
                attacker.SendOperation(OperationCode.AttackRequest, ReturnCode.Failed, null, "Maximum number of marching troops reached.");

                log.Debug("@@@@@@@@ PROCCESS END - Max 10 marching troops.");
                return false;
            }

            Enemy = attacker.World.PlayersManager.GetPlayer(request.TargetPlayerId);
            if (Enemy == null)
            {
                attacker.SendOperation(OperationCode.AttackRequest, ReturnCode.Failed, null, "Destination not found.");

                log.Debug("@@@@@@@@ PROCCESS END - Destination not found.");
                return false;
            }

            var timestart = DateTime.UtcNow.AddSeconds(2);
            var success = true;
            var dist = attacker.World.GetDistanceBw2Points(Enemy.WorldRegion, attacker.WorldRegion) * 100;// / 0.2f;

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
                delay /= 100;
                if (delay < 10) delay = 10;

                marchingArmy.StartTime = timestart;
                marchingArmy.Distance = (int)dist;
                marchingArmy.Duration = (int)delay;
                marchingArmy.TargetId = Enemy.PlayerId;

                var resp = await BaseUserDataManager.GetFullPlayerData(attacker.PlayerId);
                if (!resp.IsSuccess || !resp.HasData) throw new DataNotExistExecption(resp.Message);

                AttackResponseData attackData;
                var attackerCompleteData = resp.Data;
                if (reinforcement)
                {
                    await GameService.BkingdomePvpManager.SendReinforcement(attackerCompleteData, marchingArmy);
                    attackData = new AttackResponseData(attackerCompleteData, marchingArmy, Enemy.PlayerId, 0);
                }
                else
                {
                    if (Enemy.PlayerData.Invaded == 0)
                    {
                        var location = new MapLocation() { X = attacker.WorldRegion.X, Y = attacker.WorldRegion.Y };
                        var watchLevel = await GameService.BkingdomePvpManager.AttackOtherPlayer(attackerCompleteData, marchingArmy, location, Enemy.PlayerId);

                        attackData = new AttackResponseData(attackerCompleteData, marchingArmy, Enemy.PlayerId, Enemy.PlayerData.Name, watchLevel);
                    }
                    else
                    {
                        await GameService.BkingdomePvpManager.AttackMonster(attackerCompleteData, marchingArmy);
                        attackData = new AttackResponseData(attackerCompleteData, marchingArmy, Enemy.PlayerId, Enemy.PlayerData.Invaded);
                    }
                }
                var str = "Passing Data attackerId {0} Data {1} defenderId {2} ";
                log.InfoFormat(str, attacker.PlayerId, JsonConvert.SerializeObject(marchingArmy), Enemy.PlayerId);

                var attackResponse = new AttackResponse(attackData);
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

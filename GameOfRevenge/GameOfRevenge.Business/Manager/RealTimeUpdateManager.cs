using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExitGames.Logging;
using GameOfRevenge.Business.Manager.UserData;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Services;

namespace GameOfRevenge.Business.Manager
{
    public class RealTimeUpdateManager : IRealTimeUpdateManager
    {
        public static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private static readonly List<AttackStatusData> attackPlayerData = new List<AttackStatusData>();
        private readonly IKingdomPvPManager pvpManager = new KingdomPvPManager();
        public DelayedAction Disposable;
        protected readonly object SyncRoot = new object(); // that is for world user access

        public AttackStatusData GetAttackerData(int attackerId)
        {
            AttackStatusData attackerData = null;
            lock (SyncRoot)
            {
                attackerData = attackPlayerData.Where(d => d.Attacker.PlayerId == attackerId).FirstOrDefault();
            }
            return attackerData;
        }

        public AttackStatusData GetDefenderData(int defenderId)
        {
            AttackStatusData attackerData = null;
            lock (SyncRoot)
            {
                attackerData = attackPlayerData.Where(d => d.Defender.PlayerId == defenderId).FirstOrDefault();
            }
            return attackerData;
        }

        public void AddNewAttackOnWorld(AttackStatusData data)
        {
            lock (SyncRoot)
            {
                attackPlayerData.Add(data);
            }
        }

        public async Task Update(Action<AttackStatusData> CallBackResult)
        {
            if (attackPlayerData.Count > 0)
            {
                List<AttackStatusData> attackerList = null;
                lock (SyncRoot)
                {
                    attackerList = attackPlayerData.ToList();
                }
                log.Debug("****** UPDATE BATTLE START ****** x " + attackerList.Count);
                foreach (AttackStatusData item in attackerList)
                {
                    try
                    {
                        switch (item.State)
                        {
                            case 0://marching to target
//                                log.Debug("** MARCHING **");
                                if (item.Attacker.Data.MarchingArmy.TimeLeftForTask <= 0) item.State++;
                                break;
                            case 1://battle simulation
//                                log.Debug("** BATTLE **");
                                var response = await pvpManager.BattleSimulation(item.Attacker.PlayerId, item.Attacker.Data, item.Defender.PlayerId, item.Defender.Data);
                                if (response.Case >= 200)//illegal
                                {
                                }
                                else if (response.Case < 100)//error
                                {
                                }
                                else
                                {
                                    item.WinnerPlayerId = response.Data.AttackerWon ? item.Attacker.PlayerId : item.Defender.PlayerId;
                                }
                                item.State++;
                                break;
                            case 2:
//                                log.Debug("** WAITING **");
                                if (item.Attacker.Data.MarchingArmy.IsTimeForReturn) item.State++;
                                break;
                            case 3://marching to castle
//                                log.Debug("** MARCHING BACK **");
                                if (item.Attacker.Data.MarchingArmy.TimeLeft <= 0) item.State++;
                                break;
                            case 4://end
                                log.Debug("** END ** " + item.Attacker.PlayerId +" vs "+item.Defender.PlayerId);
                                CallBackResult(item);
                                lock (SyncRoot) attackPlayerData.Remove(item);
                                break;
                        }
                    }
                    catch (Exception exx)
                    {
                        log.Debug("EXCEPTION " + exx.Message);
                    }
                }

                log.Debug("****** UPDATE BATTLE END ******");
            }

            if (Disposable != null) Disposable.Dispose();
            Disposable = new DelayedAction();
            Disposable.WaitForCallBack(async () => { await Update(CallBackResult); }, 1000);
        }
    }
}

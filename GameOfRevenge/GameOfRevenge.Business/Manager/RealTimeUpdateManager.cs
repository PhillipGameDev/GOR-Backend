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
        protected readonly object SyncRoot = new object(); // that is for world user access

        public DelayedAction Disposable;

        public AttackStatusData GetAttackerData(int attackerId)
        {
            AttackStatusData data = null;
            lock (SyncRoot)
            {
                data = attackPlayerData.Where(d => d.Attacker.PlayerId == attackerId).FirstOrDefault();
            }
            return data;
        }

        public AttackStatusData GetDefenderData(int defenderId)
        {
            AttackStatusData data = null;
            lock (SyncRoot)
            {
                data = attackPlayerData.Where(d => d.Defender.PlayerId == defenderId).FirstOrDefault();
            }
            return data;
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
                List<AttackStatusData> list = null;
                lock (SyncRoot)
                {
                    list = attackPlayerData.ToList();
                }
                if (list != null)
                {
                    log.Debug("****** UPDATE BATTLE START ****** x " + list.Count);
                    foreach (AttackStatusData item in list)
                    {
                        string debugMsg = "";
                        try
                        {
                            debugMsg = item.State+"";
                            switch (item.State)
                            {
                                case 0://marching to target
    //                                log.Debug("** MARCHING **");
                                    if (item.Attacker.MarchingArmy.TimeLeftForTask <= 0)
                                    {
                                        var resp = await new UserResourceManager().GetFullPlayerData(item.Report.DefenderId);
                                        if (!resp.IsSuccess) throw new Exception("defender data not found");

                                        item.Defender = resp.Data;
                                        item.State++;
                                    }
                                    break;
                                case 1://battle simulation
    //                                log.Debug("** BATTLE **");
                                    //TODO: split battle simulation
                                    var response = await pvpManager.BattleSimulation(item.Attacker, item.Defender);
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
    //                                log.Debug("** WAITING ** "+item.Attacker.MarchingArmy.StartTime+"  "+item.Attacker.MarchingArmy.ReachedTime+"  "+ item.Attacker.MarchingArmy.BattleDuration+"    "+item.Attacker.MarchingArmy.IsTimeForReturn);
                                    if (item.Attacker.MarchingArmy.IsTimeForReturn) item.State++;
                                    break;
                                case 3://marching to castle
    //                                log.Debug("** MARCHING BACK **  "+item.Attacker.MarchingArmy.TimeLeft);
                                    if (item.Attacker.MarchingArmy.TimeLeft <= 0) item.State++;
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
                            log.Debug("EXCEPTION " + debugMsg+"  "+ exx.Message);
                            lock (SyncRoot) attackPlayerData.Remove(item);
                        }
                    }
                    log.Debug("****** UPDATE BATTLE END ******");
                }
            }

            if (Disposable != null) Disposable.Dispose();
            Disposable = new DelayedAction();
            Disposable.WaitForCallBack(async () => { await Update(CallBackResult); }, 1000);
        }
    }
}

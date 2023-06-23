using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExitGames.Logging;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Business.Manager.UserData;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Kingdom;
using GameOfRevenge.Common.Services;

namespace GameOfRevenge.Business.Manager
{
    public class RealTimeUpdateManager : IRealTimeUpdateManager
    {
        public static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private static readonly List<AttackStatusData> attackInProgress = new List<AttackStatusData>();

        private readonly KingdomPvPManager pvpManager = new KingdomPvPManager();
        protected readonly object SyncRoot = new object(); // that is for world user access

        public DelayedAction Disposable;

        public AttackStatusData GetAttackerData(int attackerId)
        {
            AttackStatusData data = null;
            lock (SyncRoot)
            {
                data = attackInProgress.Find(x => (x.AttackData.AttackerId == attackerId));
            }
            return data;
        }

        public AttackStatusData GetAttackDataForDefender(int defenderId)
        {
            AttackStatusData data = null;
            lock (SyncRoot)
            {
                data = attackInProgress.Find(x => (x.AttackData.EnemyId == defenderId));
            }
            return data;
        }

        public void AddNewAttackOnWorld(AttackStatusData data)
        {
            if (data.MarchingArmy.Report != null) data.State = 5;
            lock (SyncRoot)
            {
                attackInProgress.Add(data);
            }
        }

        public async Task Update(Action<AttackStatusData, bool> attackResultCallback)
        {
            if (attackInProgress.Count > 0)
            {
                List<AttackStatusData> list = null;
                lock (SyncRoot)
                {
                    list = attackInProgress.ToList();
                }
                if (list != null)
                {
                    double timeleft = double.MaxValue;
                    log.Debug("****** UPDATE BATTLE START ****** x " + list.Count);
                    foreach (AttackStatusData item in list)
                    {
                        try
                        {
                            switch (item.State)
                            {
                                case 0://marching to target
                                    if (item.MarchingArmy.TimeLeftForTask <= 0)
                                    {
                                        var atkResp = await BaseUserDataManager.GetFullPlayerData(item.AttackData.AttackerId);
                                        if (!atkResp.IsSuccess) throw new Exception("attacker data not found");

                                        item.Attacker = atkResp.Data;
                                        item.State++;
                                    }
                                    break;

                                case 1://pulling data
                                    var defResp = await BaseUserDataManager.GetFullPlayerData(item.AttackData.EnemyId);
                                    if (!defResp.IsSuccess) throw new Exception("defender data not found");

                                    item.Defender = defResp.Data;
                                    item.State++;
                                    break;

                                case 2://prepare data
                                    var (atkPower, defPower) = pvpManager.PrepareBattleData(item.Attacker, item.MarchingArmy, item.Defender);
                                    item.AttackerPower = atkPower;
                                    item.DefenderPower = defPower;

                                    log.Debug("atk pwr= " + atkPower.HitPoints + " vs def pwr=" + defPower.HitPoints);
                                    item.State++;
                                    break;

                                case 3://battle simulation
                                    if ((item.DefenderPower.HitPoints > 0) && (item.AttackerPower.HitPoints > 0))
                                    {
//                                        pvpManager.Attack(item.AttackerPower, item.DefenderPower);
                                        item.AttackerPower.AttackPlayer(item.DefenderPower);
                                        log.Debug("atk pwr= " + item.AttackerPower.HitPoints + " xx def pwr=" + item.DefenderPower.HitPoints);
                                    }
                                    else
                                    {
                                        await pvpManager.FinishBattleData(item.Attacker, item.AttackerPower, item.Defender, item.DefenderPower, item.MarchingArmy);
//                                        item.BattleReport = reportResp.Result;
                                        item.State++;
                                    }
                                    break;

                                case 4://waiting for return
                                    if (item.MarchingArmy.IsTimeForReturn)
                                    {
                                        log.Debug("send defender under attack end event");
                                        attackResultCallback(item, false);
                                        item.State++;
                                    }
                                    break;

                                case 5://marching to castle
                                    if (item.MarchingArmy.TimeLeft <= 0)
                                    {
                                        //SAVE PLAYER REPORT
                                        log.Debug("ApplyAttackerChangesAndSendReport!!!");
                                        await pvpManager.ApplyAttackerChangesAndSendReport((s)=> { log.Debug(s); }, item.MarchingArmy);

                                        log.Debug("send attacker attack end event");
                                        attackResultCallback(item, true);
                                        log.Debug("** END ** " + item.AttackData.AttackerId + " vs " + item.AttackData.EnemyId);
                                        lock (SyncRoot) attackInProgress.Remove(item);
                                    }
                                    break;
                            }
                            if (item.MarchingArmy.TimeLeft < timeleft) timeleft = item.MarchingArmy.TimeLeft;
                        }
                        catch (Exception ex)
                        {
                            log.Debug("EXCEPTION " + item.State+"  "+ item.AttackData.AttackerId+" vs "+item.AttackData.EnemyId+"  " +ex.Message);
                            lock (SyncRoot) attackInProgress.Remove(item);
                        }
                    }
                    log.Debug("****** UPDATE BATTLE END ****** " + timeleft);
                    list.Clear();
                    list = null;
                }
            }

            if (Disposable != null) Disposable.Dispose();
            Disposable = new DelayedAction();
            Disposable.WaitForCallBack(async () => { await Update(attackResultCallback); }, 1000);
        }
    }
}

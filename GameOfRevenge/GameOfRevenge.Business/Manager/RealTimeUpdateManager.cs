using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExitGames.Logging;
using GameOfRevenge.Business.Manager.UserData;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Services;

namespace GameOfRevenge.Business.Manager
{
    public class RealTimeUpdateManager : IRealTimeUpdateManager
    {
        public static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private static readonly UserResourceManager userResourceManager = new UserResourceManager();
        private static readonly List<AttackStatusData> attackPlayerData = new List<AttackStatusData>();

        private readonly KingdomPvPManager pvpManager = new KingdomPvPManager();
        protected readonly object SyncRoot = new object(); // that is for world user access

        public DelayedAction Disposable;

        public AttackStatusData GetAttackerData(int attackerId)
        {
            AttackStatusData data = null;
            lock (SyncRoot)
            {
                data = attackPlayerData.Find(x => (x.Attacker.PlayerId == attackerId));
            }
            return data;
        }

        public AttackStatusData GetAttackDataForDefender(int defenderId)
        {
            AttackStatusData data = null;
            lock (SyncRoot)
            {
                data = attackPlayerData.Find(x => (x.AttackData.EnemyId == defenderId));
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
                        try
                        {
                            switch (item.State)
                            {
                                case 0://marching to target
                                    if (item.Attacker.MarchingArmy.TimeLeftForTask <= 0)
                                    {
                                        var resp = await userResourceManager.GetFullPlayerData(item.Report.DefenderId);
                                        if (!resp.IsSuccess) throw new Exception("defender data not found");

                                        item.Defender = resp.Data;
                                        item.State++;
                                    }
                                    break;

                                case 1://prepare data
                                    var (atkPower, defPower) = pvpManager.PrepareBattleData(item.Attacker, item.Defender);
                                    item.AttackerPower = atkPower;
                                    item.DefenderPower = defPower;

                                    item.initialAttackerAtkPower = atkPower.Attack;
                                    item.initialAttackerDefPower = atkPower.Defense;
                                    item.initialDefenderAtkPower = defPower.Attack;
                                    item.initialDefenderDefPower = defPower.Defense;

                                    log.Debug("atk pwr= " + atkPower.HitPoints + " vs def pwr=" + defPower.HitPoints);
                                    item.State++;
                                    break;

                                case 2://battle simulation
                                    if ((item.DefenderPower.HitPoints > 0) && (item.AttackerPower.HitPoints > 0))
                                    {
                                        pvpManager.Attack(item.AttackerPower, item.DefenderPower);
                                        log.Debug("atk pwr= " + item.AttackerPower.HitPoints + " xx def pwr=" + item.DefenderPower.HitPoints);
                                    }
                                    else
                                    {
                                        item.AttackerPower.Attack = item.initialAttackerAtkPower;
                                        item.AttackerPower.Defense = item.initialAttackerDefPower;
                                        item.DefenderPower.Attack = item.initialDefenderAtkPower;
                                        item.DefenderPower.Defense = item.initialDefenderDefPower;

                                        var report = pvpManager.FinishBattleData(item.Attacker, item.AttackerPower, item.Defender, item.DefenderPower);
                                        item.BattleReport = report;
                                        item.WinnerPlayerId = report.AttackerWon? item.Attacker.PlayerId : item.Defender.PlayerId;
                                        item.State++;
                                    }
                                    break;

                                case 3://waiting for return
                                    if (item.Attacker.MarchingArmy.IsTimeForReturn)
                                    {
                                        //SAVE DEFENDER REPORT
                                        await pvpManager.ApplyDefenderChangesAndSendReport(item.BattleReport, item.Defender);
                                        item.State++;
                                    }
                                    break;

                                case 4://marching to castle
                                    if (item.Attacker.MarchingArmy.TimeLeft <= 0)
                                    {
                                        //SAVE PLAYER REPORT AND END BATTLE
                                        await pvpManager.ApplyAttackerChangesAndSendReport(item.BattleReport, item.Attacker);
                                        item.State++;
                                    }
                                    break;

                                case 5://end
                                    log.Debug("** END ** " + item.Attacker.PlayerId +" vs "+item.Defender.PlayerId);
                                    CallBackResult(item);
                                    lock (SyncRoot) attackPlayerData.Remove(item);
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Debug("EXCEPTION " + item.State+"  "+ item.Attacker.PlayerId+" vs "+item.Defender.PlayerId+"  " +ex.Message);
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

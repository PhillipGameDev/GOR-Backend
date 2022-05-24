using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameOfRevenge.Business.Manager.UserData;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Services;

namespace GameOfRevenge.Business.Manager
{
    public class RealTimeUpdateManager : IRealTimeUpdateManager
    {
        private static readonly List<AttackStatusData> attackPlayerData = new List<AttackStatusData>();
        private readonly IKingdomPvPManager pvpManager = new KingdomPvPManager();
        public DelayedAction Disposable;
        protected readonly object SyncRoot = new object(); // that is for world user access

        public AttackStatusData GetAttackerData(int attackerId)
        {
            AttackStatusData attackerData = null;
            lock (SyncRoot)
                attackerData = attackPlayerData.Where(d => d.Attacker.PlayerId == attackerId).FirstOrDefault();
            return attackerData;
        }

        public void AddNewAttackOnWorld(AttackStatusData data)
        {
            lock (SyncRoot)
                attackPlayerData.Add(data);
        }

        public async Task Update(Action<AttackStatusData> CallBackResult)
        {
            var attackerList = attackPlayerData.ToList();

            foreach (var item in attackerList)
            {
                if (item.Attacker.Data.MarchingArmy.TimeLeft <= 0)
                {
                    var response = await pvpManager.BattleSimulation(item.Attacker.PlayerId, item.Attacker.Data, item.Defender.PlayerId, item.Defender.Data);
                    item.WinnerPlayerId = response.Data.AttackerWon == true ? item.Attacker.PlayerId : item.Defender.PlayerId;
                    CallBackResult(item);
                    lock (attackPlayerData)
                        attackPlayerData.Remove(item);
                }
            }

            if (Disposable != null) Disposable.Dispose();
            Disposable = new DelayedAction();
            Disposable.WaitForCallBack(async () => { await Update(CallBackResult); }, 1000);
        }
    }
}

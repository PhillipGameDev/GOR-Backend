using GameOfRevenge.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GameOfRevenge.Common.Interface
{
    public interface IRealTimeUpdateManager
    {
        AttackStatusData GetAttackerData(int attackerId);
        AttackStatusData GetDefenderData(int defenderId);
        void AddNewAttackOnWorld(AttackStatusData data);
        Task Update(Action<AttackStatusData> CallBackResult);
    }
}

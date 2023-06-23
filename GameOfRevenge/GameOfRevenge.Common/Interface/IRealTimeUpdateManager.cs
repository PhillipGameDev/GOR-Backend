using System;
using System.Threading.Tasks;
using GameOfRevenge.Common.Models;

namespace GameOfRevenge.Common.Interface
{
    public interface IRealTimeUpdateManager
    {
        AttackStatusData GetAttackerData(int attackerId);
        AttackStatusData GetAttackDataForDefender(int defenderId);
        void AddNewAttackOnWorld(AttackStatusData data);
        Task Update(Action<AttackStatusData, bool> callBackResult);
    }
}

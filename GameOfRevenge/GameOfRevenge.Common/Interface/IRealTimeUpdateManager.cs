using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.PlayerData;

namespace GameOfRevenge.Common.Interface
{
    public interface IRealTimeUpdateManager
    {
        List<AttackStatusData> GetAllAttackerData(int attackerId);
        List<AttackStatusData> GetAllAttackDataForDefender(int defenderId);
        void AddNewMarchingArmy(AttackStatusData data);
        bool UpdateMarchingArmy(MarchingArmy marchingArmy);
        MarchingArmy GetMarchingArmy(long marchingId);
        Task Update(Action<AttackStatusData, int> callBackResult);
    }
}

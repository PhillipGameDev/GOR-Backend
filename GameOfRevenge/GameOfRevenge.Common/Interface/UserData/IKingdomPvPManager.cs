using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Kingdom;
using GameOfRevenge.Common.Models.Kingdom.AttackAlertReport;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Common.Interface.UserData
{
    public interface IKingdomPvPManager
    {
        Task<Response<AttackStatusData>> AttackOtherPlayer(int attackerId, MarchingArmy army, MapLocation location, int defenderId);
//        (BattlePower, BattlePower) PrepareBattleData(PlayerCompleteData attackerArmy, PlayerCompleteData defenderArmy);
//        Task<Response<BattleReport>> BattleSimulation(PlayerCompleteData attackerData, PlayerCompleteData defenderData);
//        void ApplyPlayerArmyToMarch(PlayerCompleteData data, MarchingArmy army, bool removeArmy);
    }
}

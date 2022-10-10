using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Kingdom;
using GameOfRevenge.Common.Models.Kingdom.AttackAlertReport;
using GameOfRevenge.Common.Net;
using static GameOfRevenge.Common.Models.Kingdom.AttackAlertReport.UnderAttackReport;

namespace GameOfRevenge.Common.Interface.UserData
{
    public interface IKingdomPvPManager
    {
//        Task<Response<AttackStatusData>> AttackOtherPlayer(int attackerId, int defenderId, int watchLevel, MarchingArmy army, MapLocation location);
//        Task<Response<AttackStatusData>> AttackOtherPlayer(int attackerId, Response<PlayerCompleteData> defender, int watchLevel, MarchingArmy army, MapLocation location);
        Task<Response<AttackStatusData>> AttackOtherPlayer(int attackerId, PlayerCompleteData attackerData, int defenderId, List<UserNewBoost> defenderBoosts, int watchLevel, MarchingArmy army, MapLocation location);
        Task<Response<BattleReport>> BattleSimulation(PlayerCompleteData attackerData, PlayerCompleteData defenderData);
    }
}

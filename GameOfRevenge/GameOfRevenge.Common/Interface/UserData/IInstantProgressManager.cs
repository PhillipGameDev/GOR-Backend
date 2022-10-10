using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Net;
using System.Threading.Tasks;

namespace GameOfRevenge.Common.Interface.UserData
{
    public interface IInstantProgressManager
    {
        Response<int> GetInstantBuildCost(int playerId, StructureType structType, int level);
        Task<Response<int>> GetBuildingSpeedUpCost(int playerId, int locId);
        Task<Response<UserStructureData>> InstantBuildStructure(int playerId, StructureType structType, int level, int locId);
        Task<Response<UserStructureData>> SpeedUpBuildStructure(int playerId, int locId, PlayerCompleteData playerData = null);
        Task<Response<UserTroopData>> InstantRecruitTroops(int playerId, int locationId, TroopType troopType, int troopLevel, int troopCount);
        Response<int> GetInstantRecruitCost(TroopType type, int troopLevel, int count);
    }
}

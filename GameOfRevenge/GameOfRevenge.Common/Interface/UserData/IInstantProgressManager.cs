using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Net;
using System.Threading.Tasks;

namespace GameOfRevenge.Common.Interface.UserData
{
    public interface IInstantProgressManager
    {
        Response<int> GetInstantBuildCost(int playerId, StructureType structureType, int level);
        Task<Response<int>> GetBuildingSpeedUpCost(int playerId, int location);
        Task<Response<BuildingStructureData>> InstantBuildStructure(int playerId, StructureType structureType, int level, int location);
        Task<Response<UserStructureData>> SpeedUpBuildStructure(int playerId, int location, PlayerCompleteData playerData = null);
        Task<Response<UserTroopData>> InstantRecruitTroops(int playerId, int location, TroopType troopType, int troopLevel, int troopCount);
        Response<int> GetInstantRecruitCost(TroopType type, int troopLevel, int count);
    }
}

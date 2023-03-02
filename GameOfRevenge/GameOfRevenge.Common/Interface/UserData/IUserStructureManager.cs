using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Common.Interface.UserData
{
    public interface IUserStructureManager : IBaseUserManager
    {
        Task<Response<UserStructureData>> CreateBuilding(int playerId, StructureType type, int location);
        Task<Response<UserStructureData>> CreateBuilding(int playerId, StructureType type, int location, bool removeRes, bool createBuilder, bool instantBuild);
        Task<Response<UserStructureData>> UpgradeBuilding(int playerId, StructureType type, int location);
        Task<Response<UserStructureData>> UpgradeBuilding(int playerId, StructureType type, int location, bool removeRes, bool createBuilder);
        Task<Response<UserStructureData>> HelpBuilding(int playerId, int toPlayerId, StructureType type, int location, int seconds);
        Task<Response<UserStructureData>> DestroyBuilding(int playerId, StructureType type, int location);
        Task<Response<UserStructureData>> CheckBuildingStatus(int playerId, StructureType type);
        Task<Response<int>> CollectResource(int playerId, int locId, float multiplier = 0);

        int GetMaxInfirmaryCapacity(IReadOnlyList<StructureInfos> structures);

        Task<Response> UpdateGate(int playerId, int hp);
        Task<Response<int>> RepairGate(int playerId);
        Task<Response<List<DataRequirement>>> RepairGateCost(int playerId);
        Task<Response<GateHpData>> GetGateHp(int playerId);
        Task<Response<bool>> GiftResource(int playerId, int toPlayerId, int food, int wood, int ore);
    }
}

using GameOfRevenge.Common.Interface.Model;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Inventory;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Models.Troop;
using GameOfRevenge.Common.Net;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameOfRevenge.Common.Interface.UserData
{
    public interface IBaseUserManager
    {
        Dictionary<int, UserStructureData> GetMultipleBuildings(UserStructureData structure);

        Task<Response<List<PlayerDataTable>>> GetAllPlayerData(int playerId);
        Task<Response<PlayerCompleteData>> GetFullPlayerData(int playerId);

        Task<Response<UserVIPDetails>> AddVIPPoints(int playerId, int points);
        Task<Response<UserVIPDetails>> ActivateVIPBoosts(int playerId);

        bool HasRequirements(IReadOnlyList<IReadOnlyDataRequirement> requirements, PlayerCompleteData playerData);
        bool HasRequirements(IReadOnlyList<IReadOnlyDataRequirement> requirements, PlayerCompleteData playerData, int count);
        bool HasResourceRequirements(IReadOnlyList<IReadOnlyDataRequirement> requirements, ResourcesList resourcess, int count);
        bool HasStructureRequirements(IReadOnlyList<IReadOnlyDataRequirement> requirements, List<StructureInfos> structures);
        bool HasActiveBoostRequirements(IReadOnlyList<IReadOnlyDataRequirement> requirements, List<UserRecordNewBoost> boosts);

        int GetInstantBuildCost(int timeLeft);

        Task<Response<RankingElement>> GetRanking(int playerId);
        Task<Response<List<RankingElement>>> GetRankings(long rankId = 0);
    }
}

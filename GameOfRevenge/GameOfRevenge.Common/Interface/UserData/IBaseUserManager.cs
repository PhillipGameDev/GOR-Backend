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
        UserResourceData PlayerDataToUserResourceData(PlayerDataTable playerData);
        UserResourceData PlayerDataToUserResourceData(PlayerDataTableUpdated playerDataUpdated);

        UserStructureData PlayerDataToUserStructureData(PlayerDataTable playerData);
        UserStructureData PlayerDataToUserStructureData(PlayerDataTableUpdated playerDataUpdated);

        UserTroopData PlayerDataToUserTroopData(PlayerDataTable playerData);
        UserTroopData PlayerDataToUserTroopData(PlayerDataTableUpdated playerDataUpdated);

        UserInventoryData PlayerDataToUserInventoryData(PlayerDataTable playerData);
        UserInventoryDataUpdated PlayerDataToUserInventoryData(PlayerDataTableUpdated playerDataUpdated);

        UserBoostData PlayerDataToUserBoostData(PlayerDataTable playerData);
        UserBoostData PlayerDataToUserBoostData(PlayerDataTableUpdated playerDataUpdated);

        UserHeroData PlayerDataToUserHeroData(PlayerDataTable playerData);
        UserHeroData PlayerDataToUserHeroData(PlayerDataTableUpdated playerDataUpdated);

        UserTechnologyData PlayerDataToUserTechnologyData(PlayerDataTable playerData);
        UserTechnologyData PlayerDataToUserTechnologyData(PlayerDataTableUpdated playerDataUpdated);

        PlayerDataTable UserResourceDataToPlayerData(UserResourceData playerData);
        PlayerDataTable UserStructureDataToPlayerData(UserStructureData playerData);
        PlayerDataTable UserTroopDataToPlayerData(UserTroopData playerData);
        PlayerDataTable UserInventoryDataToPlayerData(UserInventoryData playerData);
        PlayerDataTable UserTechnologyDataToPlayerData(UserTechnologyData playerData);

        Dictionary<int, UserStructureData> GetMultipleBuildings(UserStructureData structure);

        Task<Response<List<PlayerDataTable>>> GetAllPlayerData(int playerId);
        Task<Response<PlayerCompleteData>> GetFullPlayerData(int playerId);

        bool HasRequirements(IReadOnlyList<IReadOnlyDataRequirement> requirements, PlayerCompleteData playerData);
        bool HasRequirements(IReadOnlyList<IReadOnlyDataRequirement> requirements, PlayerCompleteData playerData, int count);
        bool HasResourceRequirements(IReadOnlyList<IReadOnlyDataRequirement> requirements, ResourcesList resourcess, int count);
        bool HasStructureRequirements(IReadOnlyList<IReadOnlyDataRequirement> requirements, List<StructureInfos> structures);
        bool HasActiveBoostRequirements(IReadOnlyList<IReadOnlyDataRequirement> requirements, List<UserRecordNewBoost> boosts);

        PlayerDataTable UserBoostDataToPlayerData(UserBoostData playerData);
//        UserBuffData PlayerDataToUserBuffData(PlayerDataTable playerData);

        int GetInstantBuildCost(int timeLeft);
    }
}

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
        UserStructureData PlayerDataToUserStructureData(PlayerDataTable playerData);
        UserTroopData PlayerDataToUserTroopData(PlayerDataTable playerData);
        UserInventoryData PlayerDataToUserInventoryData(PlayerDataTable playerData);
        UserTechnologyData PlayerDataToUserTechnologyData(PlayerDataTable playerData);

        PlayerDataTable UserResourceDataToPlayerData(UserResourceData playerData);
        PlayerDataTable UserStructureDataToPlayerData(UserStructureData playerData);
        PlayerDataTable UserTroopDataToPlayerData(UserTroopData playerData);
        PlayerDataTable UserInventoryDataToPlayerData(UserInventoryData playerData);
        PlayerDataTable UserTechnologyDataToPlayerData(UserTechnologyData playerData);

        Dictionary<int, UserStructureData> GetMultipleBuildings(UserStructureData structure);
        UserStructureData GetStructureDataAccLoc(UserStructureData structure, int locId);
        Task<Response<PlayerCompleteData>> GetPlayerData(int playerId);

        bool HasRequirements(IReadOnlyList<IReadOnlyDataRequirement> requirements, List<StructureInfos> structures, ResourcesList resourcess);
        bool HasRequirements(IReadOnlyList<IReadOnlyDataRequirement> requirements, List<StructureInfos> structures, ResourcesList resourcess, int count);
        bool HasRequirements(IReadOnlyList<IReadOnlyDataRequirement> requirements, ResourcesList resourcess, int count);
        bool HasRequirements(IReadOnlyList<IReadOnlyDataRequirement> requirements, List<StructureInfos> structures);

        PlayerDataTable UserBuffDataToPlayerData(UserBuffData playerData);
        UserBuffData PlayerDataToUserBuffData(PlayerDataTable playerData);

        int GetInstantBuildCost(int timeLeft);
    }
}

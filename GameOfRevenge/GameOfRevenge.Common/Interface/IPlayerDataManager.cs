using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Common.Interface
{
    public interface IPlayerDataManager
    {
        Task<Response<List<PlayerDataTable>>> GetAllPlayerData(int playerId, DataType type = DataType.Unknown);
        Task<Response<List<PlayerDataTable>>> GetAllPlayerData(int playerId, DataType type, int valueId);

        Task<Response> AddOrUpdateAllPlayerData(List<PlayerDataTable> playerDatas);
//        Task<Response<PlayerDataTable>> AddOrUpdatePlayerData(int playerId, DataType type, int valueId, string value);

        Task<Response<PlayerDataTableUpdated>> AddOrUpdatePlayerData(int playerId, DataType type, int valueId, string value, bool unique = true);

//        Task<Response<PlayerDataTableUpdated>> UpdatePlayerData(int playerId, DataType type, int valueId, string value);
        Task<Response<PlayerDataTableUpdated>> UpdatePlayerDataID(int playerId, long playerDataId, string value);

        Task<Response<PlayerDataTableUpdated>> IncrementPlayerData(int playerId, DataType type, int valueId, int value, bool log = true);
        Task<Response<PlayerDataTableUpdated>> SumPlayerData(int playerId, long playerDataId, int value);

        Task<Response> RemoveAllPlayerData(int playerId);
        Task<Response<PlayerDataTableUpdated>> RemovePlayerData(int playerId, DataType type, int valueId);
        Task<Response<PlayerDataTableUpdated>> RemovePlayerData(int playerId, long playerDataId);

        Task<Response<PlayerDataTable>> GetPlayerData(int playerId, DataType type, int valueId);
        Task<Response<PlayerDataTable>> GetPlayerDataById(long playerDataId);

        Task<Response<List<PlayerDataTable>>> AddPlayerResourceData(int playerId, int food, int wood, int ore, int gem, int gold);
        Task<Response<List<PlayerDataTable>>> RemovePlayerResourceData(int playerId, int food, int wood, int ore, int gem, int gold);
        Task<Response<List<PlayerDataTable>>> UpdatePlayerResourceData(int playerId, int food, int wood, int ore, int gem, int gold);

        Task<Response<List<StoredDataTable>>> GetAllPlayerStoredData(int playerId, int? locationId = null);
//        Task<Response<PlayerDataTable>> StoreResource(int playerId, int structureLocationId, int resId, int value);
        Task<Response<StoredPlayerDataTable>> StoreResource(int playerId, int locationId, int resId, int value);
        Task<Response<RankingElement>> GetRanking(int playerId);
        Task<Response<List<RankingElement>>> GetRankings(long rankId);
        Task<Response<List<PlayerDataTable>>> GetAllMarchingTroops();
    }
}

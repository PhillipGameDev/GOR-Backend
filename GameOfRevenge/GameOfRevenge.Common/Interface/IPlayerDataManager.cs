using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Common.Interface
{
    public interface IPlayerDataManager
    {
        Task<Response<List<PlayerDataTable>>> GetAllPlayerData(int playerId);
        Task<Response> AddOrUpdateAllPlayerData(List<PlayerDataTable> playerDatas);
        Task<Response<PlayerDataTable>> AddOrUpdatePlayerData(int playerId, DataType type, int valueId, string value);
        Task<Response<PlayerDataTable>> GetPlayerData(int playerId, DataType type, int valueId);
        Task<Response<PlayerDataTable>> GetPlayerDataById(int playerDataId);
        Task<Response> RemovePlayerData(int playerId, DataType type, int valueId);
        Task<Response> RemoveAllPlayerData(int playerId);
        Task<Response<List<PlayerDataTable>>> AddPlayerResourceData(int playerId, int food, int wood, int ore, int gem);
        Task<Response<List<PlayerDataTable>>> RemovePlayerResourceData(int playerId, int food, int wood, int ore, int gem);
        Task<Response<List<PlayerDataTable>>> UpdatePlayerResourceData(int playerId, int food, int wood, int ore, int gem);

        Task<Response<List<StoredDataTable>>> GetAllPlayerStoredData(int playerId, int structureLocationId);
//        Task<Response<PlayerDataTable>> StoreResource(int playerId, int structureLocationId, int resId, int value);
        Task<Response> StoreResource(int playerId, int structureLocationId, int valueId, int value);
    }
}

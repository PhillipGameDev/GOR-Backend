using System.Threading.Tasks;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Models.Boost;
using GameOfRevenge.Common.Models.PlayerData;
using System.Collections.Generic;
using GameOfRevenge.Common.Models;

namespace GameOfRevenge.Common.Interface.UserData
{
    public interface IUserActiveBoostsManager
    {
        Task<Response<List<UserRecordNewBoost>>> GetAllPlayerActiveBoostData(int playerId);

        Task<Response<UserRecordNewBoost>> AddBoost(int playerId, NewBoostType type);
//        Task<Response<UserRecordNewBoost>> AddBoost(int playerId, NewBoostType type, int count);
        Task<Response<UserBoostData>> RemoveBoost(int playerId, NewBoostType itemId);
        Task<Response<UserBoostData>> RemoveBoost(int playerId, NewBoostType itemId, int count);
    }
}

using System.Threading.Tasks;
using System.Collections.Generic;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Boost;
using GameOfRevenge.Common.Models.PlayerData;

namespace GameOfRevenge.Common.Interface.UserData
{
    public interface IUserActiveBoostsManager
    {
        Task<Response<List<UserRecordNewBoost>>> GetAllPlayerActiveBoostData(int playerId);

        Task<Response<BoostActivatedResponse>> ActivateBoost(int playerId, NewBoostType type);
        Task<Response<UserBoostData>> RemoveBoost(int playerId, NewBoostType itemId);
        Task<Response<UserBoostData>> RemoveBoost(int playerId, NewBoostType itemId, int count);
    }
}

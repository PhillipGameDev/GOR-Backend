using System.Threading.Tasks;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Models.Boost;
using GameOfRevenge.Common.Models.PlayerData;

namespace GameOfRevenge.Common.Interface.UserData
{
    public interface IUserActiveBoostsManager
    {
        Task<Response<UserBoostData>> AddBoost(int playerId, BoostType itemId);
        Task<Response<UserBoostData>> AddBoost(int playerId, BoostType itemId, int count);
        Task<Response<UserBoostData>> RemoveBoost(int playerId, BoostType itemId);
        Task<Response<UserBoostData>> RemoveBoost(int playerId, BoostType itemId, int count);
    }
}

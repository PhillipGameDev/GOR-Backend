using System.Threading.Tasks;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Common.Interface
{
    public interface IAccountManager
    {
        Task<Response<Player>> TryLoginOrRegister(string identifier, string name, bool accepted, int version);
        Task<Response<Player>> ChangeName(int playerId, string name);
        Task<Response<Player>> GetAccountInfo(int playerId);
        Task<Response<Player>> GetAccountInfo(string identifier);
        Task<Response<PlayerTutorialData>> GetTutorialInfo(string identifier);
        Task<Response<PlayerTutorialData>> UpdateTutorialInfo(string identifier, string playerData, bool isComplete);
    }
}

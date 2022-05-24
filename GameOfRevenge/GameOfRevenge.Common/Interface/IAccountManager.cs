using System.Threading.Tasks;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Common.Interface
{
    public interface IAccountManager
    {
        Task<Response<Player>> GetAccountInfo(int userId);
        Task<Response<Player>> GetAccountInfo(string identifier);
        Task<Response<PlayerTutorialData>> GetTutorialInfo(string identifier);
        Task<Response<Player>> TryLoginOrRegister(string identifier, string name, bool accepted);
        Task<Response<PlayerTutorialData>> UpdateTutorialInfo(string identifier, string playerData, bool isComplete);
    }
}

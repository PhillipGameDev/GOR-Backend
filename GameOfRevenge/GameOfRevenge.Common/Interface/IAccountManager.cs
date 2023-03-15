using System.Threading.Tasks;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Quest.Template;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Common.Interface
{
    public interface IAccountManager
    {
        Task<Response<Player>> TryLoginOrRegister(string identifier, string name, bool accepted, int version);
        Task<Response<Player>> SetProperties(int playerId, string name = null, int? vipPoints = null, bool? terms = null, int? worldTileId = null);
        Task<Response<string[]>> ChangeName(int playerId, string name);

        Task<Response> Debug(int playerId, int dip);
        Task<Response<PlayerInfo>> GetAccountInfo(int playerId);
        Task<Response<PlayerInfo>> GetAccountInfo(string identifier);
        Task<Response<PlayerTutorialData>> GetTutorialInfo(string playerId);
        Task<Response<PlayerTutorialData>> UpdateTutorialInfo(string playerId, string playerData, bool isComplete);
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Common.Interface
{
    public interface IAccountManager
    {
        Task<Response<Player>> TryLoginOrRegister(string identifier, bool accepted, int version);
        PlayerID AddPlayerToZone(int playerId, int zoneSize, List<PlayerID> list);

        Task<Response<Player>> SetProperties(int playerId, string firebaseId = null, bool? terms = null, int? worldTileId = null, string name = null, int? vipPoints = null);

        Task<Response<string[]>> ChangeName(int playerId, string name);

        Task<Response> Debug(int playerId, int dip);
        Task<Response<List<PlayerID>>> GetAllPlayerIDs(int? playerId = null, int length = 0, bool includeTileId = false);
        Task<Response<PlayerInfo>> GetAccountInfo(int playerId);
        Task<Response<PlayerInfo>> GetAccountInfo(string identifier);
        Task<Response<PlayerTutorialData>> GetTutorialInfo(string playerId);
        Task<Response<PlayerTutorialData>> UpdateTutorialInfo(string playerId, string playerData, bool isComplete);
    }
}

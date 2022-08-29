using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Hero;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Common.Models.Troop;
using GameOfRevenge.Common.Net;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameOfRevenge.Common.Interface.UserData
{
    public interface IUserHeroManager : IBaseUserManager
    {
        Task<Response<UserHeroDataList>> GetHeroDataList(int playerId, HeroType type);
        Task<Response> UnlockHero(int playerId, HeroType heroType);
        Task<Response<int>> AddHeroWarPoints(int playerId, HeroType heroType, int? value);

        Task<Response<UserHeroDetails>> GetHeroPoint(int playerId, int heroId);
        Task<Response<UserHeroDetails>> SaveHeroPoint(int playerId, int heroId, int warPoints);
    }
}

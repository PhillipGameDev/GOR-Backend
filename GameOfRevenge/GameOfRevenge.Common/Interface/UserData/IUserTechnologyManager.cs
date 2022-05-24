using System.Threading.Tasks;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Common.Interface.UserData
{
    public interface IUserTechnologyManager
    {
        Task<Response<TechnologyInfos>> UpgradeTechnology(int playerId, TechnologyType type);
        Task<Response<TechnologyInfos>> UpgradeTechnology(int playerId, int id);
    }
}

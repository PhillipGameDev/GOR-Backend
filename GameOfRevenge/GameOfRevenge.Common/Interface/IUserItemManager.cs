using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Quest;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Common.Interface
{
    public interface IUserItemManager
    {
        Task<Response<List<PlayerItemDataTable>>> GetUserAllItems(int playerId);
        Task<Response<PlayerDataTableUpdated>> ConsumeItem(int playerId, long playerDataId, int itemCount, string context = null);
    }
}

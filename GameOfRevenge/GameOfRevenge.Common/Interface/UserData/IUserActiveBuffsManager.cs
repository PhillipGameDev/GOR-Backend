using System.Threading.Tasks;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Models.Inventory;
using GameOfRevenge.Common.Models.PlayerData;

namespace GameOfRevenge.Common.Interface.UserData
{
    public interface IUserActiveBuffsManager
    {
        Task<Response<UserBuffData>> AddBuff(int playerId, InventoryItemType itemId);
        Task<Response<UserBuffData>> AddBuff(int playerId, InventoryItemType itemId, int count);
        Task<Response<UserBuffData>> RemoveBuff(int playerId, InventoryItemType itemId);
        Task<Response<UserBuffData>> RemoveBuff(int playerId, InventoryItemType itemId, int count);
    }
}

using GameOfRevenge.Common.Models.Inventory;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Common.Net;
using System.Threading.Tasks;

namespace GameOfRevenge.Common.Interface.UserData
{
    public interface IUserInventoryManager : IBaseUserManager
    {
        Task<Response<UserInventoryData>> BuyItem(int playerId, InventoryItemType itemId, int count);
        Task<Response<UserInventoryData>> UseItem(int playerId, InventoryItemType itemId, int count);
    }
}

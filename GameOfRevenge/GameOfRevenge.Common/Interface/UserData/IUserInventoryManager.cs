using GameOfRevenge.Common.Models.Inventory;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Common.Net;
using System.Threading.Tasks;

namespace GameOfRevenge.Common.Interface.UserData
{
    public interface IUserInventoryManager : IBaseUserManager
    {
        Task<Response<UserInventoryDataUpdated>> UpdateItem(int playerId, InventoryItemType itemType, long playerDataId, int value);

        Task<Response<UserInventoryData>> AddUniqueItem(int playerId, InventoryItemType itemType, int value = 1);
        Task<Response<UserInventoryData>> AddItem(int playerId, InventoryItemType itemType, int value = 1, bool unique = false);

        Task<Response<UserInventoryDataUpdated>> SumItem(int playerId, InventoryItemType itemType, long playerDataId, int value);

        Task<Response<UserInventoryData>> RemoveItem(int playerId, InventoryItemType itemType, long playerDataId);

//        Task<Response<UserInventoryData>> BuyItem(int playerId, InventoryItemType itemId, int count);
        Task<Response<UserInventoryData>> UseItem(int playerId, InventoryItemType itemId, int count);
    }
}

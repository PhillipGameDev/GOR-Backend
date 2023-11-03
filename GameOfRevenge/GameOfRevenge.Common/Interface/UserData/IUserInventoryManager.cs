using GameOfRevenge.Common.Models.Inventory;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Common.Net;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameOfRevenge.Common.Interface.UserData
{
    public interface IUserInventoryManager : IBaseUserManager
    {
        Task<Response<List<InventoryUserDataTable>>> GetAllUserInventory(int playerId);
        Task<Response> UpdateInventoryOrder(int playerId, int itemId, int value);
        Task<Response<InventoryUserDataTable>> UpgradeInventory(int playerId, int inventoryId, int level, DateTime startTime);
        Task<Response> AddNewInventory(int playerId, int inventoryId);
        Task<Response<UserInventoryDataUpdated>> UpdateItem(int playerId, InventoryItemType itemType, long playerDataId, int value);

        Task<Response<UserInventoryData>> AddUniqueItem(int playerId, InventoryItemType itemType, int value = 1);
        Task<Response<UserInventoryData>> AddItem(int playerId, InventoryItemType itemType, int value = 1, bool unique = false);

        Task<Response<UserInventoryDataUpdated>> AddItem(int playerId, InventoryItemType itemType, long playerDataId, int value);

        Task<Response<UserInventoryData>> RemoveItem(int playerId, InventoryItemType itemType, long playerDataId);

//        Task<Response<UserInventoryData>> BuyItem(int playerId, InventoryItemType itemId, int count);
        Task<Response<UserInventoryData>> UseItem(int playerId, InventoryItemType itemId, int count);
    }
}

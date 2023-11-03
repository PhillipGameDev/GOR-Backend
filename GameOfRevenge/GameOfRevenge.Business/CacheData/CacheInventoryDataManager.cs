using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameOfRevenge.Business.Manager;
using GameOfRevenge.Business.Manager.GameDef;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Inventory;

namespace GameOfRevenge.Business.CacheData
{
    public static class CacheInventoryDataManager
    {
        public const string InventoryNotExist = "Inventory item does not exist";

        private static bool isLoaded = false;

        private static List<InventoryTable> inventoryItems = null;
        private static List<InventoryDataTable> inventoryData = null;

        public static bool IsLoaded { get => isLoaded && inventoryItems != null && inventoryData != null; }

        public static IReadOnlyList<IReadOnlyInventoryTable> AllInventoryItems { get { CheckCacheMemory(); return inventoryItems.ToList(); } }
        public static IReadOnlyList<IReadOnlyInventoryDataTable> AllInventoryData { get { CheckCacheMemory(); return inventoryData.ToList(); } }

        public static IReadOnlyInventoryTable GetFullInventoryItemData(int itemId)
        {
            var item = AllInventoryItems.FirstOrDefault(x => (x.Id == itemId));
            if (item == null) throw new CacheDataNotExistExecption(InventoryNotExist);
            else return item;
        }
        public static IReadOnlyInventoryTable GetFullInventoryItemData(InventoryItemType itemType)
        {
            var item = AllInventoryItems.FirstOrDefault(x => (x.Code == itemType));
            if (item == null) throw new CacheDataNotExistExecption(InventoryNotExist);
            else return item;
        }

        #region Cache Check, Load and Clear

        public static async Task LoadCacheMemoryAsync()
        {
            var inventoryManager = new InventoryManager();

            var itemsResp = await inventoryManager.GetAllInventoryItems();
            if (!itemsResp.IsSuccess)
            {
                ClearCache();
                throw new CacheDataNotExistExecption(itemsResp.Message);
            }
            inventoryItems = itemsResp.Data;

            var dataResp = await inventoryManager.GetAllInventoryData();
            if (!dataResp.IsSuccess)
            {
                ClearCache();
                throw new CacheDataNotExistExecption(dataResp.Message);
            }
            inventoryData = dataResp.Data;

            isLoaded = true;
        }

        public static void CheckCacheMemory()
        {
            if (isLoaded) return;

            var tsk = LoadCacheMemoryAsync();
            tsk.Wait();
        }

/*        public static async Task CheckLoadCacheMemoryAsync()
        {
            if (isLoaded) return;

            await LoadCacheMemoryAsync();
        }*/

        public static void ClearCache()
        {
            isLoaded = false;

            if (inventoryItems != null) { inventoryItems.Clear(); inventoryItems = null; }
            if (inventoryData != null) { inventoryData.Clear(); inventoryData = null; }
        }
        #endregion
    }
}

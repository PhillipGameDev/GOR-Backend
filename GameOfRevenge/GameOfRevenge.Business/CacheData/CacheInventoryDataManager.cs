using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameOfRevenge.Business.Manager;
using GameOfRevenge.Business.Manager.GameDef;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models.Inventory;

namespace GameOfRevenge.Business.CacheData
{
    public static class CacheInventoryDataManager
    {
        public const string InventoryNotExist = "Inventory item does not exist";

        private static bool isLoaded = false;
        private static List<InventoryDataTable> InventoryItems = null;
        private static List<InventoryItemType> itemTypes;

        public static bool IsLoaded { get => isLoaded && InventoryItems != null; }
        public static IReadOnlyList<IReadOnlyInventoryDataTable> ItemList
        {
            get
            {
                if (InventoryItems == null) CheckCacheMemory();
                return InventoryItems?.ToList();
            }
        }
        public static IReadOnlyList<InventoryItemType> ItemTypes { get { CheckCacheMemory(); return itemTypes; } }

        public static bool IsItemTypeValid(InventoryItemType itemType) => ItemTypes.Contains(itemType);

        public static IReadOnlyInventoryDataTable GetFullInventoryItemData(int itemId)
        {
            var item = ItemList.FirstOrDefault(x => (x.Id == itemId));
            if (item == null) throw new CacheDataNotExistExecption(InventoryNotExist);
            else return item;
        }
        public static IReadOnlyInventoryDataTable GetFullInventoryItemData(InventoryItemType itemType)
        {
            var item = ItemList.FirstOrDefault(x => (x.Code == itemType));
            if (item == null) throw new CacheDataNotExistExecption(InventoryNotExist);
            else return item;
        }

        #region Cache Check, Load and Clear

        public static async Task LoadCacheMemoryAsync()
        {
            ClearCache();

            var resManager = new InventoryManager();
            var response = await resManager.GetAllInventoryItems();// GetAllInventoryItemDatas();

            if (response.IsSuccess)
            {
                InventoryItems = response.Data;

                itemTypes = new List<InventoryItemType>();
                foreach (var item in response.Data)
                {
                    if (itemTypes.Contains(item.Code)) continue;
                    if (item.Code == InventoryItemType.Unknown) continue;
                    itemTypes.Add(item.Code);
                }
                isLoaded = true;
            }
            else
            {
                ClearCache();
                throw new CacheDataNotExistExecption(response.Message);
            }
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

            if (InventoryItems != null)
            {
                InventoryItems.Clear();
                InventoryItems = null;
            }

            if (itemTypes != null)
            {
                itemTypes.Clear();
                itemTypes = null;
            }
        }
        #endregion
    }
}

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
        private static List<InventoryDataRequirementRel> inventoryInfo = null;
        private static List<InventoryItemType> itemTypes;
        private static List<BuffItemRel> buffItemRel;

        public static bool IsLoaded { get => isLoaded && inventoryInfo != null; }
        public static IReadOnlyList<IReadOnlyInventoryDataRequirementRel> ItemInfos { get { CheckLoadCacheMemory(); return inventoryInfo.ToList(); } }
        public static IReadOnlyList<InventoryItemType> ItemTypes { get { CheckLoadCacheMemory(); return itemTypes; } }
        public static IReadOnlyList<IReadOnlyBuffItemRel> BuffItemRelations { get { CheckLoadCacheMemory(); return buffItemRel; } }

        public static IReadOnlyInventoryDataRequirementRel GetFullInventoryItemData(int itemId)
        {
            var item = ItemInfos.Where(x => x.Info.Id == itemId).FirstOrDefault();
            if (item == null) throw new CacheDataNotExistExecption(InventoryNotExist);
            else return item;
        }
        public static IReadOnlyInventoryDataRequirementRel GetFullInventoryItemData(InventoryItemType itemType)
        {
            var item = ItemInfos.Where(x => x.Info.Code == itemType).FirstOrDefault();
            if (item == null) throw new CacheDataNotExistExecption(InventoryNotExist);
            else return item;
        }


        #region Cache Check, Load and Clear
        private static List<BuffItemRel> LoadBuffs()
        {
            try
            {
                var buffs = Enum.GetNames(typeof(BuffType));
                var invs = Enum.GetNames(typeof(InventoryItemType));
                var datas = new List<BuffItemRel>();

                for (int i = 0; i < invs.Length; i++)
                {
                    for (int j = 0; j < buffs.Length; j++)
                    {
                        if (buffs[j].Equals(invs[i]))
                        {
                            var data = new BuffItemRel()
                            {
                                InventoryType = (InventoryItemType)i,
                                BuffType = (BuffType)j
                            };

                            datas.Add(data);
                        }
                    }
                }

                return datas;
            }
            catch (Exception ex)
            {
                ClearCache();
                throw new CacheDataNotExistExecption(ex.Message, ex);
            }
        }
        public static async Task LoadCacheMemoryAsync()
        {
            ClearCache();

            var resManager = new InventoryManager();
            var response = await resManager.GetAllInventoryItemDatas();

            if (response.IsSuccess)
            {
                buffItemRel = LoadBuffs();
                itemTypes = new List<InventoryItemType>();
                inventoryInfo = response.Data;
                foreach (var item in response.Data)
                {
                    if (itemTypes.Contains(item.Info.Code)) continue;
                    if (item.Info.Code == InventoryItemType.Other) continue;
                    itemTypes.Add(item.Info.Code);
                }
                isLoaded = true;
            }
            else
            {
                ClearCache();
                throw new CacheDataNotExistExecption(response.Message);
            }
        }
        public static void LoadCacheMemory()
        {
            var tsk = LoadCacheMemoryAsync();
            tsk.Wait();
        }
        public static void CheckLoadCacheMemory()
        {
            if (isLoaded) return;
            else LoadCacheMemory();
        }
        public static async Task CheckLoadCacheMemoryAsync()
        {
            if (isLoaded) return;
            else await LoadCacheMemoryAsync();
        }
        public static void ClearCache()
        {
            isLoaded = false;

            if (buffItemRel != null)
            {
                buffItemRel.Clear();
                buffItemRel = null;
            }

            if (inventoryInfo != null)
            {
                inventoryInfo.Clear();
                inventoryInfo = null;
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

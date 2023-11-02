using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Quest;
using GameOfRevenge.Business.Manager.GameDef;

namespace GameOfRevenge.Business.CacheData
{
    public static class CacheItemManager
    {

        private static bool isLoaded = false;

        public static bool IsLoaded { get => isLoaded; }

        private static List<ItemTable> allItems = null;
        public static IReadOnlyList<ItemTable> AllItems { get { CheckLoadCacheMemory(); return allItems.ToList(); } }

        #region Cache Check, Load and Clear
        public static async Task LoadCacheMemoryAsync()
        {
            var itemManager = new ItemManager();

            var itemResp = await itemManager.GetAllItems();
            if (!itemResp.IsSuccess)
            {
                ClearCache();
                throw new CacheDataNotExistExecption(itemResp.Message);
            }

            allItems = itemResp.Data;

            isLoaded = true;
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

            if (allItems != null) { allItems.Clear(); allItems = null; }
        }
#endregion
    }
}

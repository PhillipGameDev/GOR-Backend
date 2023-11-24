using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Quest;
using GameOfRevenge.Business.Manager.GameDef;
using GameOfRevenge.Common.Models.Monster;

namespace GameOfRevenge.Business.CacheData
{
    public static class CacheMonsterManager
    {

        private static bool isLoaded = false;

        public static bool IsLoaded { get => isLoaded; }

        private static List<MonsterDataTable> allItems = null;
        public static IReadOnlyList<MonsterDataTable> AllItems { get { CheckLoadCacheMemory(); return allItems.ToList(); } }

        #region Cache Check, Load and Clear
        public static async Task LoadCacheMemoryAsync()
        {
            var monsterManager = new MonsterManager();

            var itemResp = await monsterManager.GetAllMonsterData();
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

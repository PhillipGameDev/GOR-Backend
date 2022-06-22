using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameOfRevenge.Business.Manager.GameDef;
using GameOfRevenge.Common.Models;

namespace GameOfRevenge.Business.CacheData
{
    public static class CacheProductDataManager
    {
        public const string ProductNotExist = "Product item does not exist";

        private static bool isLoaded = false;
        private static List<MarketProductTable> Products = null;

        public static bool IsLoaded { get => isLoaded && Products != null; }
        public static IReadOnlyList<IReadOnlyProductTable> ProductList
        {
            get
            {
                if (Products == null) LoadCacheMemory();
                return Products.ToList();
            }
        }

#region Cache Check, Load and Clear
        public static async Task LoadCacheMemoryAsync()
        {
            ClearCache();

            var resManager = new MarketManager();
            var response = await resManager.GetAllProducts();

            if (response.IsSuccess)
            {
                Products = response.Data;
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

            if (Products != null)
            {
                Products.Clear();
                Products = null;
            }
        }
#endregion
    }
}

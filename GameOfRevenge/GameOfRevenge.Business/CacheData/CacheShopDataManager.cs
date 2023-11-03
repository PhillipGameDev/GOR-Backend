using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Quest;
using GameOfRevenge.Business.Manager.GameDef;

namespace GameOfRevenge.Business.CacheData
{
    public static class CacheShopDataManager
    {

        private static bool isLoaded = false;

        public static bool IsLoaded { get => isLoaded; }

        private static List<ShopCategory> allShopCategories = null;
        public static IReadOnlyList<ShopCategory> AllShopCategories { get { CheckLoadCacheMemory(); return allShopCategories.ToList(); } }

        private static List<ShopItemTable> allShopItems = null;
        public static IReadOnlyList<ShopItemTable> AllShopItems { get { CheckLoadCacheMemory(); return allShopItems.ToList(); } }

        private static List<PackageList> allPackageLists = null;
        public static IReadOnlyList<PackageList> AllPackageLists { get { CheckLoadCacheMemory(); return allPackageLists.ToList(); } }

        private static List<PackageItemTable> allPackageItems = null;
        public static IReadOnlyList<PackageItemTable> AllPackageItems { get { CheckLoadCacheMemory(); return allPackageItems.ToList(); } }

        #region Cache Check, Load and Clear
        public static async Task LoadCacheMemoryAsync()
        {

            var shopManager = new ShopManager();

            var shopCategoryResp = await shopManager.GetAllShopCategories();
            if (!shopCategoryResp.IsSuccess)
            {
                ClearCache();
                throw new CacheDataNotExistExecption(shopCategoryResp.Message);
            }

            var shopItemResp = await shopManager.GetAllShopItems();
            if (!shopItemResp.IsSuccess)
            {
                ClearCache();
                throw new CacheDataNotExistExecption(shopItemResp.Message);
            }

            allShopItems = shopItemResp.Data;

            var shopCategories = new List<ShopCategory>();
            foreach (var category in shopCategoryResp.Data)
            {
                shopCategories.Add(new ShopCategory()
                {
                    CategoryId = category.Id,
                    Name = category.Name,
                    Items = shopItemResp.Data.FindAll(e => e.CategoryId == category.Id)
                });                
            }

            allShopCategories = shopCategories;

            var packageListsResp = await shopManager.GetAllPackageLists();
            if (!packageListsResp.IsSuccess)
            {
                ClearCache();
                throw new CacheDataNotExistExecption(packageListsResp.Message);
            }

            var packageItemsResp = await shopManager.GetAllPackageItems();
            if (!packageItemsResp.IsSuccess)
            {
                ClearCache();
                throw new CacheDataNotExistExecption(packageItemsResp.Message);
            }

            allPackageItems = packageItemsResp.Data;

            var packageLists = new List<PackageList>();
            foreach (var packageList in packageListsResp.Data)
            {
                packageLists.Add(new PackageList()
                {
                    ListId = packageList.Id,
                    Name = packageList.Name,
                    Cost = packageList.Cost,
                    Items = packageItemsResp.Data.FindAll(e => e.PackageId == packageList.Id)
                });
            }

            allPackageLists = packageLists;

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

            if (allShopCategories != null) { allShopCategories.Clear(); allShopCategories = null; }
            if (allShopItems != null) { allShopItems.Clear(); allShopItems = null; }
            if (allPackageLists != null) { allPackageLists.Clear(); allPackageLists = null; }
            if (allPackageItems != null) { allPackageItems.Clear(); allPackageItems = null; }
        }
#endregion
    }
}

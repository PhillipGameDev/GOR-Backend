using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Quest;
using GameOfRevenge.Business.Manager.GameDef;

namespace GameOfRevenge.Business.CacheData
{
    public static class CacheProductDataManager
    {
        public const string ProductNotExist = "Product item does not exist";

        private static bool isLoaded = false;
        private static List<MarketProductTable> Products = null;

        public static bool IsLoaded { get => isLoaded && Products != null; }

        private static List<ProductPackage> allPackageRewards = null;
        public static IReadOnlyList<ProductPackage> AllPackageRewards { get { CheckLoadCacheMemory(); return allPackageRewards.ToList(); } }

        public static IReadOnlyList<IReadOnlyProductTable> ProductList
        {
            get
            {
                if (Products == null) LoadCacheMemory();
                return Products?.ToList();
            }
        }

        public static IReadOnlyList<ProductPackage> GetPackageProducts(int playerId = 0)
        {
            var packageProducts = AllPackageRewards;

            return packageProducts;
        }

/*        public static IReadOnlyList<ProductPackage> GetPackages()
        {
            var packageProducts = AllPackageRewards;

            return packageProducts;
        }*/

#region Cache Check, Load and Clear
        public static async Task LoadCacheMemoryAsync()
        {
            var marketManager = new MarketManager();
            var packagesResp = await marketManager.GetStorePackages(true);
            if (!packagesResp.IsSuccess)
            {
                ClearCache();
                throw new CacheDataNotExistExecption(packagesResp.Message);
            }

            var availableIAPs = packagesResp.Data;
            var packageProducts = new List<ProductPackage>();
            var allTaskRewards = CacheQuestDataManager.AllQuestRewards;
            foreach (var package in availableIAPs)
            {
                IReadOnlyQuestRewardRelData data = allTaskRewards.FirstOrDefault(x => (x.Quest != null) &&
                                            (x.Quest.QuestGroup == QuestGroupType.PRODUCT_PACK) &&
                                            (x.Quest.QuestId == package.QuestId));
                if (data == null) continue;

                var prod = new ProductPackage()
                {
                    ProductId = package.ProductId,

                    PackageId = package.PackageId,
                    QuestId = package.QuestId,
                    Cost = package.Cost,
                    Rewards = data.Rewards
                };
                packageProducts.Add(prod);
            }
            allPackageRewards = packageProducts;

            var response = await marketManager.GetAllProducts();
            if (!response.IsSuccess)
            {
                ClearCache();
                throw new CacheDataNotExistExecption(response.Message);
            }

            Products = response.Data;
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

            if (Products != null)
            {
                Products.Clear();
                Products = null;
            }
        }
#endregion
    }
}

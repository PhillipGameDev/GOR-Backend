using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameOfRevenge.Business.Manager.GameDef;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Quest;

namespace GameOfRevenge.Business.CacheData
{
    public static class CacheProductDataManager
    {
        public const string ProductNotExist = "Product item does not exist";

        private static bool isLoaded = false;
        private static List<MarketProductTable> Products = null;

        public static bool IsLoaded { get => isLoaded && Products != null; }

        private static List<IAPProduct> allIAPProductRewards = null;
        public static IReadOnlyList<IAPProduct> AllIAPProductRewards { get { CheckLoadCacheMemory(); return allIAPProductRewards.ToList(); } }

        public static IReadOnlyList<IReadOnlyProductTable> ProductList
        {
            get
            {
                if (Products == null) LoadCacheMemory();
                return Products?.ToList();
            }
        }

        private static IAPProduct[] availableIAPs = new IAPProduct[]
        {
            new IAPProduct("p_a001"), new IAPProduct("p_a002"), new IAPProduct("p_a003"),
            new IAPProduct("p_a004"), new IAPProduct("p_a005"), new IAPProduct("p_a006"),
            new IAPProduct("r_a001"), new IAPProduct("r_a002"), new IAPProduct("r_a003")
        };

        public static IReadOnlyList<IAPProduct> GetIAPProducts(int playerId = 0)
        {
            var iapProducts = AllIAPProductRewards;
/*            var iapProducts = new List<IAPProduct>()
            {
                new IAPProduct( "p_a001", "Pack 1", "Pack 1 description", new List<DataReward>()
                {
                    new DataReward{ DataType = DataType.Resource, ValueId = (int)ResourceType.Food, Value = 1000, Count = 5 }
                }),
                new IAPProduct( "r_a001", "Resource Pack 1", "Resource Pack 1 description", new List<DataReward>()
                {
                    new DataReward(){ DataType = DataType.Resource, ValueId = (int)ResourceType.Food, Value = 10000, Count = 5 },
                    new DataReward(){ DataType = DataType.Resource, ValueId = (int)ResourceType.Wood, Value = 10000, Count = 5 },
                    new DataReward(){ DataType = DataType.Resource, ValueId = (int)ResourceType.Gems, Value = 1000, Count = 1 }
                })
            };*/

            return iapProducts;
        }

#region Cache Check, Load and Clear
        public static async Task LoadCacheMemoryAsync()
        {
            ClearCache();

            var iapProducts = new List<IAPProduct>();
            var allTaskRewards = CacheQuestDataManager.AllQuestRewards;
            foreach (var product in availableIAPs)
            {
                var data = allTaskRewards.FirstOrDefault(x => (x.Quest != null) && (x.Quest.QuestGroup == QuestGroupType.PRODUCT_PACK) && (x.Quest.DataString == product.ProductId));
                if (data == null) continue;

                var prod = new IAPProduct(product.ProductId, product.Name, product.Description, data.Rewards);
                iapProducts.Add(prod);
            }
            allIAPProductRewards = iapProducts;

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

    public class IAPProduct
    {
        public string ProductId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public IReadOnlyList<IReadOnlyDataReward> Rewards { get; private set; }

        public IAPProduct(string productId)
        {
            ProductId = productId;
        }

        public IAPProduct(string productId, string name, string description, IReadOnlyList<IReadOnlyDataReward> rewards)
        {
            ProductId = productId;
            Name = name;
            Description = description;
            Rewards = rewards;
        }
    }
}

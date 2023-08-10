using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameOfRevenge.Common.Models
{
    public class ProductPackage
    {
        //TODO: remove this on future versions, we don't use IAP id for products
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int PackageId { get; set; }
        public int QuestId { get; set; }
        public int Cost { get; set; }
        [JsonIgnore]
        public bool Active { get; set; }
        public IReadOnlyList<IReadOnlyDataReward> Rewards { get; set; }

        public ProductPackage()
        {
        }
    }

    public class IAPProduct
    {
        public string ProductId { get; set; }
        public int Value { get; set; }

        public IAPProduct(string productId, int value)
        {
            ProductId = productId;
            Value = value;
        }
    }
}

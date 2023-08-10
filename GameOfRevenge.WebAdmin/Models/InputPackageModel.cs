using System.Collections.Generic;
using GameOfRevenge.Common.Models;

namespace GameOfRevenge.WebAdmin.Models
{
    public class InputPackageModel
    {
        public ProductPackage Package { get; set; }
        public int PackageId { get; set; }
        public string PackageName { get; set; }
        public int PackageCost { get; set; }
        public bool PackageActive { get; set; }
        public IReadOnlyList<IReadOnlyDataReward> Rewards { get; set; }

        public string Description { get; set; }
        public int Value { get; set; }

        public InputPackageModel()
        {
        }

        public InputPackageModel(ProductPackage product, string name, bool active)
        {
            Package = product;
            PackageId = product.PackageId;
            PackageCost = product.Cost;
            PackageActive = active;
            Rewards = product.Rewards;
            PackageName = name;
            PackageActive = active;
        }
    }
}

using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameOfRevenge.Common.Models
{
    public class ShopCategory
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public IReadOnlyList<ShopItemTable> Items { get; set; }

        public ShopCategory()
        {
        }
    }
}

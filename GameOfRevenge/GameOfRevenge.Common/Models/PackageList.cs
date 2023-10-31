using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameOfRevenge.Common.Models
{
    public class PackageList
    {
        public int ListId { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }
        public IReadOnlyList<PackageItemTable> Items { get; set; }

        public PackageList()
        {
        }
    }
}

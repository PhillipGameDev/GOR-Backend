using GameOfRevenge.Common.Models.Inventory;

namespace GameOfRevenge.Common.Models
{
    public class UserItemDetails
    {
        public long id { get; set; }
        public InventoryItemType ItemType { get; set; }
        public int Level { get; set; }
    }
}

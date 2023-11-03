using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GameOfRevenge.Common.Models.Inventory
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum InventoryItemType
    {
        Unknown = 0,
        Helmet = 1,
        Weapon = 2,
        Shield = 3,
        Belt = 4,
        Boots = 5,
        Accessory = 6,
        Armor = 7,
        Boots2 = 8,
        Gloves = 9,
        Sword = 10,
    }
}

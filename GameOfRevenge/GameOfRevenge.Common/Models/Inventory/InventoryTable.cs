using System;
using System.Data;
using GameOfRevenge.Common.Helper;
using GameOfRevenge.Common.Interface.Model.Table;
using GameOfRevenge.Common.Models.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GameOfRevenge.Common.Models.Inventory
{
    public interface IReadOnlyInventoryTable : IReadOnlyBaseRefEnumTable<InventoryItemType>
    {
        [JsonProperty("ItemType")]
        new InventoryItemType Code { get; }

        [JsonProperty(Order = 1)]
        [JsonConverter(typeof(StringEnumConverter))]
        RarityType Rarity { get; }
    }

    public class InventoryTable : BaseTable, IBaseRefEnumTable<InventoryItemType>,
                                        IReadOnlyInventoryTable
    {
        public int Id { get; set; }
        [JsonProperty("ItemType")]
        public InventoryItemType Code { get; set; }
        public string Name { get; set; }

        public RarityType Rarity { get; set; }


        // [JsonIgnore]

        public override void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            Id = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Code = reader.GetValue(index) == DBNull.Value ? InventoryItemType.Unknown : reader.GetString(index).ToEnum<InventoryItemType>(); index++;
            Name = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            Rarity = reader.GetValue(index) == DBNull.Value ? RarityType.Common : reader.GetString(index).ToEnum<RarityType>();
        }
    }
}

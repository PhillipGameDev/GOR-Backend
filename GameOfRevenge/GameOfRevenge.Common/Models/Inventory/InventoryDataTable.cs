using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using GameOfRevenge.Common.Helper;
using GameOfRevenge.Common.Interface.Model.Table;
using GameOfRevenge.Common.Models.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GameOfRevenge.Common.Models.Inventory
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RawResourceType
    {
        [EnumMember(Value = "Steel")]
        Red,
        [EnumMember(Value = "Stone")]
        Green,
        [EnumMember(Value = "Ruby")]
        Blue
    }

    public class InventoryRequirement
    {
        public RawResourceType Type { get; set; }
        public int Value { get; set; }
        public override string ToString()
        {
            return $"{Type.ToString()} : {Value}";
        }
    }

    public interface IReadOnlyInventoryDataTable
    {
        int Id { get; }// PK
        int InventoryId { get; }
        int InventoryLevel { get; }
        string Requirements { get; }
        IReadOnlyList<InventoryRequirement> RequirementValues { get; }
        int TimeToUpgrade { get; }
    }

    public class InventoryDataTable : IBaseTable, IReadOnlyInventoryDataTable
    {
        public int Id { get; set; }
        public int InventoryId { get; set; }
        public int InventoryLevel { get; set; }
        [JsonIgnore]
        public string Requirements { get; set; }
        public IReadOnlyList<InventoryRequirement> RequirementValues { get; set; }
        public int TimeToUpgrade { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            Id = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            InventoryId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            InventoryLevel = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Requirements = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            TimeToUpgrade = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index);
            RequirementValues = JsonConvert.DeserializeObject<IReadOnlyList<InventoryRequirement>>(Requirements);
        }
    }
}

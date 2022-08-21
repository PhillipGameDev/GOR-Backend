using System;
using System.Data;
using GameOfRevenge.Common.Interface.Model.Table;
using GameOfRevenge.Common.Models.Table;
using Newtonsoft.Json;

namespace GameOfRevenge.Common.Models
{
    public class PlayerDataTable : BaseDataTypeTable<int, string>, IBaseUserDataTable<int, string>
    {
        [JsonProperty("PlayerDataId")]
        public long Id { get; set; }

        [JsonIgnore]
        public int PlayerId { get; set; }

        public override void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            Id = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt64(index); index++;
//            PlayerId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            DataType = reader.GetValue(index) == DBNull.Value ? DataType.Unknown : (DataType)Enum.Parse(typeof(DataType), reader.GetString(index)); index++;
            ValueId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Value = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index);
        }
    }
}

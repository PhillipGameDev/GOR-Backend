using System;
using System.Data;
using GameOfRevenge.Common.Interface.Model.Table;
using GameOfRevenge.Common.Models.Table;
using Newtonsoft.Json;

namespace GameOfRevenge.Common.Models
{
    public class PlayerDataTableUpdated : BaseDataTypeTableUpdated<int, string>, IBaseUserDataTableUpdated<int, string>
    {
        [JsonProperty("PlayerDataId")]
        public long Id { get; set; }

        [JsonIgnore]
        public int PlayerId { get; set; }

        public override void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            Id = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt64(index); index++;
            DataType = reader.GetValue(index) == DBNull.Value ? DataType.Unknown : (DataType)Enum.Parse(typeof(DataType), reader.GetString(index)); index++;
            ValueId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Value = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            if (index < reader.FieldCount)
            {
                string val = (reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index));
                OldValue = val;
//                if (int.TryParse(val, out int oldValue)) OldValue = oldValue;
            }
        }

        public PlayerDataTable ToPlayerDataTable
        {
            get
            {
                return new PlayerDataTable
                {
                    Id = this.Id,
                    PlayerId = this.PlayerId,
                    DataType = this.DataType,
                    ValueId = this.ValueId,
                    Value = this.Value
                };
            }
        }
    }
}

using System;
using System.Data;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models.Quest
{
    public interface IReadOnlyPlayerRewardData
    {
        long PlayerDataId { get; }
        DataType DataType { get; }
        int ValueId { get; }
        int Value { get; }
        
        int Count { get; }
    }

    public class PlayerRewardDataTable : IBaseTable, IReadOnlyPlayerRewardData
    {
        public long PlayerDataId { get; set; }
        public DataType DataType { get; set; }
        public int ValueId { get; set; }
        public int Value { get; set; }
        public int Count { get; set; }


        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            PlayerDataId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt64(index); index++;
            DataType = reader.GetValue(index) == DBNull.Value ? DataType.Unknown : (DataType)Enum.Parse(typeof(DataType), reader.GetString(index)); index++;
            ValueId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Value = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            int.TryParse(reader.GetString(index), out int val);
            Count = val;
        }
    }
}

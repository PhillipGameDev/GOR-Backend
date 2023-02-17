using GameOfRevenge.Common.Interface.Model;
using GameOfRevenge.Common.Interface.Model.Table;
using GameOfRevenge.Common.Models.Table;
using System;
using System.Data;

namespace GameOfRevenge.Common.Models
{
    public interface IReadOnlyDataReward : IBaseDataTypeTable<int, int>, IReadOnlyBaseDataTypeTable<int, int>
    {
        int RewardId { get; }
        int QuestId { get; }
        int Count { get; }
    }

    public interface IDataReward : IBaseDataTypeTable<int, int>, IReadOnlyBaseDataTypeTable<int, int>, IReadOnlyDataReward
    {
        new int RewardId { get; set; }
        new int QuestId { get; set; }
        new int Count { get; set; }
    }

    public class DataReward : BaseDataTypeTable<int, int>, IBaseTable, IBaseDataTypeTable<int, int>, IReadOnlyBaseDataTypeTable<int, int>, IDataReward, IReadOnlyDataReward
    {
        public int RewardId { get; set; }
        public int QuestId { get; set; }
        public int Count { get; set; }

        public override void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            RewardId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            QuestId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            DataType = reader.GetValue(index) == DBNull.Value ? DataType.Unknown : (DataType)reader.GetInt32(index); index++;
            ValueId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Value = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Count = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index);
        }

        public DataReward()
        {
        }

        public DataReward(DataType dataType, int valueId, int value, int count)
        {
            DataType = dataType;
            ValueId = valueId;
            Value = value;
            Count = count;
        }

        public override string ToString()
        {
            return DataType.ToString() + ": " + ValueId + ": " + Value + "x " + Count;
        }
    }
}

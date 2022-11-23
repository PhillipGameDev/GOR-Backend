using GameOfRevenge.Common.Interface.Model;
using GameOfRevenge.Common.Interface.Model.Table;
using GameOfRevenge.Common.Models.Table;
using System;
using System.Data;

namespace GameOfRevenge.Common.Models
{
    public interface IReadOnlyDataRequirement : IBaseDataTypeTable<int, int>, IReadOnlyBaseDataTypeTable<int, int>
    {
        int DataId { get; }
        int RequirementId { get; }
    }

    public interface IDataRequirement : IBaseDataTypeTable<int, int>, IReadOnlyBaseDataTypeTable<int, int>, IReadOnlyDataRequirement
    {
        new int DataId { get; set; }
        new int RequirementId { get; set; }
    }

    public class DataRequirement : BaseDataTypeTable<int, int>, IBaseTable, IBaseDataTypeTable<int, int>, IReadOnlyBaseDataTypeTable<int, int>, IDataRequirement, IReadOnlyDataRequirement
    {
        public int DataId { get; set; }
        public int RequirementId { get; set; }

        public override void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            RequirementId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            DataId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            DataType = reader.GetValue(index) == DBNull.Value ? DataType.Unknown : (DataType)reader.GetInt32(index); index++;
            ValueId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Value = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index);
        }

        public DataRequirement()
        {
        }

        public DataRequirement(DataType dataType, int valueId, int value)
        {
            DataType = dataType;
            ValueId = valueId;
            Value = value;
        }

        public override string ToString()
        {
            return DataType.ToString() + ": " + ValueId + ": " + Value;
        }
    }
}

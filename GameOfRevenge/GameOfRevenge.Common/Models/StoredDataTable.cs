using System;
using System.Data;
using System.Runtime.Serialization;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models
{
    public interface IReadOnlyStoredDataTable
    {
        long DataId { get; }
        int LocationId { get; }
        DataType DataType { get; }
        int ValueId { get; }
        int Value { get; }
    }

    [DataContract]
    public class StoredDataTable : IBaseTable, IReadOnlyStoredDataTable
    {
        [DataMember]
        public long DataId { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public int LocationId { get; set; }
        [DataMember]
        public DataType DataType { get; set; }
        [DataMember]
        public int ValueId { get; set; }
        [DataMember]
        public int Value { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            DataId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt64(index); index++;
            LocationId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            DataType = reader.GetValue(index) == DBNull.Value ? DataType.Unknown : (DataType)reader.GetInt32(index); index++;
            ValueId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Value = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index);
        }
    }
}

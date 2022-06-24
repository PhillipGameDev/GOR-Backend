using System;
using System.Data;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models
{
    public interface IReadOnlyStoredDataTable
    {
//        long StoredDataId { get; }
//        int PlayerId { get; }
        int StructureLocationId { get; }
        int DataTypeId { get; }
        int ValueId { get; }
        int Value { get; }
    }

    public class StoredDataTable : IBaseTable, IReadOnlyStoredDataTable
    {
//        public long StoredDataId { get; set; }
//        public int PlayerId { get; set; }
        public int StructureLocationId { get; set; }
        public int DataTypeId { get; set; }
        public int ValueId { get; set; }
        public int Value { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
//            StoredDataId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt64(index); index++;
//            PlayerId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            StructureLocationId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            DataTypeId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            ValueId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Value = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index);
        }
    }
}

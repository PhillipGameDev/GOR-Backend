using System;
using System.Data;
using System.Runtime.Serialization;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models
{
    public interface IReadOnlyStoredPlayerDataTable
    {
        long StoreId { get; }
        int Value { get; }
        long DataId { get; }
        string DataValue { get; }
    }

    public class StoredPlayerDataTable : IBaseTable, IReadOnlyStoredPlayerDataTable
    {
        public long StoreId { get; set; }
        public int Value { get; set; }
        public long DataId { get; set; }
        public string DataValue { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            StoreId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt64(index); index++;
            Value = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            DataId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt64(index); index++;
            DataValue = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index);
        }
    }
}

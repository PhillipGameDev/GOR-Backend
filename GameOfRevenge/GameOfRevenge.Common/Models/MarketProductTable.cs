using System;
using System.Data;
using GameOfRevenge.Common.Helper;
using GameOfRevenge.Common.Interface.Model.Table;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models
{
    public interface IReadOnlyProductTable
    {
        int ProductId { get; }// PK
        string Name { get; }
        string Description { get; }
        int DataTypeId { get; }// FK -> DataTypeTable
        int Value { get; }
        string REF1 { get; }
        string REF2 { get; }
        string REF3 { get; }
        string REF4 { get; }
    }

    public class MarketProductTable : IBaseTable, IReadOnlyProductTable
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DataTypeId { get; set; }
        public int Value { get; set; }
        public string REF1 { get; set; }
        public string REF2 { get; set; }
        public string REF3 { get; set; }
        public string REF4 { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            ProductId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Name = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            Description = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            DataTypeId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Value = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            REF1 = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            REF2 = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            REF3 = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            REF4 = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index);
        }
    }
}

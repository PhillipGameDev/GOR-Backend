using System;
using System.Data;
using GameOfRevenge.Common.Helper;
using GameOfRevenge.Common.Interface.Model.Table;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models
{
    public interface IReadOnlyShopItemTable: IBaseDataTypeTable<int, int>
    {
        int Id { get; } // PK
        int CategoryId { get; }
        int Cost { get; }
        int ItemId { get; }
    }

    public class ShopItemTable : IBaseTable, IReadOnlyShopItemTable
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public DataType DataType { get; set; }
        public int ValueId { get; set; }
        public int Value { get; set; }
        public int Cost { get; set; }
        public int ItemId { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            Id = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            CategoryId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            ItemId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            DataType = (DataType)(reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index)); index++;
            ValueId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Value = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Cost = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index);
        }
    }
}

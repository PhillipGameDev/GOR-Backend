using System;
using System.Data;
using GameOfRevenge.Common.Helper;
using GameOfRevenge.Common.Interface.Model.Table;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models
{
    public interface IReadOnlyShopCategoryTable
    {
        int Id { get; } // PK
        string Name { get; }
    }

    public class ShopCategoryTable : IBaseTable, IReadOnlyShopCategoryTable
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            Id = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Name = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index);
        }
    }
}

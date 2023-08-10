using System;
using System.Data;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models
{
    public interface IReadOnlyStorePackageTable
    {
        int PackageId { get; }// PK
        int QuestId { get; }// FK
        string ProductId { get; }
        int Cost { get; }
        bool Active { get; }
    }

    public class StorePackageTable : IBaseTable, IReadOnlyStorePackageTable
    {
        public int PackageId { get; set; }// PK
        public int QuestId { get; set; }// FK
        public string ProductId { get; set; }
        public int Cost { get; set; }
        public bool Active { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            PackageId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            QuestId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            ProductId = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            Cost = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Active = reader.GetValue(index) == DBNull.Value ? false : reader.GetBoolean(index);
        }
    }
}

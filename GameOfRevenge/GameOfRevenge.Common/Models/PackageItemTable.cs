using System;
using System.Data;
using GameOfRevenge.Common.Helper;
using GameOfRevenge.Common.Interface.Model.Table;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models
{
    public interface IReadOnlyPackageItemTable
    {
        int Id { get; } // PK
        int PackageId { get; }
        int TypeId { get; }
        int ValueId { get; }
        int Value { get; }
        int Count { get; }
    }

    public class PackageItemTable : IBaseTable, IReadOnlyPackageItemTable
    {
        public int Id { get; set; } // PK
        public int PackageId { get; set; }
        public int TypeId { get; set; }
        public int ValueId { get; set; }
        public int Value { get; set; }
        public int Count { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            Id = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            PackageId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            TypeId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            ValueId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Value = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Count = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index);
        }
    }
}

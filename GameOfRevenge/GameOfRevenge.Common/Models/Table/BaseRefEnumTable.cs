using System;
using System.Data;
using GameOfRevenge.Common.Helper;
using GameOfRevenge.Common.Interface.Model.Table;

namespace GameOfRevenge.Common.Models.Table
{
    public abstract class BaseRefEnumTable<T> : BaseTable, IBaseTable, IBaseRefEnumTable<T>, IReadOnlyBaseRefEnumTable<T> where T : Enum
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public T Code { get; set; }

        public override void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            Id = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Name = reader.GetString(index) ?? string.Empty; index++;
            Code = reader.GetString(index) == null ? default : reader.GetString(index).ToEnum<T>();
        }
    }
}

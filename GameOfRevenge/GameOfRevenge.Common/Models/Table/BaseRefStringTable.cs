using System;
using System.Data;
using GameOfRevenge.Common.Interface.Model.Table;

namespace GameOfRevenge.Common.Models.Table
{
    public abstract class BaseRefStringTable : BaseTable, IBaseTable, IBaseRefEnumTable<string>, IReadOnlyBaseRefEnumTable<string>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public override void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            Id = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Name = reader.GetString(index) ?? string.Empty; index++;
            Code = reader.GetString(index) ?? default;
        }
    }
}

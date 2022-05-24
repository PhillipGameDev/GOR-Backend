using System;
using System.Data;
using GameOfRevenge.Common.Helper;
using GameOfRevenge.Common.Interface.Model.Table;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models.Structure
{
    public interface IReadOnlyStructureTable : IReadOnlyBaseRefEnumTable<StructureType>
    {
        PostionType Position { get; }
        string Description { get; }
    }

    public class StructureTable : BaseRefEnumTable<StructureType>, IBaseTable, IBaseRefEnumTable<StructureType>, IReadOnlyBaseRefEnumTable<StructureType>, IReadOnlyStructureTable
    {
        public PostionType Position { get; set; }
        public string Description { get; set; }

        public override void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            Id = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Name = reader.GetString(index) ?? string.Empty; index++;
            Code = reader.GetString(index) == null ? default : reader.GetString(index).ToEnum<StructureType>(); index++;
            Position = reader.GetString(index) == null ? default : reader.GetString(index).ToEnum<PostionType>(); index++;
            Description = reader.GetString(index) ?? string.Empty;
        }
    }
}

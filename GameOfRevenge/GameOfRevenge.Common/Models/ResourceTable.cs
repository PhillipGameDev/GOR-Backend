using System;
using System.Data;
using GameOfRevenge.Common.Helper;
using GameOfRevenge.Common.Interface.Model.Table;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models
{
    public interface IReadOnlyResourceTable : IReadOnlyBaseRefEnumTable<ResourceType>
    {
        RarityType Rarity { get; }
    }

    public class ResourceTable : BaseRefEnumTable<ResourceType>, IBaseTable, IBaseRefEnumTable<ResourceType>, IReadOnlyBaseRefEnumTable<ResourceType>, IReadOnlyResourceTable
    {
        public RarityType Rarity { get; set; }

        public override void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            Id = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Rarity = reader.GetValue(index) == DBNull.Value ? RarityType.Common : reader.GetString(index).ToEnum<RarityType>(); index++;
            Name = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            Code = reader.GetValue(index) == DBNull.Value ? ResourceType.Other : reader.GetString(index).ToEnum<ResourceType>();
        }
    }
}

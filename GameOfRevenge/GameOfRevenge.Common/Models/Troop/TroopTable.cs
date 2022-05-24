using System;
using System.Data;
using GameOfRevenge.Common.Helper;
using GameOfRevenge.Common.Interface.Model.Table;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models.Troop
{
    public interface IReadOnlyTroopTable : IReadOnlyBaseRefEnumTable<TroopType>
    {
        string Description { get; }
        bool IsMagic { get; }
        bool IsMelee { get; }
        bool IsMounted { get; }
        bool IsSiege { get; }
    }

    public class TroopTable : BaseRefEnumTable<TroopType>, IBaseTable, IBaseRefEnumTable<TroopType>, IReadOnlyBaseRefEnumTable<TroopType>, IReadOnlyTroopTable
    {
        public string Description { get; set; }
        public bool IsMelee { get; set; }
        public bool IsMounted { get; set; }
        public bool IsSiege { get; set; }
        public bool IsMagic { get; set; }

        public override void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            Id = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Name = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            Code = reader.GetValue(index) == DBNull.Value ? TroopType.Other : reader.GetString(index).ToEnum<TroopType>(); index++;
            Description = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            IsMelee = reader.GetValue(index) != DBNull.Value && reader.GetBoolean(index); index++;
            IsMounted = reader.GetValue(index) != DBNull.Value && reader.GetBoolean(index); index++;
            IsMagic = reader.GetValue(index) != DBNull.Value && reader.GetBoolean(index); index++;
            IsSiege = reader.GetValue(index) != DBNull.Value && reader.GetBoolean(index);
        }
    }
}

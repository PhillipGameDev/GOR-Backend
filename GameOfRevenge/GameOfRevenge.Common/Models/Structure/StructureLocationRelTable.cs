using GameOfRevenge.Common.Helper;
using GameOfRevenge.Common.Models.Table;
using System;
using System.Data;

namespace GameOfRevenge.Common.Models.Structure
{
    public class StructureLocationRelTable : IBaseTable
    {
        public int StructureLocationId { get; set; }
        public StructureType StructureType { get; set; }
        public int Location { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            StructureLocationId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            StructureType = reader.GetValue(index) == DBNull.Value ? StructureType.Unknown : reader.GetString(index).ToEnum<StructureType>(); index++;
            Location = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index);
        }
    }
}

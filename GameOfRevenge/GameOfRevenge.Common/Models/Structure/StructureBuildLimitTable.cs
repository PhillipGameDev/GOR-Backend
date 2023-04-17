using GameOfRevenge.Common.Helper;
using GameOfRevenge.Common.Models.Table;
using System;
using System.Data;

namespace GameOfRevenge.Common.Models.Structure
{
    public class StructureBuildLimitTable : IBaseTable
    {
        public int StructureBuildDataId { get; set; }
        public StructureType BuildStructure { get; set; }
        public int TownHallLevel { get; set; }
        public int MaxBuildCount { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            StructureBuildDataId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            TownHallLevel = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            BuildStructure = reader.GetValue(index) == DBNull.Value ? StructureType.Unknown : reader.GetString(index).ToEnum<StructureType>(); index++;
            MaxBuildCount = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index);
        }
    }
}

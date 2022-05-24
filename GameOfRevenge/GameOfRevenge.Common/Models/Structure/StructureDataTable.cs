using System;
using System.Data;
using GameOfRevenge.Common.Interface.Model.Table;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models.Structure
{
    public interface IReadOnlyStructureDataTable : IReadOnlyBaseRefEnumLevelDataTable
    {
        double FoodProduction { get; }
        int HitPoint { get; }
        double OreProduction { get; }
        int PopulationSupport { get; }
        int StructureSupport { get; }
        int TimeToBuild { get; }
        double WoodProduction { get; }
        double ResourceCapacity { get; }
        int SafeDeposit { get; }
        int WoundedCapacity { get; }
    }

    public class StructureDataTable : BaseRefEnumLevelDataTable, IBaseTable, IBaseRefEnumLevelDataTable, IReadOnlyBaseRefEnumLevelDataTable, IReadOnlyStructureDataTable
    {
        public int HitPoint { get; set; }
        public double FoodProduction { get; set; }
        public double WoodProduction { get; set; }
        public double OreProduction { get; set; }
        public int PopulationSupport { get; set; }
        public int StructureSupport { get; set; }
        public int TimeToBuild { get; set; }
        public int SafeDeposit { get; set; }
        public double ResourceCapacity { get; set; }
        public int WoundedCapacity { get; set; }

        public override void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            DataId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Id = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Level = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            HitPoint = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            FoodProduction = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetDouble(index); index++;
            WoodProduction = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetDouble(index); index++;
            OreProduction = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetDouble(index); index++;
            PopulationSupport = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            StructureSupport = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            TimeToBuild = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            SafeDeposit = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            ResourceCapacity = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetDouble(index); index++;
            WoundedCapacity = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index);
        }
    }
}

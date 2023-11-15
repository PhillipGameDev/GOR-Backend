using System;
using System.Data;
using System.Runtime.Serialization;
using GameOfRevenge.Common.Interface.Model.Table;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models.Structure
{
    public interface IReadOnlyStructureDataTable : IReadOnlyBaseRefEnumLevelDataTable
    {
        int HitPoint { get; }
        int FoodProduction { get; }
        int WoodProduction { get; }
        int OreProduction { get; }
        int PopulationSupport { get; }
        int StructureSupport { get; }
        int TimeToBuild { get; }
        int ResourceCapacity { get; }
        int SafeDeposit { get; }
        int WoundedCapacity { get; }
        int InstantBuildCost { get; }
        int KingEXP { get; }
    }

//    [DataContract]
    public class StructureDataTable : BaseRefEnumLevelDataTable, IBaseTable, IBaseRefEnumLevelDataTable, IReadOnlyBaseRefEnumLevelDataTable, IReadOnlyStructureDataTable
    {
//        [DataMember(EmitDefaultValue = false, Order = 2)]
        public int HitPoint { get; set; }
//        [DataMember(EmitDefaultValue = false, Order = 2)]
        public int FoodProduction { get; set; }
//        [DataMember(EmitDefaultValue = false, Order = 2)]
        public int WoodProduction { get; set; }
//        [DataMember(EmitDefaultValue = false, Order = 2)]
        public int OreProduction { get; set; }
//        [DataMember(EmitDefaultValue = false, Order = 2)]
        public int PopulationSupport { get; set; }
//        [DataMember(EmitDefaultValue = false, Order = 2)]
        public int StructureSupport { get; set; }
//        [DataMember(EmitDefaultValue = false, Order = 2)]
        public int TimeToBuild { get; set; }
//        [DataMember(EmitDefaultValue = false, Order = 2)]
        public int SafeDeposit { get; set; }
//        [DataMember(EmitDefaultValue = false, Order = 2)]
        public int ResourceCapacity { get; set; }
//        [DataMember(EmitDefaultValue = false, Order = 2)]
        public int WoundedCapacity { get; set; }
//        [DataMember(EmitDefaultValue = false, Order = 2)]
        public int InstantBuildCost { get; set; }

        public int KingEXP { get; set; }

        public override void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            DataId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            InfoId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Level = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            HitPoint = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            FoodProduction = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            WoodProduction = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            OreProduction = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            PopulationSupport = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            StructureSupport = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            TimeToBuild = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            SafeDeposit = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            ResourceCapacity = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            WoundedCapacity = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            InstantBuildCost = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            KingEXP = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
        }
    }
}

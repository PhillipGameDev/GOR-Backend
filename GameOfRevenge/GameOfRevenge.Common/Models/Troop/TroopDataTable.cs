using System;
using System.Data;
using GameOfRevenge.Common.Interface.Model.Table;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models.Troop
{
    public interface IReadOnlyTroopDataTable : IReadOnlyBaseRefEnumLevelDataTable
    {
        float Health { get; }
        float WoundedThreshold { get; }
        float AttackDamage { get; }
        float AttackRange { get; }
        float AttackSpeed { get; }
        float Defense { get; }
        float MovementSpeed { get; }
        int WeightLoad { get; }
        int TraningTime { get; }
        int OutputCount { get; }
        int Power { get; set; }
    }

    public class TroopDataTable : BaseRefEnumLevelDataTable, IBaseTable, IBaseRefEnumLevelDataTable, IReadOnlyBaseRefEnumLevelDataTable, IReadOnlyTroopDataTable
    {
        public float Health { get; set; }
        public float WoundedThreshold { get; set; }
        public float AttackDamage { get; set; }
        public float AttackRange { get; set; }
        public float AttackSpeed { get; set; }
        public float Defense { get; set; }
        public float MovementSpeed { get; set; }
        public int WeightLoad { get; set; }
        public int TraningTime { get; set; }
        public int OutputCount { get; set; }
        public int Power { get; set; }

        public override void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            DataId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Id = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Level = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;

            Health = reader.GetValue(index) == DBNull.Value ? 0 : (float)reader.GetDouble(index); index++;
            WoundedThreshold = reader.GetValue(index) == DBNull.Value ? 0 : (float)reader.GetDouble(index); index++;
            AttackDamage = reader.GetValue(index) == DBNull.Value ? 0 : (float)reader.GetDouble(index); index++;
            AttackRange = reader.GetValue(index) == DBNull.Value ? 0 : (float)reader.GetDouble(index); index++;
            AttackSpeed = reader.GetValue(index) == DBNull.Value ? 0 : (float)reader.GetDouble(index); index++;
            Defense = reader.GetValue(index) == DBNull.Value ? 0 : (float)reader.GetDouble(index); index++;
            MovementSpeed = reader.GetValue(index) == DBNull.Value ? 0 : (float)reader.GetDouble(index); index++;

            WeightLoad = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            TraningTime = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            OutputCount = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;

            Power = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index);
        }
    }
}

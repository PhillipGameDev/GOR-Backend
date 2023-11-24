using GameOfRevenge.Common.Helper;
using GameOfRevenge.Common.Interface.Model.Table;
using GameOfRevenge.Common.Models.Table;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Xml.Linq;

namespace GameOfRevenge.Common.Models.Monster
{
    public interface IReadOnlyMonsterDataTable
    {
        int Id { get; }
        MonsterType MonsterType { get; }
        int Level { get; }
        int Health { get; }
        int Attack { get; }
        int Defense { get; }
    }

    public class MonsterDataTable : IReadOnlyMonsterDataTable, IBaseTable
    {
        public int Id { get; set; }
        public MonsterType MonsterType { get; set; }
        public int Level { get; set; }
        public int Health { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            Id = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            MonsterType = reader.GetValue(index) == DBNull.Value ? MonsterType.Other : (MonsterType)reader.GetInt32(index); index++;
            Level = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Health = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Attack = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Defense = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
        }
    }
}

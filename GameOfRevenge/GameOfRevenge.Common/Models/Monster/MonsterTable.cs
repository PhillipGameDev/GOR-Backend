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
    public interface IReadOnlyMonsterTable
    {
        int Id { get; }
        int MonsterDataId { get; }
        MonsterType MonsterType { get; }
        int Level { get; }
        int InitialHealth { get; }
        int Attack { get; }
        int Defense { get; }
        int WorldId { get; }
        int X { get; }
        int Y { get; }
        int Health { get; }
        DateTime CreatedDate { get; }
    }

    public class MonsterTable : IReadOnlyMonsterTable, IBaseTable
    {
        public int Id { get; set; }
        public int MonsterDataId { get; set; }
        public MonsterType MonsterType { get; set; }
        public int Level { get; set; }
        public int InitialHealth { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int WorldId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Health { get; set; }
        public DateTime CreatedDate { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            Id = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            MonsterDataId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            MonsterType = reader.GetValue(index) == DBNull.Value ? MonsterType.Other : (MonsterType)reader.GetInt32(index); index++;
            Level = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            InitialHealth = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Attack = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Defense = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            WorldId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            X = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Y = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Health = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            CreatedDate = reader.GetValue(index) == DBNull.Value ? DateTime.UtcNow : reader.GetDateTime(index); index++;
        }
    }
}

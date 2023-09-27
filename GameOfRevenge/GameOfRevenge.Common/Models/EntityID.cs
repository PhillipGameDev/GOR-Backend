using System;
using System.Data;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models
{
    public class EntityID : IBaseTable
    {
        public int EntityId { get; set; }
        public EntityType EntityType { get; set; }
        public int HitPoints { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Seed { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            var index = 0;
            EntityId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            EntityType = reader.GetValue(index) == DBNull.Value ? EntityType.Default : (EntityType)reader.GetByte(index); index++;
            HitPoints = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            X = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Y = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Seed = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index);
        }
    }
}

using System;
using System.Data;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models.Hero
{
    public interface IReadOnlyHeroDataRel
    {
        int Id { get; set; }
        int HeroId { get; set; } //record id hero table
        int StatType { get; set; }//enum from 1 to 6
    }

    public class HeroDataRel : IBaseTable, IReadOnlyHeroDataRel
    {
        public int Id { get; set; }
        public int HeroId { get; set; }
        public int StatType { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            Id = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            HeroId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            StatType = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index);
        }
    }
}

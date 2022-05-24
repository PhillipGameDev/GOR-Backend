using System;
using System.Data;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models.Hero
{
    public interface IReadOnlyHeroBoostTable
    {
        int HeroId { get; set; }
        int HeroBoostId { get; set; }
        int BoostId { get; set; }
    }

    public class HeroBoostTable : IBaseTable, IReadOnlyHeroBoostTable
    {
        public int HeroBoostId { get; set; }
        public int HeroId { get; set; }
        public int BoostId { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            HeroBoostId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            HeroId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            BoostId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index);
        }
    }
}

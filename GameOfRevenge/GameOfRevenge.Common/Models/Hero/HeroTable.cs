using System;
using System.Data;
using GameOfRevenge.Common.Helper;
using GameOfRevenge.Common.Interface.Model.Table;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models.Hero
{
    public interface IReadOnlyHeroTable
    {
        int HeroId { get; }
        string Code { get; }
        string Name { get; }
        string Description { get; }
    }

    public class HeroTable : IBaseTable, IReadOnlyHeroTable
    {
        public int HeroId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            HeroId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Name = reader.GetString(index) ?? string.Empty; index++;
            Code = reader.GetString(index) ?? string.Empty; index++;
            Description = reader.GetString(index) ?? string.Empty; 
        }
    }
}

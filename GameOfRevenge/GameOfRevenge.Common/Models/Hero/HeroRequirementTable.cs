using System;
using System.Data;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models.Hero
{
    public interface IReadOnlyHeroRequirementTable
    {
        int HeroId { get; }
        int HeroRequirementId { get; }
        int StructureDataId { get; }
        int StructureId { get; }
    }

    public class HeroRequirementTable : IBaseTable, IReadOnlyHeroRequirementTable
    {
        public int HeroRequirementId { get; set; }
        public int HeroId { get; set; }
        public int StructureDataId { get; set; }
        public int StructureId { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            HeroRequirementId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            HeroId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            StructureDataId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index);
        }
    }
}

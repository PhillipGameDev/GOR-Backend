using GameOfRevenge.Common.Models.Structure;
using static GameOfRevenge.Business.CacheData.CacheStructureDataManager;

namespace GameOfRevenge.Business.CacheData
{
    public class BuildingInfoData
    {
        public int StructureId { get; set; }
        public StructureType StructureType { get; set; }
        public string Name { get; internal set; }
        public string Code { get; internal set; }
        public string Description { get;  set; }
        public InfoDataTable Table { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}

using System.Collections.Generic;

namespace GameOfRevenge.Common.Models.Inventory
{
    public interface IReadOnlyBuffDataRequirementRel
    {
        IReadOnlyInventoryDataTable Info { get; }
        IReadOnlyList<IReadOnlyDataRequirement> Requirements { get; }
    }

    public class BuffDataRequirementRel : IReadOnlyBuffDataRequirementRel
    {
        public InventoryDataTable Info { get; set; }
        public List<DataRequirement> Requirements { get; set; }

        IReadOnlyInventoryDataTable IReadOnlyBuffDataRequirementRel.Info => Info;
        IReadOnlyList<IReadOnlyDataRequirement> IReadOnlyBuffDataRequirementRel.Requirements => Requirements;
    }
}

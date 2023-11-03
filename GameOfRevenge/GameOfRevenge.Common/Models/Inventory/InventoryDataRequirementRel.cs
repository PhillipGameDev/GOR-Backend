using System.Collections.Generic;

namespace GameOfRevenge.Common.Models.Inventory
{
    public interface IReadOnlyBuffDataRequirementRel
    {
        IReadOnlyInventoryTable Info { get; }
        IReadOnlyList<IReadOnlyDataRequirement> Requirements { get; }
    }

    public class BuffDataRequirementRel : IReadOnlyBuffDataRequirementRel
    {
        public InventoryTable Info { get; set; }
        public List<DataRequirement> Requirements { get; set; }

        IReadOnlyInventoryTable IReadOnlyBuffDataRequirementRel.Info => Info;
        IReadOnlyList<IReadOnlyDataRequirement> IReadOnlyBuffDataRequirementRel.Requirements => Requirements;
    }
}

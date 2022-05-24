using System.Collections.Generic;

namespace GameOfRevenge.Common.Models.Inventory
{
    public interface IReadOnlyInventoryDataRequirementRel
    {
        IReadOnlyInventoryTable Info { get; }
        IReadOnlyList<IReadOnlyDataRequirement> Requirements { get; }
    }

    public class InventoryDataRequirementRel : IReadOnlyInventoryDataRequirementRel
    {
        public InventoryTable Info { get; set; }
        public List<DataRequirement> Requirements { get; set; }

        IReadOnlyInventoryTable IReadOnlyInventoryDataRequirementRel.Info => Info;
        IReadOnlyList<IReadOnlyDataRequirement> IReadOnlyInventoryDataRequirementRel.Requirements => Requirements;
    }
}

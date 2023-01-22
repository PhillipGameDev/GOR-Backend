using GameOfRevenge.Common.Interface.Model;
using GameOfRevenge.Common.Models;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace GameOfRevenge.Common.Models.Structure
{
    public interface IReadOnlyStructureDataRequirementRel
    {
        IReadOnlyStructureTable Info { get; }
        IReadOnlyList<IReadOnlyStructureDataRequirement> Levels { get; }
        IReadOnlyList<int> Locations { get; }
        IReadOnlyDictionary<string, int> BuildLimit { get; }

        IReadOnlyStructureDataRequirement GetStructureLevelById(int level);
    }

    public interface IReadOnlyStructureDataRequirement
    {
        IReadOnlyStructureDataTable Data { get; }
        IReadOnlyList<IReadOnlyDataRequirement> Requirements { get; }
    }

    public class StructureDataRequirementRel : IReadOnlyStructureDataRequirementRel
    {
        public StructureTable Info { get; set; }
        public List<StructureDataRequirement> Levels { get; set; }
        public List<int> Locations { get; set; }
        public Dictionary<string, int> BuildLimit { get; set; }

        public StructureDataRequirement GetStructureLevelById(int level)
        {
            return Levels.Find(d => (d.Data.Level == level));
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyStructureTable IReadOnlyStructureDataRequirementRel.Info => Info;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyList<IReadOnlyStructureDataRequirement> IReadOnlyStructureDataRequirementRel.Levels => Levels;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyList<int> IReadOnlyStructureDataRequirementRel.Locations => Locations;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyDictionary<string, int> IReadOnlyStructureDataRequirementRel.BuildLimit => BuildLimit;

        IReadOnlyStructureDataRequirement IReadOnlyStructureDataRequirementRel.GetStructureLevelById(int level) => GetStructureLevelById(level);

        public override string ToString()
        {
            if (Info != null) return Info.Code.ToString();
            else return base.ToString();
        }
    }

    public class StructureDataRequirement : IReadOnlyStructureDataRequirement
    {
        public StructureDataTable Data { get; set; }
        public List<DataRequirement> Requirements { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyStructureDataTable IReadOnlyStructureDataRequirement.Data => Data;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyList<IReadOnlyDataRequirement> IReadOnlyStructureDataRequirement.Requirements => Requirements;

        public override string ToString()
        {
            if (Data != null) return Data.Level.ToString();
            else return base.ToString();
        }
    }
}

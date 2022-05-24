using System.Collections.Generic;
using System.Diagnostics;
using GameOfRevenge.Common.Interface.Model;

namespace GameOfRevenge.Common.Models.Technology
{
    public interface IReadOnlyTechnologyDataRequirementRel
    {
        IReadOnlyTechnologyTable Info { get; }
        IReadOnlyList<IReadOnlyTechnologyDataRequirement> Levels { get; }
    }

    public interface IReadOnlyTechnologyDataRequirement
    {
        IReadOnlyTechnologyDataTable Data { get; }
        IReadOnlyList<IReadOnlyDataRequirement> Requirements { get; }
    }

    public class TechnologyDataRequirementRel : IReadOnlyTechnologyDataRequirementRel
    {
        public TechnologyTable Info { get; set; }
        public List<TechnologyDataRequirements> Levels { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyTechnologyTable IReadOnlyTechnologyDataRequirementRel.Info => Info;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyList<IReadOnlyTechnologyDataRequirement> IReadOnlyTechnologyDataRequirementRel.Levels => Levels;
    }

    public class TechnologyDataRequirements : IReadOnlyTechnologyDataRequirement
    {
        public TechnologyDataTable Data { get; set; }
        public List<DataRequirement> Requirements { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyTechnologyDataTable IReadOnlyTechnologyDataRequirement.Data => Data;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyList<IReadOnlyDataRequirement> IReadOnlyTechnologyDataRequirement.Requirements => Requirements;
    }
}

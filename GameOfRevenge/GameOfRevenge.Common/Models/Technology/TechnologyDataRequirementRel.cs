using System.Collections.Generic;
using System.Diagnostics;
using GameOfRevenge.Common.Interface.Model;

namespace GameOfRevenge.Common.Models.Technology
{
/*    public interface IReadOnlySubTechnologyDataRequirementRel
    {
        IReadOnlySubTechnologyTable Info { get; }
        IReadOnlyList<IReadOnlySubTechnologyDataRequirement> Levels { get; }
    }

    public interface IReadOnlySubTechnologyDataRequirement
    {
        IReadOnlySubTechnologyDataTable Data { get; }
        IReadOnlyList<IReadOnlyDataRequirement> Requirements { get; }
    }

    public class SubTechnologyDataRequirementRel : IReadOnlySubTechnologyDataRequirementRel
    {
        public SubTechnologyTable Info { get; set; }
        public List<SubTechnologyDataRequirements> Levels { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlySubTechnologyTable IReadOnlySubTechnologyDataRequirementRel.Info => Info;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyList<IReadOnlySubTechnologyDataRequirement> IReadOnlySubTechnologyDataRequirementRel.Levels => Levels;
    }

    public class SubTechnologyDataRequirements : IReadOnlySubTechnologyDataRequirement
    {
        public SubTechnologyDataTable Data { get; set; }
        public List<DataRequirement> Requirements { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlySubTechnologyDataTable IReadOnlySubTechnologyDataRequirement.Data => Data;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyList<IReadOnlyDataRequirement> IReadOnlySubTechnologyDataRequirement.Requirements => Requirements;
    }*/


    public interface IReadOnlyTechnologyGroupInfo
    {
        GroupTechnologyType GroupType { get; }
        IReadOnlyList<IReadOnlyTechnologyDataRequirementRel> Technologies { get; }
    }

    [System.Serializable]
    public class TechnologyGroupInfo
    {
        public GroupTechnologyType GroupType { get; set; }
        public List<TechnologyDataRequirementRel> Technologies { get; set; }

        public TechnologyGroupInfo()
        {
        }

        public TechnologyGroupInfo(GroupTechnologyType group, List<TechnologyDataRequirementRel> list)
        {
            GroupType = group;
            Technologies = list;
        }
    }

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

    [System.Serializable]
    public class TechnologyDataRequirementRel : IReadOnlyTechnologyDataRequirementRel
    {
        public TechnologyTable Info { get; set; }
        public List<TechnologyDataRequirements> Levels { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyTechnologyTable IReadOnlyTechnologyDataRequirementRel.Info => Info;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyList<IReadOnlyTechnologyDataRequirement> IReadOnlyTechnologyDataRequirementRel.Levels => Levels;
    }

    [System.Serializable]
    public class TechnologyDataRequirements : IReadOnlyTechnologyDataRequirement
    {
        public TechnologyDataTable Data { get; set; }
        public List<DataRequirement> Requirements { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyTechnologyDataTable IReadOnlyTechnologyDataRequirement.Data => Data;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyList<IReadOnlyDataRequirement> IReadOnlyTechnologyDataRequirement.Requirements => Requirements;

        public TechnologyDataRequirements()
        {
        }

        public TechnologyDataRequirements(TechnologyDataTable data, List<DataRequirement> requirements)
        {
            Data = data;
            Requirements = requirements;
        }
    }
}

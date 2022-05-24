using GameOfRevenge.Common.Interface.Model;
using GameOfRevenge.Common.Models;
using System.Collections.Generic;
using System.Diagnostics;

namespace GameOfRevenge.Common.Models.Troop
{
    public interface IReadOnlyTroopDataRequirementRel
    {
        IReadOnlyTroopTable Info { get; }
        IReadOnlyList<IReadOnlyTroopDataRequirements> Levels { get; }
    }

    public interface IReadOnlyTroopDataRequirements
    {
        IReadOnlyTroopDataTable Data { get; }
        IReadOnlyList<IReadOnlyDataRequirement> Requirements { get; }
    }

    public class TroopDataRequirementRel : IReadOnlyTroopDataRequirementRel
    {
        public TroopTable Info { get; set; }
        public List<TroopDataRequirements> Levels { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyTroopTable IReadOnlyTroopDataRequirementRel.Info => Info;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyList<IReadOnlyTroopDataRequirements> IReadOnlyTroopDataRequirementRel.Levels => Levels;

        public override string ToString()
        {
            if (Info != null) return Info.Code.ToString();
            else return base.ToString();
        }
    }

    public class TroopDataRequirements : IReadOnlyTroopDataRequirements
    {
        public TroopDataTable Data { get; set; }
        public List<DataRequirement> Requirements { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyTroopDataTable IReadOnlyTroopDataRequirements.Data => Data;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyList<IReadOnlyDataRequirement> IReadOnlyTroopDataRequirements.Requirements => Requirements;

        public override string ToString()
        {
            if (Data != null) return Data.Level.ToString();
            else return base.ToString();
        }
    }
}

using GameOfRevenge.Common.Models.Structure;
using System.Collections.Generic;
using System.Diagnostics;

namespace GameOfRevenge.Common.Models.Troop
{
    public interface IReadOnlyTroopBuildingRelation
    {
        StructureType Structure { get; }
        IReadOnlyList<TroopType> Troops { get; }
    }


    public class TroopBuildingRelation : IReadOnlyTroopBuildingRelation
    {
        public StructureType Structure { get; set; }
        public List<TroopType> Troops { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        StructureType IReadOnlyTroopBuildingRelation.Structure => Structure;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyList<TroopType> IReadOnlyTroopBuildingRelation.Troops => Troops;

        public TroopBuildingRelation() : this(StructureType.Unknown, new List<TroopType>()) { }
        public TroopBuildingRelation(StructureType structure, List<TroopType> troops)
        {
            Structure = structure;
            Troops = troops;
        }
    }
}

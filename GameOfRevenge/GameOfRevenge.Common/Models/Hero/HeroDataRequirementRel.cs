using System.Collections.Generic;
using System.Diagnostics;

namespace GameOfRevenge.Common.Models.Hero
{
    public interface IReadOnlyHeroDataRequirementRel
    {
        IReadOnlyHeroTable Info { get; }
        IReadOnlyList<IReadOnlyHeroRequirementTable> Requirements { get; }
        IReadOnlyList<IReadOnlyHeroBoostTable> Boosts { get; }
    }

    public class HeroDataRequirementRel : IReadOnlyHeroDataRequirementRel
    {
        public HeroTable Info { get; set; }
        public List<HeroRequirementTable> Requirements { get; set; }
        public List<HeroBoostTable> Boosts { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyHeroTable IReadOnlyHeroDataRequirementRel.Info => Info;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyList<IReadOnlyHeroRequirementTable> IReadOnlyHeroDataRequirementRel.Requirements => Requirements;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyList<IReadOnlyHeroBoostTable> IReadOnlyHeroDataRequirementRel.Boosts => Boosts;
    }
}

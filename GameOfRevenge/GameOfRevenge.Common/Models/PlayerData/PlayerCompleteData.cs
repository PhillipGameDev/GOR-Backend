using System.Collections.Generic;

namespace GameOfRevenge.Common.Models
{
    public class PlayerCompleteData
    {
        public int PlayerId { get; set; }

        public ResourcesList Resources { get; set; }
        public List<StructureInfos> Structures { get; set; }
        public List<TroopInfos> Troops { get; set; }
        public MarchingArmy MarchingArmy { get; set; }
        public List<TechnologyInfos> Technologies { get; set; }
        public List<InventoryInfo> Items { get; set; }
        public List<UserBuffDetails> Buffs { get; set; }
        public List<UserHeroDetails> Heros { get; set; }
    }
}

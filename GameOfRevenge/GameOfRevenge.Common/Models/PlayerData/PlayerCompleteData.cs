using System.Collections.Generic;

namespace GameOfRevenge.Common.Models
{
    public class PlayerCompleteData
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public bool IsDeveloper { get; set; }
        public bool IsAdmin { get; set; }

        public int HelpedBuild { get; set; }
        public int ClanId { get; set; }
        public UserKingDetails King { get; set; }
        public UserVIPDetails VIP { get; set; }
        public int VIPPoints { get; set; }
        public int VIPLevel => UserVIPDetails.VIPLevel(VIPPoints);
        public List<UserRecordBuilderDetails> Workers { get; set; }

        public ResourcesList Resources { get; set; }
        public MarchingArmy MarchingArmy { get; set; }

        public List<StructureInfos> Structures { get; set; }
        public List<TroopInfos> Troops { get; set; }
        public List<TechnologyInfos> Technologies { get; set; }
//        public List<SubTechnologyInfos> SubTechnologies { get; set; }
        public List<UserItemDetails> Items { get; set; }
        public List<UserRecordNewBoost> Boosts { get; set; }
        public List<UserHeroDetails> Heroes { get; set; }
    }
}

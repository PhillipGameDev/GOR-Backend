using System.Collections.Generic;
using GameOfRevenge.Common.Models.Quest;
using GameOfRevenge.Common.Models.PlayerData;
using System.Linq;

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
        public int VIPLevel => UserVIPDetails.VIPLevel((VIP != null)? VIP.Points : 0);
        public List<UserRecordBuilderDetails> Workers { get; set; }

        public ResourcesList Resources { get; set; }
//        public MarchingArmy MarchingArmy { get; set; }
        public List<MarchingArmy> MarchingArmies { get; set; }

        public List<StructureInfos> Structures { get; set; }
        public List<TroopInfos> Troops { get; set; }
        public List<TechnologyInfos> Technologies { get; set; }
        public List<UserItemDetails> Items { get; set; }
        public List<UserRecordNewBoost> Boosts { get; set; }
        public List<UserHeroDetails> Heroes { get; set; }
        public List<UserTechnologyInfo> UserTechnologies { get; set; }

        public int CityLevel => Structures.Find(e => e.StructureType == Structure.StructureType.CityCounsel).Buildings.First().CurrentLevel;
    }

    public class AllPlayerData
    {
        public PlayerInfo PlayerInfo { get; set; }
        public List<PlayerDataTable> PlayerData { get; set; }
        public List<PlayerQuestDataTable> QuestData { get; set; }
    }
}

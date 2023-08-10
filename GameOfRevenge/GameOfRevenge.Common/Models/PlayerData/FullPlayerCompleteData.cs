using System.Linq;
using System.Collections.Generic;
using GameOfRevenge.Common.Models.Clan;
using GameOfRevenge.Common.Models.Quest;
using GameOfRevenge.Common.Models.Structure;

namespace GameOfRevenge.Common.Models
{
    public class FullPlayerCompleteData : PlayerCompleteData
    {
        public ClanData Clan { get; set; }
        public List<PlayerDataReward> Rewards { get; set; }
        public List<PlayerBackupTable> Backups { get; set; }
        public List<PlayerQuestDataTable> Quests { get; set; }

        public int KingLevel => (King != null) ? King.Level : 0;
        public int CastleLevel
        {
            get
            {
                var structure = Structures?.Find(x => (x.StructureType == StructureType.CityCounsel));
                var castle = structure?.Buildings.FirstOrDefault();
                var castleLvl = (castle != null) ? castle.CurrentLevel : 0;

                return castleLvl;
            }
        }
        public long Food => (Resources != null) ? Resources.Food : 0;
        public long Wood => (Resources != null) ? Resources.Wood : 0;
        public long Ore => (Resources != null) ? Resources.Ore : 0;
        public long Gems => (Resources != null) ? Resources.Gems : 0;

        public FullPlayerCompleteData()
        {
        }

        public FullPlayerCompleteData(PlayerCompleteData data)
        {
            PlayerId = data.PlayerId;
            PlayerName = data.PlayerName;
            IsDeveloper = data.IsDeveloper;
            IsAdmin = data.IsAdmin;

            HelpedBuild = data.HelpedBuild;
            ClanId = data.ClanId;
            King = data.King;
            VIP = data.VIP;
            VIPPoints = data.VIPPoints;
            Workers = data.Workers;

            Resources = data.Resources;
            MarchingArmy = data.MarchingArmy;

            Structures = data.Structures;
            Troops = data.Troops;
            Technologies = data.Technologies;
            Items = data.Items;
            Boosts = data.Boosts;
            Heroes = data.Heroes;
        }
    }
}
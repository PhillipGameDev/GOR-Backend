using GameOfRevenge.Common.Models.Kingdom;
using GameOfRevenge.Common.Models.PlayerData;

namespace GameOfRevenge.Common.Models
{
    public class AttackStatusData
    {
        public AttackResponseData AttackData { get; set; }
        public MarchingArmy MarchingArmy { get; set; }

        public PlayerCompleteData Attacker { get; set; }
        public PlayerCompleteData Defender { get; set; }
        public BattlePower AttackerPower { get; set; }
        public BattlePower DefenderPower { get; set; }

        public int State { get; set; }
    }

    public class AttackResponseData
    {
        public long MarchingId { get; set; }
        public MarchingType MarchingType { get; set; }

        public int AttackerId { get; set; }
        public string AttackerName { get; set; }

//        public int MonsterId { get; set; }//obsolete, use TargetId

        public int TargetId { get; set; }
        public string TargetName { get; set; }

        public string StartTime { get; set; }
        public int Recall { get; set; }
        public int Distance { get; set; }
        public int AdvanceReduction { get; set; }
        public int ReturnReduction { get; set; }
        public int Duration { get; set; }

        public byte KingLevel { get; set; }
        public byte WatchLevel { get; set; }

        public int[] Troops { get; set; }
        public int[] Heroes { get; set; }

        public AttackResponseData()
        {
        }

        public AttackResponseData(PlayerCompleteData attackerCompleteData, MarchingArmy marchingArmy, int targetId)
        {
            MarchingId = marchingArmy.MarchingId;
            MarchingType = marchingArmy.MarchingType;

            AttackerId = attackerCompleteData.PlayerId;
            AttackerName = attackerCompleteData.PlayerName;

            TargetId = targetId;
//            EnemyId = targetId;//obsolete
//            MonsterId = monsterId;//obsolete

            StartTime = marchingArmy.StartTime.ToUniversalTime().ToString("s") + "Z";
            Recall = marchingArmy.Recall;
            Distance = marchingArmy.Distance;
            AdvanceReduction = marchingArmy.AdvanceReduction;
            ReturnReduction = marchingArmy.ReturnReduction;
            Duration = marchingArmy.Duration;

            Troops = marchingArmy.TroopsToArray();
            Heroes = marchingArmy.HeroesToArray(attackerCompleteData.Heroes);
        }

        public AttackResponseData(PlayerCompleteData attackerCompleteData, MarchingArmy marchingArmy, int targetId, string enemyName, byte watchLevel)
        {
            MarchingId = marchingArmy.MarchingId;
            MarchingType = marchingArmy.MarchingType;

            AttackerId = attackerCompleteData.PlayerId;
            AttackerName = attackerCompleteData.PlayerName;

            TargetId = targetId;
//            EnemyId = targetId;//obsolete
            TargetName = enemyName;//obsolete

            KingLevel = attackerCompleteData.King.Level;
            WatchLevel = watchLevel;

            StartTime = marchingArmy.StartTime.ToUniversalTime().ToString("s") + "Z";
            Recall = marchingArmy.Recall;
            Distance = marchingArmy.Distance;
            AdvanceReduction = marchingArmy.AdvanceReduction;
            ReturnReduction = marchingArmy.ReturnReduction;
            Duration = marchingArmy.Duration;

            Troops = marchingArmy.TroopsToArray();
            Heroes = marchingArmy.HeroesToArray(attackerCompleteData.Heroes);
        }
    }
}

using System;
using System.Runtime.Serialization;
using GameOfRevenge.Common.Models.Kingdom;
using GameOfRevenge.Common.Models.Kingdom.AttackAlertReport;

namespace GameOfRevenge.Common.Models
{
    public class AttackStatusData
    {
        public AttackSocketResponse AttackData { get; set; }
        public int WinnerPlayerId { get; set; }
        public PlayerCompleteData Attacker { get; set; }
        public PlayerCompleteData Defender { get; set; }
        public BattlePower AttackerPower { get; set; }
        public BattlePower DefenderPower { get; set; }
        public int initialAttackerAtkPower { get; set; }
        public int initialAttackerDefPower { get; set; }
        public int initialDefenderAtkPower { get; set; }
        public int initialDefenderDefPower { get; set; }
        public BattleReport BattleReport { get; set; }
        public UnderAttackReport Report { get; set; }
        public int State { get; set; }
    }

//    [DataContract]
    public class AttackSocketResponse
    {
//        [DataMember]
        public int AttackerId { get; set; }
//        [DataMember]
        public string AttackerUsername { get; set; }

//        [DataMember]
        public int EnemyId { get; set; }
//        [DataMember]
        public string EnemyUsername { get; set; }

//        [DataMember]
        public int LocationX { get; set; }
//        [DataMember]
        public int LocationY { get; set; }

//        [DataMember]
        public DateTime StartTime { get; set; }
//        [DataMember]
        public int ReachedTime { get; set; }
//        [DataMember]
        public int BattleDuration { get; set; }

//        [DataMember]
        public byte KingLevel { get; set; }
//        [DataMember]
        public byte WatchLevel { get; set; }

//        [DataMember]
        public int[] Troops { get; set; }
//        [DataMember]
        public int[] Heroes { get; set; }
    }
}

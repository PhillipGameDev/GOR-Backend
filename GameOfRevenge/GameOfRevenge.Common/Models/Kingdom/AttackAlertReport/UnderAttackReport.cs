using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GameOfRevenge.Common.Models.Kingdom.AttackAlertReport
{
    [DataContract]
    public class UnderAttackReport
    {
        [DataMember]
        public int AttackerId { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public int DefenderId { get; set; }

        [DataMember]
        public string AttackerUsername { get; set; }
        [DataMember]
        public MapLocation Location { get; set; }

        [DataMember]
        public DateTime StartTime { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public int ReachedTime { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public byte KingLevel { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public byte WatchLevel { get; set; }

        //        public int TotalTroopSize { get; set; }
        [DataMember]
        public List<TroopData> Troops { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<HeroData> Heroes { get; set; }

        public string EnemyUsername { get; set; }
        public int Duration { get; set; }
    }

    public class AttackReport : UnderAttackReport
    {
    }
}
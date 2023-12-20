using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GameOfRevenge.Common.Models.Kingdom
{
    public enum BattleTroopType
    {
        Other = 0,
        Troop = 1,
        Gate = 2,
        Monster = 3,
        GloryKingdom = 4
    }

    [DataContract]
    public class BattleReplay : BattleReport
    {
        [DataMember]
        public DateTime DateTime { get; set; }
        [DataMember] 
        public List<BattleTroopInfo> Troops { get; set; }
        [DataMember]
        public List<BattleStep> Steps { get; set; }
    }

    [Serializable]
    public class BattleTroopInfo
    {
        public int Id { get; set; }
        public BattleTroopType TroopMainType { get; set; }
        public TroopType? TroopType { get; set; }
        public MonsterType? MonsterType { get; set; }
        public float Attack { get; set; }
        public float Defense { get; set; }
        public float Health { get; set; }   
        public int Count { get; set; }
        public bool IsAttacker { get; set; }

        public float AttackMultiplier { get; set; }
        public float DefenseMultiplier { get; set; }

        public float RealAttack => Attack * AttackMultiplier;
        public float RealDefense => Defense * DefenseMultiplier;
    }

    [Serializable]
    public class BattleStep
    {
        public List<BattleStatus> Troops { get; set; }

        public BattleStep()
        {
            Troops = new List<BattleStatus>();
        }
    }

    [Serializable]
    public class BattleStatus
    {
        public int TroopId { get; set; }
        public float Health { get; set; }
        public int AttackingId { get; set; }
    }
    
}

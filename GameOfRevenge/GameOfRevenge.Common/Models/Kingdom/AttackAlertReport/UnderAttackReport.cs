using System;
using System.Collections.Generic;

namespace GameOfRevenge.Common.Models.Kingdom.AttackAlertReport
{
    public class UnderAttackReport
    {
        public int AttackerId { get; set; }
        public int DefenderId { get; set; }

        public string AttackerUsername { get; set; }
        public MapLocation Location { get; set; }

        public DateTime StartTime { get; set; }
        public int ReachedTime { get; set; }

        public int KingLevel { get; set; }

        public int TotalTroopSize { get; set; }
        public List<TroopData> Troops { get; set; }

        public int TotalHeroSize { get; set; }
        public List<TroopData> Heroes { get; set; }
    }
}
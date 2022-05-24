using System;
using System.Collections.Generic;

namespace GameOfRevenge.Common.Models.Kingdom.AttackAlertReport
{
    public class UnderAttackReport
    {
        public int Id { get; set; }
        public int Defenderid { get; set; }

        public string Name { get; set; }
        public MapLocation Location { get; set; }
        public double TimeTaken { get; set; }

        public int KingLevel { get; set; }

        public int TotalTroopSize { get; set; }
        public List<TroopData> Troops { get; set; }

        public int TotalHeroSize { get; set; }
        public List<TroopData> Heros { get; set; }
    }
}
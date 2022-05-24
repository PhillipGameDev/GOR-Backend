using System.Collections.Generic;

namespace GameOfRevenge.Common.Models.Kingdom
{
    public class ClientBattleReport
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }

        public int TotalArmy { get; set; }
        public int Survived { get; set; }
        public int Wounded { get; set; }
        public int Dead { get; set; }

        public int Food { get; set; }
        public int Wood { get; set; }
        public int Ore { get; set; }

        public List<string> Heros { get; set; }
    }

    public class TroopDetailsPvP
    {
        public int Level { get; set; }
        public TroopType Type { get; set; }
        public int Hp { get; set; }
        public int UnitHp { get; set; }
        public int Count => Hp / UnitHp;
        public int TotalCount { get; set; }
        public int LoadPerUnit { get; set; }
        public int Dead { get; set; }
    }
}

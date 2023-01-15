using System;
using System.Collections.Generic;
using GameOfRevenge.Common.Models.Troop;

namespace GameOfRevenge.Common.Models.Kingdom
{
    public class ClientBattleReport
    {
        public int PlayerId { get; set; }
        public string Username { get; set; }

        public int Attack { get; set; }
        public int Defense { get; set; }
        public int TotalArmy { get; set; }
//        public int Power { get; set; }
        public int Survived
        {
            get => TotalArmy - Dead;
        }
        public int Wounded { get; set; }
        public int Dead { get; set; }

        public int Food { get; set; }
        public int Wood { get; set; }
        public int Ore { get; set; }

        public List<string> Heroes { get; set; }
    }

    public class TroopDetailsPvP
    {
        public TroopType Type { get; set; }
        public int Level => Data.Level;
        public int Hp { get; set; }
//        public int UnitHp { get; set; }
        public float RemainUnits => (Data.Health > 0)? (Hp / (float)Data.Health) : 0;
        public int InitialCount { get; set; }
        public int LoadPerUnit { get; set; }
        public int Dead { get; set; }
        public int Wounded { get; set; }

        public IReadOnlyTroopDataTable Data { get; set; }

        public TroopDetailsPvP(TroopType type, int count, IReadOnlyTroopDataTable data)
        {
            Type = type;
            Data = data;
            InitialCount = count;
            LoadPerUnit = data.WeightLoad;
            Hp = (int)data.Health * count;

//            UnitHp = (int)data.Health;
        }
    }
}

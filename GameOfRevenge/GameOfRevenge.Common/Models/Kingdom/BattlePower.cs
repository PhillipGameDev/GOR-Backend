using System.Collections.Generic;
using System.Runtime.Serialization;
using GameOfRevenge.Common.Models.Troop;
using GameOfRevenge.Common.Models.Kingdom.AttackAlertReport;
using GameOfRevenge.Common.Models.Hero;

namespace GameOfRevenge.Common.Models.Kingdom
{
    public class BattlePower : ClientBattleReport
    {
        public int HitPoint { get; set; }

        public List<AttackDefenseMultiplier> Multipliers { get; set; }
        public int GateHp { get; set; }

//        public new List<HeroType> Heroes { get; set; }
        public List<TroopInfos> Army { get; set; }
        public List<TroopDetailsPvP> TroopsAlive { get; set; }

        public int TotalLoad { get; set; }

        public void Recalculate()
        {
            if ((TroopsAlive == null) || (TroopsAlive.Count == 0)) return;

            float points = 0;
            float attack = 0;
            float defense = 0;

            foreach (var troops in TroopsAlive)
            {
                if (troops.Hp <= 0) continue;

                float attackMultiplier = 1;
                float defenseMultiplier = 1;
                var multi = Multipliers.Find(x => (x.Troop == troops.Troop));
                if (multi != null)
                {
                    attackMultiplier = multi.AttackMultiplier;
                    defenseMultiplier = multi.DefenseMultiplier;
                }
                points += troops.Data.Health * troops.RemainUnits;
                attack += (troops.Data.AttackDamage * troops.RemainUnits) * attackMultiplier;
                defense += (troops.Data.Defense * troops.RemainUnits) * defenseMultiplier;
            }
            var globalMulti = Multipliers.Find(x => (x.Troop == null));
            if (globalMulti != null)
            {
                attack *= globalMulti.AttackMultiplier;
                defense *= globalMulti.DefenseMultiplier;
            }

            HitPoint = (int)points;
            Attack = (int)attack;
            Defense = (int)defense;
        }
    }

    public class AttackDefenseMultiplier
    {
        public TroopInfos Troop { get; set; }
        public float AttackMultiplier { get; set; } = 1;
        public float DefenseMultiplier { get; set; } = 1;
    }

    [DataContract]
    public class ClientBattleReport
    {
        [DataMember]
        public int PlayerId { get; set; }
        [DataMember]
        public string Username { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int Attack { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public int Defense { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public int TotalArmy { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public int Wounded { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public int Dead { get; set; }
        public int Survived => (TotalArmy - Dead);

        [DataMember(EmitDefaultValue = false)]
        public int Food { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public int Wood { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public int Ore { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<HeroData> Heroes { get; set; }
    }

    public class TroopDetailsPvP
    {
        public TroopInfos Troop { get; set; }
        public int Level => Data.Level;
        public int Hp { get; set; }
//        public int UnitHp { get; set; }
        public float RemainUnits => (Data.Health > 0)? (Hp / (float)Data.Health) : 0;
        public int InitialCount { get; set; }
        public int LoadPerUnit { get; set; }
        public int Dead { get; set; }
        public int Wounded { get; set; }

        public IReadOnlyTroopDataTable Data { get; set; }

        public TroopDetailsPvP(TroopInfos troop, int count, IReadOnlyTroopDataTable data)
        {
            Troop = troop;
            Data = data;
            InitialCount = count;
            LoadPerUnit = data.WeightLoad;
            Hp = (int)data.Health * count;

//            UnitHp = (int)data.Health;
        }
    }
}

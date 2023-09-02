using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GameOfRevenge.Common.Models.Troop;
using GameOfRevenge.Common.Models.Kingdom.AttackAlertReport;
using GameOfRevenge.Common.Models.Hero;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Models.PlayerData;

namespace GameOfRevenge.Common.Models.Kingdom
{
    public class BattlePower : ClientBattleReport
    {
        public int HitPoints { get; set; }

        public List<AttackDefenseMultiplier> Multipliers { get; set; }
        public int GateHP { get; set; }

        public List<TroopInfos> Troops { get; set; }
        public List<TroopDetailsPvP> TroopChanges { get; set; }

        public int TotalLoad { get; set; }

        internal int TempAttack;
        internal int TempDefense;

        public BattlePower(int playerId, int monsterId, int hitPoints, int attack, int defense)
        {
            PlayerId = playerId;
            MonsterId = monsterId;
            HitPoints = hitPoints;
            TempAttack = attack;
            TempDefense = defense;
            Attack = attack;
            Defense = defense;
//            Username
//            TotalArmy
//            Wounded
//            Dead
        }

        public BattlePower(PlayerCompleteData completeData, MarchingArmy marchingArmy,
            Func<TroopType, IReadOnlyTroopDataRequirementRel> getTroopData,
            Func<PlayerCompleteData, MarchingArmy, List<AttackDefenseMultiplier>> getAtkDefMultiplier)
        {
            PlayerId = completeData.PlayerId;
            Username = completeData.PlayerName;
            
            var troops = (marchingArmy != null)? marchingArmy.Troops : completeData.Troops;
            if ((troops != null) && (troops.Count > 0))
            {
                TroopChanges = new List<TroopDetailsPvP>();
                foreach (var troop in troops)
                {
                    if (troop == null) continue;

                    var troopSpecData = getTroopData(troop.TroopType);
                    if (troopSpecData == null) continue;

                    foreach (var troopData in troop.TroopData)
                    {
                        if (troopData == null) continue;

                        var troopLvlDefData = troopSpecData.Levels.FirstOrDefault(x => (x.Data.Level == troopData.Level));
                        if ((troopLvlDefData == null) || (troopLvlDefData.Data == null)) continue;

                        var troopDetails = new TroopDetailsPvP(troop, troopData.FinalCount, troopLvlDefData.Data);
                        TroopChanges.Add(troopDetails);
                    }
                }
            }

            if (marchingArmy != null)
            {
                Heroes = GetAvailableHeroes(marchingArmy.Heroes, completeData.Heroes);
            }
            else
            {
                var structures = completeData.Structures.Find(x => (x.StructureType == StructureType.Gate));
                if (structures != null)
                {
                    var defenderGate = structures.Buildings.OrderBy(x => x.Level).FirstOrDefault();
                    GateHP = (defenderGate != null) ? defenderGate.HitPoints : 0;
                }
            }

            Multipliers = getAtkDefMultiplier(completeData, marchingArmy);
            Recalculate();
            Attack = TempAttack;
            Defense = TempDefense;
        }

        private List<HeroData> GetAvailableHeroes(List<HeroType> marchingHeroes, List<UserHeroDetails> userHeroes)
        {
            List<HeroData> heroes = null;
            if ((marchingHeroes != null) && (marchingHeroes.Count > 0))
            {
                heroes = marchingHeroes.ConvertAll(x =>
                {
                    return new HeroData()
                    {
                        Type = x,
                        Level = (int)userHeroes.Find(y => (y.HeroType == x))?.Level
                    };
                });
            }

            return heroes;
        }

        public void Recalculate()
        {
            if ((TroopChanges == null) || (TroopChanges.Count == 0)) return;

            float points = 0;
            float attack = 0;
            float defense = 0;

            foreach (var troops in TroopChanges)
            {
                if (troops.TotalHP <= 0) continue;

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

            HitPoints = (int)points;
            TempAttack = (int)attack;
            TempDefense = (int)defense;
        }

        public void AttackMonster(BattlePower defenderPower)
        {
            var attackerPower = this;
            var random = new Random();
            var atkSoldierHealth = attackerPower.TroopChanges.Average(x => x.Data.Health);
            var atkSoldiersToSacrifice = Math.Max(10, (attackerPower.TroopChanges.Sum(x => x.InitialCount) * 0.1f));

            var defDamage = defenderPower.TempAttack - attackerPower.TempDefense;
            if (defDamage < (atkSoldierHealth * atkSoldiersToSacrifice))
            {
                defDamage = (int)(atkSoldierHealth * atkSoldiersToSacrifice * (random.Next(5, 10) / 10f));
            }
            var multiplier = (random.Next(3, 8) / 10f);
            var damageToAttacker = defDamage * multiplier;

            foreach (var troop in attackerPower.TroopChanges)
            {
                var temp = troop.TotalHP;
                troop.TotalHP -= (int)damageToAttacker;
                if (troop.TotalHP <= 0)
                {
                    troop.TotalHP = 0;
                    damageToAttacker -= temp;
                    if (damageToAttacker < 0) damageToAttacker = 0;
                    continue;
                }

                break;
            }

            var atkDamage = attackerPower.TempAttack - defenderPower.TempDefense;
            multiplier = (random.Next(3, 8) / 10f);
            var damageToDefender = atkDamage * multiplier;
            defenderPower.HitPoints -= (int)damageToDefender;
            defenderPower.TempAttack = (int)(defenderPower.TempAttack * 0.9f);
            if (defenderPower.HitPoints < 0) defenderPower.HitPoints = 0;

            attackerPower.Recalculate();
        }

        public void AttackPlayer(BattlePower defenderPower)
        {
            var attackerPower = this;
            var random = new Random();
            var atkSoldierHealth = attackerPower.TroopChanges.Average(x => x.Data.Health);
            var atkSoldiersToSacrifice = Math.Max(10, (attackerPower.TroopChanges.Sum(x => x.InitialCount) * 0.1f));
            
            var defDamage = defenderPower.TempAttack - attackerPower.TempDefense;
            if (defDamage < (atkSoldierHealth * atkSoldiersToSacrifice))
            {
                defDamage = (int)(atkSoldierHealth * atkSoldiersToSacrifice * (random.Next(5, 10) / 10f));
            }
            var multiplier = (random.Next(3, 8) / 10f);
            var damageToAttacker = defDamage * multiplier;

            foreach (var troop in attackerPower.TroopChanges)
            {
                var temp = troop.TotalHP;
                troop.TotalHP -= (int)damageToAttacker;
                if (troop.TotalHP <= 0)
                {
                    troop.TotalHP = 0;
                    damageToAttacker -= temp;
                    if (damageToAttacker < 0) damageToAttacker = 0;
                    continue;
                }

                break;
            }

            float defSoldierHealth = 0;
            float defSoldiersToSacrifice = 0;
            if (defenderPower.TroopChanges != null)
            {
                defSoldierHealth = defenderPower.TroopChanges.Average(x => x.Data.Health);
                defSoldiersToSacrifice = Math.Max(20, (defenderPower.TroopChanges.Sum(x => x.InitialCount) * 0.2f));
            }

            var atkDamage = attackerPower.TempAttack - defenderPower.TempDefense;
            if (atkDamage < (defSoldierHealth * defSoldiersToSacrifice))
            {
                atkDamage = (int)(defSoldierHealth * defSoldiersToSacrifice * (random.Next(5, 10) / 10f));
            }
            multiplier = (random.Next(3, 8) / 10f);
            var damageToDefender = atkDamage * multiplier;

            if (defenderPower.GateHP > 0)
            {
                var temp = defenderPower.GateHP;
                defenderPower.GateHP -= (int)damageToDefender;
                if (defenderPower.GateHP < 0) defenderPower.GateHP = 0;
                damageToDefender -= temp;
            }
            if ((damageToDefender > 0) && (defenderPower.TroopChanges != null))
            {
                foreach (var troop in defenderPower.TroopChanges)
                {
                    var temp = troop.TotalHP;
                    troop.TotalHP -= (int)damageToDefender;
                    if (troop.TotalHP <= 0)
                    {
                        troop.TotalHP = 0;
                        damageToDefender -= temp;
                        if (damageToDefender < 0) damageToDefender = 0;
                        continue;
                    }

                    break;
                }
            }

            attackerPower.Recalculate();
            defenderPower.Recalculate();
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
        [DataMember(EmitDefaultValue = false)]
        public string Username { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public int MonsterId { get; set; }

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

#if UNITY_2017_1_OR_NEWER
        [DataMember(EmitDefaultValue = false)]
        public int Food { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public int Wood { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public int Ore { get; set; }
#endif

        [DataMember(EmitDefaultValue = false)]
        public List<HeroData> Heroes { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<DataReward> Items { get; set; }
    }

    [DataContract]
    public class TroopDetailsPvP
    {
        [DataMember]
        public TroopType TroopType { get; set; }
        [DataMember]
        public int Level { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public int Dead { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public int Wounded { get; set; }

        public TroopInfos Troop { get; set; }
        public IReadOnlyTroopDataTable Data { get; set; }
        public int InitialCount { get; set; }
        public int TotalHP { get; set; }
        public float RemainUnits => (Data.Health > 0)? (TotalHP / Data.Health) : 0;
        public int LoadPerUnit => Data.WeightLoad;

        public TroopDetailsPvP()
        {
        }

        public TroopDetailsPvP(TroopInfos troop, int count, IReadOnlyTroopDataTable data)
        {
            TroopType = troop.TroopType;
            Level = data.Level;

            Troop = troop;
            Data = data;
            InitialCount = count;
            TotalHP = (int)(data.Health * count);
        }
    }
}

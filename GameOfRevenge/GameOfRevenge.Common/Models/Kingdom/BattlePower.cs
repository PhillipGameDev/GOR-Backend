using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GameOfRevenge.Common.Models.Troop;
using GameOfRevenge.Common.Models.Kingdom.AttackAlertReport;
using GameOfRevenge.Common.Models.Hero;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Models.PlayerData;
using Newtonsoft.Json;
using GameOfRevenge.Common.Models.Monster;
using System.Collections;

namespace GameOfRevenge.Common.Models.Kingdom
{
    public class BattlePower : ClientBattleReport
    {
        public int HitPoints => (int)(TroopChanges?.Sum(e => e.TotalHP) ?? 0);

        public int AttackCalc {
            get
            {
                Dictionary<TroopType, (float attack, float defense)> attackMulti = new Dictionary<TroopType, (float, float)>();

                foreach (TroopType type in Enum.GetValues(typeof(TroopType)))
                {
                    attackMulti[type] = GetMultiplier(Multipliers, type);
                }

                return (int)TroopChanges.Sum(e => e.Count * e.Data.AttackDamage * attackMulti[e.TroopType].attack);
            }
        }
        public int DefenseCalc {
            get
            {
                Dictionary<TroopType, (float attack, float defense)> attackMulti = new Dictionary<TroopType, (float, float)>();

                foreach (TroopType type in Enum.GetValues(typeof(TroopType)))
                {
                    attackMulti[type] = GetMultiplier(Multipliers, type);
                }

                return (int)TroopChanges.Sum(e => e.Count * e.Data.Defense * attackMulti[e.TroopType].defense);
            }
        }

        public List<AttackDefenseMultiplier> Multipliers { get; set; }
        public int GateHP { get; set; }

        public List<TroopInfos> Troops { get; set; }

        public int TotalLoad { get; set; }

        private Random random = new Random();

        // Created by Monster
        public BattlePower(MonsterTable monster)
        {
            EntityType = EntityType.Monster;
            EntityId = monster.Id;

            Troops = new List<TroopInfos>()
            {
                new TroopInfos()
                {
                    Id = 0,
                    TroopMainType = BattleTroopType.Monster,
                    MonsterType = monster.MonsterType,
                    TroopData = new List<TroopDetails>
                    {
                        new TroopDetails()
                        {
                            Level = monster.Level,
                            Count = 1
                        }
                    }
                }
            };

            var info = Troops.First();
            TroopChanges = new List<TroopDetailsPvP>()
            {
                new TroopDetailsPvP()
                {
                    Troop = info,
                    TroopMainType = BattleTroopType.Monster,
                    MonsterType = info.MonsterType,
                    Level = monster.Level,
                    MonsterHP = monster.Health,
                    InitialCount = 1,
                    Count = 1,
                    Data = new TroopDataTable()
                    {
                        Health = monster.Health,
                        AttackDamage = monster.Attack,
                        Defense = monster.Defense
                    }
                }
            };
        }

        public BattlePower(EntityType entityType, int entityId, int hitPoints, int attack, int defense)
        {
            EntityType = entityType;
            EntityId = entityId;

            Troops = new List<TroopInfos>()
            {
                new TroopInfos()
                {
                    Id = 0,
                    TroopMainType = BattleTroopType.Troop,
                    TroopType = TroopType.Other,
                    TroopData = new List<TroopDetails>
                    {
                        new TroopDetails()
                        {
                            Count = 1
                        }
                    }
                }
            };

            var info = Troops.First();
            TroopChanges = new List<TroopDetailsPvP>()
            {
                new TroopDetailsPvP()
                {
                    Troop = info,
                    TroopMainType = BattleTroopType.Troop,
                    TroopType = info.TroopType,
                    InitialCount = 1,
                    Count = 1,
                    Data = new TroopDataTable()
                    {
                        Health = hitPoints,
                        AttackDamage = attack,
                        Defense = defense
                    }
                }
            };
//            Username
//            TotalArmy
//            Wounded
//            Dead
        }

        public BattlePower(PlayerCompleteData completeData, MarchingArmy marchingArmy,
            Func<TroopType, IReadOnlyTroopDataRequirementRel> getTroopData,
            Func<PlayerCompleteData, MarchingArmy, List<AttackDefenseMultiplier>> getAtkDefMultiplier, Action<string> log = null)
        {
            EntityType = EntityType.Player;
            EntityId = completeData.PlayerId;
            EntityName = completeData.PlayerName;

            var troops = (marchingArmy != null) ? marchingArmy.Troops : completeData.Troops;

            Troops = troops;
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
            } else
            {
                Heroes = GetTopLevelHeroes(completeData.Heroes);
            }

            if (marchingArmy == null && completeData.Structures != null)
            {
                var structures = completeData.Structures.Find(x => (x.StructureType == StructureType.Gate));
                if (structures != null)
                {
                    var defenderGate = structures.Buildings.OrderBy(x => x.Level).FirstOrDefault();
                    GateHP = (defenderGate != null) ? defenderGate.HitPoints : 0;
                }
            }

            if (getAtkDefMultiplier != null)
            {
                Multipliers = getAtkDefMultiplier(completeData, marchingArmy);
                log?.Invoke("Multipliers = " + JsonConvert.SerializeObject(Multipliers));
            }

            Items = new List<ItemData>();

            Attack = AttackCalc;
            Defense = DefenseCalc;

            log?.Invoke("--PREPARE-ARMY - " + Attack + ":" + Defense + ";" + HitPoints);
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

        private List<HeroData> GetTopLevelHeroes(List<UserHeroDetails> userHeroes, int count = 5)
        {
            List<HeroData> heroes = new List<HeroData>();

            if (userHeroes != null)
            {
                foreach (var userHero in userHeroes.OrderByDescending(e => e.Level).Take(count))
                {
                    heroes.Add(new HeroData()
                    {
                        Type = userHero.HeroType,
                        Level = userHero.Level
                    });
                }
            }
            return heroes;
        }
        /*
        void Recalculate(Action<string> log = null)
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
                var multi = Multipliers?.Find(x => (x.Troop == troops.Troop));
                if (multi != null)
                {
                    attackMultiplier = multi.AttackMultiplier;
                    defenseMultiplier = multi.DefenseMultiplier;
                }
                points += troops.Data.Health * troops.RemainUnits;
                attack += (troops.Data.AttackDamage * troops.RemainUnits) * attackMultiplier;
                defense += (troops.Data.Defense * troops.RemainUnits) * defenseMultiplier;
            }

            var globalMulti = Multipliers?.Find(x => (x.Troop == null));//<--- global multiplier
            if (globalMulti != null)
            {
                attack *= globalMulti.AttackMultiplier;
                defense *= globalMulti.DefenseMultiplier;
            }

            HitPoints = (int)points;
            TempAttack = (int)attack;
            TempDefense = (int)defense;
            log?.Invoke($"recalculate hp={HitPoints} atk={TempAttack} def={TempDefense}");
        }*/

        public void AttackMonster(BattlePower defenderPower, BattleReplay replay)
        {

            var attackerPower = this;
            replay.DateTime = DateTime.UtcNow;

            int monsterId = GenerateTroops(replay, attackerPower, defenderPower, BattleTroopType.Monster);
            replay.Steps = new List<BattleStep>();

            BattleStep prevStep = null;
            BattleStep currentStep = new BattleStep();

            do
            {
                // Find Targets
                if (prevStep == null)
                {
                    foreach (var troop in replay.Troops)
                    {
                        currentStep.Troops.Add(new BattleStatus()
                        {
                            TroopId = troop.Id,
                            Health = troop.Health,
                            AttackingId = -1 // Not Attack Status
                        });
                    }
                }
                FindTarget(replay, currentStep, monsterId);

                // Calculate the battle result
                CalculateBattle(replay, currentStep);

                // Save Battle Step
                replay.Steps.Add(JsonConvert.DeserializeObject<BattleStep>(JsonConvert.SerializeObject(currentStep)));

                prevStep = currentStep;
            } while (currentStep.Troops.Where(t => replay.Troops.Find(r => r.Id == t.TroopId).IsAttacker).Sum(t => t.Health) > 0
                    && currentStep.Troops.Where(t => !replay.Troops.Find(r => r.Id == t.TroopId).IsAttacker).Sum(t => t.Health) > 0);

            SyncBattleStatus(replay, currentStep, attackerPower, defenderPower);
        }

        public void AttackPlayer(BattlePower defenderPower, BattleReplay replay, Action<string> log = null)
        {
            var attackerPower = this;
            replay.DateTime = DateTime.UtcNow;

            int gateId = GenerateTroops(replay, attackerPower, defenderPower, BattleTroopType.Gate);

            replay.Steps = new List<BattleStep>();

            BattleStep prevStep = null;
            BattleStep currentStep = new BattleStep();

            do
            {
                // Find Targets
                if (prevStep == null) {
                    foreach (var troop in replay.Troops)
                    {
                        currentStep.Troops.Add(new BattleStatus()
                        {
                            TroopId = troop.Id,
                            Health = troop.Health,
                            AttackingId = -1 // Not Attack Status
                        });
                    }
                }
                FindTarget(replay, currentStep, gateId);

                // Calculate the battle result
                CalculateBattle(replay, currentStep);

                // Save Battle Step
                replay.Steps.Add(JsonConvert.DeserializeObject<BattleStep>(JsonConvert.SerializeObject(currentStep)));

                prevStep = currentStep;
            } while (currentStep.Troops.Where(t => replay.Troops.Find(r => r.Id == t.TroopId).IsAttacker).Sum(t => t.Health) > 0 
                    && currentStep.Troops.Where(t => !replay.Troops.Find(r => r.Id == t.TroopId).IsAttacker).Sum(t => t.Health) > 0);

            SyncBattleStatus(replay, currentStep, attackerPower, defenderPower);
        }

        public void SyncBattleStatus(BattleReplay replay, BattleStep currentStep, BattlePower attackerPower, BattlePower defenderPower)
        {
            for (int i = 0, len = currentStep.Troops.Count; i < len; i++)
            {
                var troop = currentStep.Troops[i];
                var battleTroop = replay.Troops.Find(e => e.Id == troop.TroopId);

                if (battleTroop.TroopMainType == BattleTroopType.Gate)
                {
                    defenderPower.GateHP = (int)troop.Health;
                } 
                else if (battleTroop.TroopMainType == BattleTroopType.Monster)
                {
                    float hp = troop.Health;
                    var details = (battleTroop.IsAttacker ? attackerPower : defenderPower).TroopChanges.Where(e => e.MonsterType == battleTroop.MonsterType);

                    if (hp == 0)
                    {
                        foreach (var detail in details) detail.MonsterHP = 0;
                    } else
                    {
                        float removedHP = details.Sum(e => e.TotalHP) - hp;

                        while (removedHP > 0)
                        {
                            foreach (var detail in details)
                            {
                                var totalHP = detail.MonsterHP * (float)random.NextDouble();

                                if (totalHP >= removedHP)
                                {
                                    detail.MonsterHP -= (int)removedHP;
                                    removedHP = 0;
                                    break;
                                } else {
                                    detail.MonsterHP -= (int)totalHP;
                                    removedHP -= totalHP;
                                }
                            }
                        }
                    }
                }
                else if (battleTroop.TroopMainType == BattleTroopType.Troop)
                {
                    float hp = troop.Health;
                    var details = (battleTroop.IsAttacker ? attackerPower : defenderPower).TroopChanges.Where(e => e.TroopType == battleTroop.TroopType);

                    if (hp == 0)
                    {
                        foreach (var detail in details) detail.Count = 0;
                    }
                    else
                    {
                        float removedHP = details.Sum(e => e.TotalHP) - hp;

                        while (removedHP > 0)
                        {
                            foreach (var detail in details)
                            {
                                var totalHP = detail.TotalHP * (float)random.NextDouble();

                                if (totalHP >= removedHP)
                                {
                                    detail.Count -= (int)(removedHP / detail.Data.Health);
                                    removedHP = 0;
                                    break;
                                } else {
                                    detail.Count -= (int)(totalHP / detail.Data.Health);
                                    removedHP -= totalHP;
                                }
                            }
                        }
                    }
                }
            }
        }

        public int GenerateTroops(BattleReplay replay, BattlePower attackerPower, BattlePower defenderPower, BattleTroopType battleType)
        {
            // Calculate Multipliers
            Dictionary<TroopType, (float attack, float defense)> attackMulti = new Dictionary<TroopType, (float, float)>();
            Dictionary<TroopType, (float attack, float defense)> defenseMulti = new Dictionary<TroopType, (float, float)>();

            foreach (TroopType type in Enum.GetValues(typeof(TroopType)))
            {
                attackMulti[type] = GetMultiplier(attackerPower.Multipliers, type);
                defenseMulti[type] = GetMultiplier(defenderPower.Multipliers, type);
            }

            int troopId = 1;
            replay.Troops = new List<BattleTroopInfo>();

            foreach (var troop in attackerPower.Troops)
            {
                var details = attackerPower.TroopChanges.FindAll(t => t.TroopType == troop.TroopType);
                replay.Troops.Add(new BattleTroopInfo()
                {
                    Id = troopId++,
                    TroopMainType = BattleTroopType.Troop,
                    TroopType = troop.TroopType,
                    Attack = details.Sum(e => e.Data.AttackDamage * e.Count),
                    Defense = details.Sum(e => e.Data.Defense * e.Count),
                    Health = details.Sum(e => e.Data.Health * e.Count),
                    Count = details.Sum(e => e.Count),
                    IsAttacker = true,
                    AttackMultiplier = attackMulti[troop.TroopType].attack,
                    DefenseMultiplier = attackMulti[troop.TroopType].defense,
                });
            }

            if (battleType == BattleTroopType.Gate)
            {
                int gateId = troopId++;

                replay.Troops.Add(new BattleTroopInfo()
                {
                    Id = gateId,
                    TroopMainType = BattleTroopType.Gate,
                    Attack = 0,
                    Defense = 0,
                    Health = defenderPower.GateHP,
                    Count = 1,
                    IsAttacker = false,
                    AttackMultiplier = 1,
                    DefenseMultiplier = 1
                });

                foreach (var troop in defenderPower.Troops)
                {
                    var details = defenderPower.TroopChanges.FindAll(t => t.TroopType == troop.TroopType).ToList();
                    if (details.Sum(e => e.Count) > 0)
                    {
                        replay.Troops.Add(new BattleTroopInfo()
                        {
                            Id = troopId++,
                            TroopMainType = BattleTroopType.Troop,
                            TroopType = troop.TroopType,
                            Attack = details.Sum(e => e.Data.AttackDamage * e.Count),
                            Defense = details.Sum(e => e.Data.Defense * e.Count),
                            Health = details.Sum(e => e.Data.Health * e.Count),
                            Count = details.Sum(e => e.Count),
                            IsAttacker = false,
                            AttackMultiplier = defenseMulti[troop.TroopType].attack,
                            DefenseMultiplier = defenseMulti[troop.TroopType].defense,
                        });
                    }
                }

                return gateId;
            } else
            {
                int monsterId = troopId++;
                var detail = defenderPower.TroopChanges.First();

                replay.Troops.Add(new BattleTroopInfo()
                {
                    Id = monsterId,
                    TroopMainType = BattleTroopType.Monster,
                    MonsterType = detail.MonsterType,
                    Attack = detail.Data.AttackDamage,
                    Defense = detail.Data.Defense,
                    Health = detail.Data.Health,
                    Count = 1,
                    IsAttacker = false,
                    AttackMultiplier = 1,
                    DefenseMultiplier = 1
                });

                return monsterId;
            }
        }

        public void FindTarget(BattleReplay replay, BattleStep currentStep, int gateId)
        {
            var attackerTroops = currentStep.Troops.Where(t => replay.Troops.Find(r => r.Id == t.TroopId).IsAttacker).ToList();
            var defenderTroops = currentStep.Troops.Where(t => !replay.Troops.Find(r => r.Id == t.TroopId).IsAttacker).ToList();

            int attackerTroopCount = attackerTroops.Where(t => t.Health > 0).Count();
            int defenderTroopCount = defenderTroops.Where(t => t.Health > 0).Count();

            BattleStatus gate = defenderTroops.Find(t => t.TroopId == gateId);
            foreach (var troop in currentStep.Troops)
            {
                bool isAttacker = attackerTroops.Contains(troop);

                if (troop.Health == 0)
                {
                    troop.AttackingId = -1;
                    continue;
                }

                if (troop.AttackingId == -1 || currentStep.Troops.Find(e => e.TroopId == troop.AttackingId).Health == 0)
                {
                    int index = random.Next(isAttacker ? defenderTroopCount : attackerTroopCount);
                    if (gate.Health > 0)
                    {
                        if (isAttacker) troop.AttackingId = gateId;
                        else
                        {
                            var attackTroop = replay.Troops.Find(r => r.Id == troop.TroopId);
                            TroopType? type = attackTroop.TroopType;

                            if (attackTroop.TroopMainType == BattleTroopType.Monster || 
                                (attackTroop.TroopMainType == BattleTroopType.Troop && (type == TroopType.Archer || type == TroopType.Slingshot)))
                            {
                                troop.AttackingId = attackerTroops.FindAll(t => t.Health > 0).ElementAt(index).TroopId;
                            }
                            else
                            {
                                troop.AttackingId = -1;
                            }
                        }
                    }
                    else
                    {
                        troop.AttackingId = (isAttacker ? defenderTroops : attackerTroops).FindAll(t => t.Health > 0).ElementAt(index).TroopId;
                    }
                }
            }
        }
        /*
        public static void CalculateBattleWithMonster(BattlePower attackPower, BattlePower defensePower,
            Dictionary<TroopType, (float attack, float defense)> attackMulti, 
            List<TroopInfoWithOpposite> attackTroops, TroopInfoWithOpposite monster)
        {
            var attack = attackTroops.Sum(e => e.Attack * attackMulti[e.TroopType].attack);
            float battleTime = monster.Health / attack;
            bool monsterDestroyed = true;

            var oppositeTroop = attackTroops[monster.OppositeId];
            var damage = monster.Attack - oppositeTroop.Defense * attackMulti[oppositeTroop.TroopType].defense;
            if (damage > 0)
            {
                float time = oppositeTroop.Health / damage;
                if (battleTime > time)
                {
                    battleTime = time;
                    monsterDestroyed = false;
                }
            }

            if (monsterDestroyed)
            {
                var detail = attackPower.TroopChanges.Where(e => e.TroopType == oppositeTroop.TroopType).First();
                detail.Count -= (int)((damage * battleTime) / detail.Data.Health);
                defensePower.TroopChanges.First().Count = 0;
            } else
            {
                attackPower.TroopChanges.Where(e => e.TroopType == oppositeTroop.TroopType).First().Count = 0;
                defensePower.TroopChanges.First().MonsterHP += battleTime * attack;
            }
        }*/

        public static void CalculateBattle(BattleReplay replay, BattleStep step)
        {
            float minTime = float.MaxValue;
            int minId = 0;

            for (int i = 0, len = step.Troops.Count; i < len; i++)
            {
                var troop = step.Troops[i];
                var oppositeTroops = step.Troops.Where(e => e.AttackingId == troop.TroopId).ToList();

                var damage = oppositeTroops.Sum(e => replay.Troops.Find(t => t.Id == e.TroopId).RealAttack) - replay.Troops.Find(t => t.Id == troop.TroopId).RealDefense;
                if (damage <= 0) continue;

                float time = troop.Health / damage;
                if (minTime > time)
                {
                    minTime = time;
                    minId = i;
                }
            }

            for (int i = 0, len = step.Troops.Count; i < len; i++)
            {
                var troop = step.Troops[i];
                var oppositeTroops = step.Troops.Where(e => e.AttackingId == troop.TroopId).ToList();

                var damage = oppositeTroops.Sum(e => replay.Troops.Find(t => t.Id == e.TroopId).RealAttack) - replay.Troops.Find(t => t.Id == troop.TroopId).RealDefense;
                if (damage <= 0) continue;

                if (i == minId)
                {
                    troop.Health = 0;
                } else
                {
                    troop.Health -= damage * minTime;
                }
            }
        }
        /*
        public static void CalculateBattle(BattlePower attackPower, BattlePower defensePower, 
            Dictionary<TroopType, (float attack, float defense)> attackMulti, Dictionary<TroopType, (float attack, float defense)> defenseMulti,
            List<TroopInfoWithOpposite> attackTroops, List<TroopInfoWithOpposite> defenseTroops, int GateHP)
        {
            // Calculate when they attack the Gate
            if (GateHP > 0)
            {
                var attack = attackTroops.Sum(e => e.Attack * attackMulti[e.TroopType].attack);
                float minTime = (float)GateHP / attack;
                int minId = -1;

                for (int i = 0, len = attackTroops.Count; i < len; i ++)
                {
                    var troop = attackTroops[i];
                    var oppositeTroops = defenseTroops.Where(e => e.OppositeId == i).ToList();
                    var damage = oppositeTroops.Sum(e => e.Attack * defenseMulti[e.TroopType].attack) - troop.Defense * attackMulti[troop.TroopType].defense;
                    if (damage <= 0) continue;

                    float time = troop.Health / damage;
                    if (minTime > time)
                    {
                        minTime = time;
                        minId = i;
                    }
                }

                if (minId == -1) // Gate Destroyed
                {
                    defensePower.GateHP = 0;
                } else
                {
                    defensePower.GateHP -= (int)(minTime * attack);
                }

                RemoveTroops(attackPower, defensePower, attackMulti, defenseMulti, attackTroops, defenseTroops, minTime);
            } else
            {
                float minTime = float.MaxValue;

                for (int i = 0, len = attackTroops.Count; i < len; i++)
                {
                    var troop = attackTroops[i];
                    var oppositeTroops = defenseTroops.Where(e => e.OppositeId == i).ToList();
                    var damage = oppositeTroops.Sum(e => e.Attack * defenseMulti[e.TroopType].attack) - troop.Defense * attackMulti[troop.TroopType].defense;
                    if (damage <= 0) continue;

                    float time = troop.Health / damage;
                    if (minTime > time) minTime = time;
                }

                for (int i = 0, len = defenseTroops.Count; i < len; i++)
                {
                    var troop = defenseTroops[i];
                    var oppositeTroops = attackTroops.Where(e => e.OppositeId == i).ToList();
                    var damage = oppositeTroops.Sum(e => e.Attack * attackMulti[e.TroopType].attack) - troop.Defense * defenseMulti[troop.TroopType].defense;
                    if (damage <= 0) continue;

                    float time = troop.Health / damage;
                    if (minTime > time) minTime = time;
                }

                RemoveTroops(attackPower, defensePower, attackMulti, defenseMulti, attackTroops, defenseTroops, minTime);
            }
        }

        public static void RemoveTroops(BattlePower attackPower, BattlePower defensePower,
            Dictionary<TroopType, (float attack, float defense)> attackMulti, Dictionary<TroopType, (float attack, float defense)> defenseMulti,
            List<TroopInfoWithOpposite> attackTroops, List<TroopInfoWithOpposite> defenseTroops, float time)
        {
            for (int i = 0, len = attackTroops.Count; i < len; i++)
            {
                var troop = attackTroops[i];
                var oppositeTroops = defenseTroops.Where(e => e.OppositeId == i).ToList();
                var damage = oppositeTroops.Sum(e => e.Attack * defenseMulti[e.TroopType].attack) - troop.Defense * attackMulti[troop.TroopType].defense;
                if (damage <= 0) continue;

                var hp = damage * time;

                foreach (var detail in attackPower.TroopChanges.Where(e => e.TroopType == troop.TroopType))
                {
                    var totalHP = detail.TotalHP;

                    if (totalHP >= hp)
                    {
                        detail.Count -= (int)((totalHP - hp) / detail.Data.Health);
                        break;
                    }

                    detail.Count = 0;
                    hp -= totalHP;
                }
            }

            for (int i = 0, len = defenseTroops.Count; i < len; i++)
            {
                var troop = defenseTroops[i];
                var oppositeTroops = attackTroops.Where(e => e.OppositeId == i).ToList();
                var damage = oppositeTroops.Sum(e => e.Attack * attackMulti[e.TroopType].attack) - troop.Defense * defenseMulti[troop.TroopType].defense;
                if (damage <= 0) continue;

                var hp = damage * time;

                foreach (var detail in defensePower.TroopChanges.Where(e => e.TroopType == troop.TroopType))
                {
                    var totalHP = detail.TotalHP;

                    if (totalHP >= hp)
                    {
                        detail.Count -= (int)((totalHP - hp) / detail.Data.Health);
                        break;
                    }

                    detail.Count = 0;
                    hp -= totalHP;
                }
            }
        }*/

        public static (float attack, float defense) GetMultiplier(List<AttackDefenseMultiplier> multipliers, TroopType troopType)
        {
            float attackMultiplier = 1;
            float defenseMultiplier = 1;

            if (multipliers != null)
            {

                var multies = multipliers.FindAll(x => (x.Troop != null && x.Troop.TroopType == troopType));
                foreach (var multi in multies)
                {
                    attackMultiplier += (multi.AttackMultiplier - 1);
                    defenseMultiplier += (multi.DefenseMultiplier - 1);
                }

                var globalMulti = multipliers.Find(x => (x.Troop == null));
                if (globalMulti != null)
                {
                    attackMultiplier += (globalMulti.AttackMultiplier - 1);
                    defenseMultiplier += (globalMulti.DefenseMultiplier - 1);
                }
            }

            return (attackMultiplier, defenseMultiplier);
        }
    }

    [Serializable]
    public class AttackDefenseMultiplier
    {
        public TroopInfos Troop { get; set; }
        public float AttackMultiplier { get; set; } = 1;
        public float DefenseMultiplier { get; set; } = 1;
    }

    [DataContract]
    public class ClientBattleReport
    {
        [DataMember(EmitDefaultValue = false)]
        public EntityType EntityType { get; set; }
        [DataMember]
        public int EntityId { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string EntityName { get; set; }

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
        public long ReplayDataId { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<TroopDetailsPvP> TroopChanges { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<HeroData> Heroes { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<ItemData> Items { get; set; }
    }

    [Serializable]
    public class ItemData
    {
        public int ItemId { get; set; }
        public int Count { get; set; }
        public ItemData(int id, int cnt)
        {
            ItemId = id;
            Count = cnt;
        }
    }

    [DataContract]
    public class TroopDetailsPvP
    {
        [DataMember]
        public BattleTroopType TroopMainType { get; set; }
        [DataMember]
        public TroopType TroopType { get; set; }
        [DataMember]
        public MonsterType MonsterType { get; set; }
        [DataMember]
        public int Level { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public int Dead { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public int Wounded { get; set; }
        [DataMember]
        public int Count { get; set; }
        [DataMember]
        public int InitialCount { get; set; }
        [DataMember]
        public int MonsterHP { get; set; }

        public TroopInfos Troop { get; set; }
        public IReadOnlyTroopDataTable Data { get; set; }
        public float TotalHP => TroopMainType == BattleTroopType.Troop ? Data.Health * Count : MonsterHP;
        public float RemainUnits => (Data.Health > 0)? (TotalHP / Data.Health) : 0;
        public int LoadPerUnit => Data.WeightLoad;

        public TroopDetailsPvP()
        {
        }

        public TroopDetailsPvP(TroopInfos troop, int count, IReadOnlyTroopDataTable data)
        {
            TroopMainType = BattleTroopType.Troop;
            TroopType = troop.TroopType;
            Level = data.Level;

            Troop = troop;
            Data = data;
            Count = InitialCount = count;
        }
    }
}

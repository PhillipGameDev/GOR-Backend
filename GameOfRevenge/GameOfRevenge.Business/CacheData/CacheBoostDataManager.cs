using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameOfRevenge.Business.Manager.GameDef;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Boost;
using GameOfRevenge.Common.Models.Inventory;

namespace GameOfRevenge.Business.CacheData
{
    public static class CacheBoostDataManager
    {
        private const string BoostNotExist = "Boost does not exist";
        private static bool isLoaded = false;
        //        private static List<BoostTypeRel> boostInfos = null;
//        private static List<BoostTypeTable> boostInfos = null;
//        private static List<CityBoostType> cityBoostTypes;

        private static List<NewBoostType> newBoostTypes;
//        private static List<SpecNewBoostData> specNewBoostDatas;
        private static SpecNewBoostDataTable newBoosts;

//        private static SpecVIPBoostData vipBoosts;
//        private static List<VIPBoostType> vipBoostTypes;

//        public static IReadOnlyList<IReadOnlyBoostTypeTable> BoostInfos { get { LoadCacheMemory(); return boostInfos.ToList(); } }

//        public static SpecVIPBoostData SpecVIPBoostDatas { get { LoadCacheMemory(); return vipBoosts; } }

        public static SpecNewBoostDataTable SpecNewBoostDataTables { get { LoadCacheMemory(); return newBoosts; } }
        public static IReadOnlyList<SpecNewBoostData> SpecNewBoostDatas { get { LoadCacheMemory(); return newBoosts.Boosts.ToList(); } }
//        public static IReadOnlyList<SpecNewBoostData> SpecNewBoostDatas { get { LoadCacheMemory(); return specNewBoostDatas.ToList(); } }

        public static List<CityBoostType> CityBoostTypes { get { LoadCacheMemory(); return newBoosts.CityBoosts; } }

        public static List<VIPBoostType> VIPBoostTypes { get { LoadCacheMemory(); return newBoosts.VIPBoosts; } }

        //        public static IReadOnlyList<IReadOnlyBoostTypeRel> BoostInfos { get { if (boostInfos == null) LoadCacheMemory(); return boostInfos.ToList(); } }

        //        public static IReadOnlyList<IReadOnlyBuffItemRel> BuffItemRelations { get { LoadCacheMemory(); return boostTypes; } }

        public static SpecNewBoostData GetNewBoostByTypeId(int id)
        {
            return SpecNewBoostDataTables.Boosts.Find(x => ((int)x.Type == id));
        }

/*        public static IReadOnlyBoostTypeTable GetFullBoostDataByTypeId(int boostId)
        {
            var data = BoostInfos.FirstOrDefault(x => x.BoostTypeId == boostId);
            if (data == null) throw new CacheDataNotExistExecption(BoostNotExist);
            else return data;
        }*/

        /*        public static IReadOnlyBoostTypeTable GetFullBoostDataByBoostId(int boostId)
                {
                    var data = BoostInfos.FirstOrDefault(x => x.Values.Where(y => y.BoostId == boostId) != null);
                    if (data == null) throw new CacheDataNotExistExecption(StructureNotExist);
                    else return data;
                }*/

/*        public static IReadOnlyBoostTypeTable GetFullBoostDataByType(NewBoostType boostType)
        {
            var data = BoostInfos.FirstOrDefault(x => x.BoostType == boostType);
            if (data == null) throw new CacheDataNotExistExecption(BoostNotExist);
            else return data;
        }*/

        public static SpecNewBoostData GetNewBoostDataByType(NewBoostType boostType)
        {
//            var data = specNewBoostDatas.Find(x => x.Type == boostType);
            var data = newBoosts.Boosts.Find(x => (x.Type == boostType));
            if (data == null) throw new CacheDataNotExistExecption(BoostNotExist);

            return data;
        }

        public static bool IsTimedTech(NewBoostTech boostTech)
        {
            bool resp = false;

            switch(boostTech)
            {
                case NewBoostTech.CityWallDefensePower: resp = true; break;
                case NewBoostTech.ProtectionFog: resp = true; break;
            }

            return resp;
        }

        /*        public static IReadOnlyBoostTable GetBoostIdByValueData(int boostId, int percentage)
                {
                    var data = BoostInfos.FirstOrDefault(x => x.Info.BoostTypeId == boostId)?.Values.FirstOrDefault(x=>x.Percentage == percentage);
                    if (data == null) throw new CacheDataNotExistExecption(StructureNotExist);
                    else return data;
                }

                public static IReadOnlyBoostTable GetBoostIdByValueData(BoostType boostType, int percentage)
                {
                    var data = BoostInfos.FirstOrDefault(x => x.Info.BoostType == boostType)?.Values.FirstOrDefault(x => x.Percentage == percentage);
                    if (data == null) throw new CacheDataNotExistExecption(StructureNotExist);
                    else return data;
                }*/

        /*        private static List<BuffItemRel> LoadBuffs()
                {
                    try
                    {
                        var buffs = Enum.GetNames(typeof(BuffType));
                        var invs = Enum.GetNames(typeof(InventoryItemType));
                        var datas = new List<BuffItemRel>();

                        for (int i = 0; i < invs.Length; i++)
                        {
                            for (int j = 0; j < buffs.Length; j++)
                            {
                                if (buffs[j].Equals(invs[i]))
                                {
                                    var data = new BuffItemRel()
                                    {
                                        InventoryType = (InventoryItemType)i,
                                        BuffType = (BuffType)j
                                    };

                                    datas.Add(data);
                                }
                            }
                        }

                        return datas;
                    }
                    catch (Exception ex)
                    {
                        ClearCache();
                        throw new CacheDataNotExistExecption(ex.Message, ex);
                    }
                }*/

        public static void LoadCacheMemory()
        {
            if (isLoaded) return;

            isLoaded = false;

//            var resManager = new BoostManager();
//            var response = await resManager.GetAllBoostTypes();// GetAllBoostRelData();

//            if (response.IsSuccess)
            {
                //                boostTypes = LoadBuffs();
//                boostInfos = response.Data;

                newBoostTypes = new List<NewBoostType>((NewBoostType[])Enum.GetValues(typeof(NewBoostType)));
                newBoostTypes.Remove(NewBoostType.Unknown);
/*                specNewBoostDatas = new List<SpecNewBoostData>();
                specNewBoostDatas.Add(GetSpecForCityBoostShield());
                specNewBoostDatas.Add(GetSpecForCityBoostBlessing());
                specNewBoostDatas.Add(GetSpecForCityBoostLifeSaver());
                specNewBoostDatas.Add(GetSpecForCityBoostFog());
                specNewBoostDatas.Add(GetSpecForCityBoostTechBoost());
                specNewBoostDatas.Add(GetSpecForCityBoostProductionBoost());*/

                newBoosts = new SpecNewBoostDataTable();
                //city boosts
                newBoosts.CityBoosts = new List<CityBoostType>((CityBoostType[])Enum.GetValues(typeof(CityBoostType)));
                newBoosts.CityBoosts.Remove(CityBoostType.Unknown);

                newBoosts.Boosts.Add(GetSpecForCityBoostShield());
                newBoosts.Boosts.Add(GetSpecForCityBoostBlessing());
                newBoosts.Boosts.Add(GetSpecForCityBoostLifeSaver());
                newBoosts.Boosts.Add(GetSpecForCityBoostProductionBoost());
                newBoosts.Boosts.Add(GetSpecForCityBoostTechBoost());
                newBoosts.Boosts.Add(GetSpecForCityBoostFog());

                //VIP boosts
                newBoosts.VIPBoosts = new List<VIPBoostType>((VIPBoostType[])Enum.GetValues(typeof(VIPBoostType)));
                newBoosts.VIPBoosts.Remove(VIPBoostType.Unknown);

                newBoosts.Boosts.Add(GetSpecForVIPBoosts());

                //kingdom technologies
                newBoosts.Boosts.Add(GetSpecForConstructionTechnology());
                newBoosts.Boosts.Add(GetSpecForUpkeepReductionTechnology());
                newBoosts.Boosts.Add(GetSpecForHealingSpeedTechnology());
                newBoosts.Boosts.Add(GetSpecForResearchSpeedTechnology());
                newBoosts.Boosts.Add(GetSpecForTroopLoadTechnology());
                newBoosts.Boosts.Add(GetSpecForTrainingSpeedTechnology());
                newBoosts.Boosts.Add(GetSpecForInfirmaryCapacityTechnology());
                newBoosts.Boosts.Add(GetSpecForStaminaRecoveryTechnology());
                newBoosts.Boosts.Add(GetSpecForResourceStorageTechnology());

                //attack technologies
                newBoosts.Boosts.Add(GetSpecForInfantryAttackTechnology());
                newBoosts.Boosts.Add(GetSpecForCavalryAttackTechnology());
                newBoosts.Boosts.Add(GetSpecForSiegeAttackTechnology());
                newBoosts.Boosts.Add(GetSpecForBowmenAttackTechnology());

                //defense technologies
                newBoosts.Boosts.Add(GetSpecForInfantryDefenseTechnology());
                newBoosts.Boosts.Add(GetSpecForCavalryDefenseTechnology());
                newBoosts.Boosts.Add(GetSpecForSiegeDefenseTechnology());
                newBoosts.Boosts.Add(GetSpecForBowmenDefenseTechnology());

                var dic = new Dictionary<byte, Dictionary<byte, object>>();
                foreach (var boost in newBoosts.Boosts)
                {
                    if ((boost.Table > 0) && !dic.ContainsKey(boost.Table))
                    {
                        dic.Add(boost.Table, boost.Levels);
                    }
                    foreach (var tech in boost.Techs)
                    {
                        if ((tech.Table == 0) || dic.ContainsKey(tech.Table)) continue;

                        dic.Add(tech.Table, tech.Levels);
                    }
                }


/*                var vipBoosts = GetSpecForVIPBoosts();

                {
                    if ((vipBoosts.Table > 0) && !dic.ContainsKey(vipBoosts.Table))
                    {
                        dic.Add(vipBoosts.Table, vipBoosts.Levels);
                    }
                    foreach (var tech in vipBoosts.Techs)
                    {
                        if ((tech.Table == 0) || dic.ContainsKey(tech.Table)) continue;

                        dic.Add(tech.Table, tech.Levels);
                    }
                }*/
                var keys = dic.Keys.ToList();
                keys.Sort();
                foreach (var key in keys)
                {
                    newBoosts.Tables.Add(key, dic[key]);
                }

                isLoaded = true;
            }
//            else
//            {
//                throw new CacheDataNotExistExecption(response.Message);
//            }
        }

        public static void ClearCache()
        {
/*            if (cityBoostTypes != null)
            {
                cityBoostTypes.Clear();
                cityBoostTypes = null;
            }*/

/*            if (boostInfos != null)
            {
                boostInfos.Clear();
                boostInfos = null;
            }*/

            if (newBoostTypes != null)
            {
                newBoostTypes.Clear();
                newBoostTypes = null;
            }

            if (newBoosts != null)
            {
                newBoosts.Boosts.Clear();
                newBoosts.Tables.Clear();
                newBoosts = null;
            }

/*            if (specNewBoostDatas != null)
            {
                specNewBoostDatas.Clear();
                specNewBoostDatas = null;
            }*/

            isLoaded = false;
        }

        private static SpecNewBoostData GetSpecForCityBoostShield()
        {
            var techs = new List<NewBoostTechSpec>
            {
                new NewBoostTechSpec(NewBoostTech.ProtectionShield, 0, null)//, 1, GetTable(1), "Time", @"d'd 'hh\:mm\:ss")
            };

            return new SpecNewBoostData(NewBoostType.Shield, techs, 1, Table1);//, null);//levels);
        }

        private static SpecNewBoostData GetSpecForCityBoostBlessing()
        {
            var techs = new List<NewBoostTechSpec>
            {
                new NewBoostTechSpec(NewBoostTech.TroopAttackMultiplier, 10, Table10, "Atk+{0:F0}%"),
                new NewBoostTechSpec(NewBoostTech.TroopDefenseMultiplier, 10, Table10, "Def+{0:F0}%"),
                new NewBoostTechSpec(NewBoostTech.CityTroopAttackMultiplier, 10, Table10),
                new NewBoostTechSpec(NewBoostTech.CityTroopDefenseMultiplier, 10, Table10)
            };

            return new SpecNewBoostData(NewBoostType.Blessing, techs, 1, Table1);
        }

        private static SpecNewBoostData GetSpecForCityBoostLifeSaver()
        {
            var techs = new List<NewBoostTechSpec>
            {
                new NewBoostTechSpec(NewBoostTech.InfirmaryCapacityAmount, 2, Table2, "+{0:N0}", "Capacity")
            };

            return new SpecNewBoostData(NewBoostType.LifeSaver, techs, 1, Table1);
        }

        private static SpecNewBoostData GetSpecForCityBoostFog()
        {
            var techs = new List<NewBoostTechSpec>
            {
                new NewBoostTechSpec(NewBoostTech.ProtectionFog, 0, null)//, "Time", @"d'd 'hh\:mm\:ss")
            };

            return new SpecNewBoostData(NewBoostType.Fog, techs, 20, Table20);
        }

        private static SpecNewBoostData GetSpecForCityBoostTechBoost()
        {
            var techs = new List<NewBoostTechSpec>
            {
                new NewBoostTechSpec(NewBoostTech.ResearchSpeedMultiplier, 11, Table11, "+{0:F0}%")
            };

            return new SpecNewBoostData(NewBoostType.TechBoost, techs, 1, Table1);
        }

        private static SpecNewBoostData GetSpecForCityBoostProductionBoost()
        {
            var techs = new List<NewBoostTechSpec>
            {
                new NewBoostTechSpec(NewBoostTech.ResourceProductionMultiplier, 11, Table11, "+{0:F0}%")
            };

            return new SpecNewBoostData(NewBoostType.ProductionBoost, techs, 1, Table1);
        }




        private static SpecNewBoostData GetSpecForVIPBoosts()
        {
            var techs = new List<NewBoostTechSpec>
            {
                //vip 1~
                new NewBoostTechSpec().VIPBoostTechSpec((NewBoostTech)VIPBoostTech.ResourceProductionMultiplier, 71, Table71, 1, "+{0:N0}%"),
                new NewBoostTechSpec().VIPBoostTechSpec((NewBoostTech)VIPBoostTech.BuildingTimeBonus, 72, Table72, 1, "{T1}"),
                //vip 2~
                new NewBoostTechSpec().VIPBoostTechSpec((NewBoostTech)VIPBoostTech.TroopRecoverySpeedMultiplier, 73, Table73, 2, "+{0:N0}%"),
                //vip 9~
                new NewBoostTechSpec().VIPBoostTechSpec((NewBoostTech)VIPBoostTech.TroopAttackMultiplier, 74, Table74, 9, "Atk+{0:N0}%"),
                new NewBoostTechSpec().VIPBoostTechSpec((NewBoostTech)VIPBoostTech.TroopDefenseMultiplier, 75, Table75, 9, "Def+{0:N0}%"),
                //vip 10~
                new NewBoostTechSpec().VIPBoostTechSpec((NewBoostTech)VIPBoostTech.InfirmaryCapacityMultiplier, 76, Table76, 10, "+{0:N0}%"),
                //vip 11~
                new NewBoostTechSpec().VIPBoostTechSpec((NewBoostTech)VIPBoostTech.TroopTrainingSpeedMultiplier, 77, Table77, 11, "+{0:N0}%"),
                //vip 14~
                new NewBoostTechSpec().VIPBoostTechSpec((NewBoostTech)VIPBoostTech.ResourceStorageMultiplier, 78, Table78, 14, "+{0:N0}%")
            };

            return new SpecNewBoostData(NewBoostType.VIP, techs, 70, Table70);
        }




        private static SpecNewBoostData GetSpecForConstructionTechnology()
        {
            var techs = new List<NewBoostTechSpec>
            {
                new NewBoostTechSpec(NewBoostTech.BuildingSpeedMultiplier, 61, Table61, "+{0:F0}%")
            };

            return new SpecNewBoostData(NewBoostType.Construction, techs);
        }

        private static SpecNewBoostData GetSpecForUpkeepReductionTechnology()
        {
            var techs = new List<NewBoostTechSpec>
            {
                new NewBoostTechSpec(NewBoostTech.TroopUpkeepReductionMultiplier, 61, Table61, "-{0:F0}%")
            };

            return new SpecNewBoostData(NewBoostType.UpkeepReduction, techs);
        }

        private static SpecNewBoostData GetSpecForHealingSpeedTechnology()
        {
            var techs = new List<NewBoostTechSpec>
            {
                new NewBoostTechSpec(NewBoostTech.TroopRecoverySpeedMultiplier, 61, Table61, "+{0:F0}%")
            };

            return new SpecNewBoostData(NewBoostType.HealingSpeed, techs);
        }

        private static SpecNewBoostData GetSpecForResearchSpeedTechnology()
        {
            var techs = new List<NewBoostTechSpec>
            {
                new NewBoostTechSpec(NewBoostTech.ResearchSpeedMultiplier, 61, Table61, "+{0:F0}%")
            };

            return new SpecNewBoostData(NewBoostType.ResearchSpeed, techs);
        }

        private static SpecNewBoostData GetSpecForTroopLoadTechnology()
        {
            var techs = new List<NewBoostTechSpec>
            {
                new NewBoostTechSpec(NewBoostTech.TroopLoadMultiplier, 61, Table61, "+{0:F0}%")
            };

            return new SpecNewBoostData(NewBoostType.TroopLoad, techs);
        }

        private static SpecNewBoostData GetSpecForTrainingSpeedTechnology()
        {
            var techs = new List<NewBoostTechSpec>
            {
                new NewBoostTechSpec(NewBoostTech.TroopTrainingSpeedMultiplier, 61, Table61, "+{0:F0}%")
            };

            return new SpecNewBoostData(NewBoostType.TrainingSpeed, techs);
        }

        private static SpecNewBoostData GetSpecForInfirmaryCapacityTechnology()
        {
            var techs = new List<NewBoostTechSpec>
            {
                new NewBoostTechSpec(NewBoostTech.InfirmaryCapacityMultiplier, 61, Table61, "+{0:F0}%")
            };

            return new SpecNewBoostData(NewBoostType.InfirmaryCapacity, techs);
        }

        private static SpecNewBoostData GetSpecForStaminaRecoveryTechnology()
        {
            var techs = new List<NewBoostTechSpec>
            {
                new NewBoostTechSpec(NewBoostTech.KingStaminaRecoverySpeedMultiplier, 61, Table61, "+{0:F0}%")
            };

            return new SpecNewBoostData(NewBoostType.StaminaRecovery, techs);
        }

        private static SpecNewBoostData GetSpecForResourceStorageTechnology()
        {
            var techs = new List<NewBoostTechSpec>
            {
                new NewBoostTechSpec(NewBoostTech.ResourceStorageMultiplier, 61, Table61, "+{0:F0}%")
            };

            return new SpecNewBoostData(NewBoostType.ResourceStorage, techs);
        }




        private static SpecNewBoostData GetSpecForInfantryAttackTechnology()
        {
            var techs = new List<NewBoostTechSpec>
            {
                new NewBoostTechSpec(NewBoostTech.InfantryAttackMultiplier, 60, Table60, "Atk+{0:F0}%"),
                new NewBoostTechSpec(NewBoostTech.InfantryAttackPower, 38, Table38, "Power+{0:N0}")
            };

            return new SpecNewBoostData(NewBoostType.InfantryAttack, techs);
        }

        private static SpecNewBoostData GetSpecForCavalryAttackTechnology()
        {
            var techs = new List<NewBoostTechSpec>
            {
                new NewBoostTechSpec(NewBoostTech.CavalryAttackMultiplier, 61, Table61, "Atk+{0:F0}%"),
                new NewBoostTechSpec(NewBoostTech.CavalryAttackPower, 40, Table40, "Power+{0:N0}")
            };

            return new SpecNewBoostData(NewBoostType.CavalryAttack, techs);
        }

        private static SpecNewBoostData GetSpecForSiegeAttackTechnology()
        {
            var techs = new List<NewBoostTechSpec>
            {
                new NewBoostTechSpec(NewBoostTech.SiegeAttackMultiplier, 61, Table61, "Atk+{0:F0}%"),
                new NewBoostTechSpec(NewBoostTech.SiegeAttackPower, 40, Table40, "Power+{0:N0}")
            };

            return new SpecNewBoostData(NewBoostType.SiegeAttack, techs);
        }

        private static SpecNewBoostData GetSpecForBowmenAttackTechnology()
        {
            var techs = new List<NewBoostTechSpec>
            {
                new NewBoostTechSpec(NewBoostTech.BowmenAttackMultiplier, 61, Table61, "Atk+{0:F0}%"),
                new NewBoostTechSpec(NewBoostTech.BowmenAttackPower, 40, Table40, "Power+{0:N0}")
            };

            return new SpecNewBoostData(NewBoostType.BowmenAttack, techs);
        }




        

        private static SpecNewBoostData GetSpecForInfantryDefenseTechnology()
        {
            var techs = new List<NewBoostTechSpec>
            {
                new NewBoostTechSpec(NewBoostTech.InfantryDefenseMultiplier, 60, Table60, "Def+{0:F0}%"),
                new NewBoostTechSpec(NewBoostTech.InfantryDefensePower, 38, Table38, "Power+{0:N0}")
            };

            return new SpecNewBoostData(NewBoostType.InfantryDefense, techs);
        }

        private static SpecNewBoostData GetSpecForCavalryDefenseTechnology()
        {
            var techs = new List<NewBoostTechSpec>
            {
                new NewBoostTechSpec(NewBoostTech.CavalryDefenseMultiplier, 60, Table60, "Def+{0:F0}%"),
                new NewBoostTechSpec(NewBoostTech.CavalryDefensePower, 42, Table42, "Power+{0:N0}")
            };

            return new SpecNewBoostData(NewBoostType.CavalryDefense, techs);
        }

        private static SpecNewBoostData GetSpecForSiegeDefenseTechnology()
        {
            var techs = new List<NewBoostTechSpec>
            {
                new NewBoostTechSpec(NewBoostTech.SiegeDefenseMultiplier, 60, Table60, "Def+{0:F0}%"),
                new NewBoostTechSpec(NewBoostTech.SiegeDefensePower, 42, Table42, "Power+{0:N0}")
            };

            return new SpecNewBoostData(NewBoostType.SiegeDefense, techs);
        }

        private static SpecNewBoostData GetSpecForBowmenDefenseTechnology()
        {
            var techs = new List<NewBoostTechSpec>
            {
                new NewBoostTechSpec(NewBoostTech.BowmenDefenseMultiplier, 60, Table60, "Def+{0:F0}%"),
                new NewBoostTechSpec(NewBoostTech.BowmenDefensePower, 42, Table42, "Power+{0:N0}")
            };

            return new SpecNewBoostData(NewBoostType.BowmenDefense, techs);
        }




/*
        private static int[] Table0 = new int[]//SHIELD
{
50,
72,
83,
92,
109,
122,
136,
150,
163,
177,
191,
204,
218,
232,
246,
259,
273,
287,
300,
314,
328,
341,
355,
369,
383,
396,
410,
424,
437,
451
};*/

        private static int[] Table1 = new int[]//NEW SHIELD
{
29 * MINUTES,
34 * MINUTES,
39 * MINUTES + 30,
45 * MINUTES,
50 * MINUTES + 40,
70 * MINUTES,
1 * DAY + 20 * MINUTES,
1 * DAY + 1 * HOUR + 45 * MINUTES,
1 * DAY + 2 * HOUR + 30 * MINUTES,
1 * DAY + 3 * HOUR + 30 * MINUTES,
1 * DAY + 4 * HOUR + 5 * MINUTES,
1 * DAY + 5 * HOUR + 5 * MINUTES,
1 * DAY + 6 * HOUR + 5 * MINUTES,
1 * DAY + 7 * HOUR + 5 * MINUTES,
1 * DAY + 8 * HOUR,
1 * DAY + 10 * HOUR,
1 * DAY + 13 * HOUR,
1 * DAY + 17 * HOUR,
1 * DAY + 20 * HOUR,
2 * DAY,
2 * DAY + 3 * HOUR,
2 * DAY + 9 * HOUR,
2 * DAY + 13 * HOUR,
2 * DAY + 17 * HOUR,
2 * DAY + 22 * HOUR,
3 * DAY + 4 * HOUR,
3 * DAY + 8 * HOUR,
3 * DAY + 11 * HOUR,
3 * DAY + 15 * HOUR,
4 * DAY
};

        private static int[] Table2 = new int[]//LIFESAVER
{
5000,
9000,
13000,
19000,
23000,
27600,
32200,
36800,
41400,
46000,
50600,
55200,
59800,
64400,
69000,
73600,
78200,
82800,
87400,
92000,
96600,
101200,
105800,
110400,
115000,
119600,
124200,
128800,
133400,
138000
};

        private static float[] Table10 = new float[]//BLESSING
{
5,
6,
9,
13,
13,
16,
18,
21,
23,
25,
28,
30,
32,
35,
37,
39,
41,
44,
46,
48,
51,
53,
55,
58,
60,
62,
64,
67,
69,
71
};

        private static float[] Table11 = new float[]//TECHBOOST, PRODUCTIONBOOST
{
3,
5,
7,
11,
13,
16,
18,
21,
23,
26,
29,
31,
34,
36,
39,
42,
44,
47,
49,
52,
55,
57,
60,
62,
65,
68,
70,
73,
75,
78
};

        private static int[] Table20 = new int[]//FOG
{
15 * MINUTES,
16 * MINUTES + 30,
17 * MINUTES + 40,
18 * MINUTES,
20 * MINUTES + 10,
23 * MINUTES,
25 * MINUTES + 50,
29 * MINUTES,
34 * MINUTES,
39 * MINUTES + 30,
45 * MINUTES,
50 * MINUTES + 40,
70 * MINUTES,
1 * DAY + 20 * MINUTES,
1 * DAY + 1 * HOUR + 45 * MINUTES,
1 * DAY + 2 * HOUR,
1 * DAY + 2 * HOUR + 30 * MINUTES,
1 * DAY + 3 * HOUR + 30 * MINUTES,
1 * DAY + 4 * HOUR + 5 * MINUTES,
1 * DAY + 8 * HOUR,
1 * DAY + 13 * HOUR,
1 * DAY + 17 * HOUR,
1 * DAY + 20 * HOUR,
2 * DAY,
2 * DAY + 9 * HOUR,
2 * DAY + 16 * HOUR,
2 * DAY + 22 * HOUR,
3 * DAY + 4 * HOUR,
3 * DAY + 15 * HOUR,
4 * DAY
};


        private static float[] Table30 = new float[]
{
10, 12, 14, 16, 18
};

        private static float[] Table31 = new float[]
{
20, 22, 24, 26, 28
};

        private static float[] Table32 = new float[]
{
25, 27, 29, 30, 33
};

        private static float[] Table33 = new float[]
{
25, 27, 29, 33, 36
};

        private static float[] Table34 = new float[]
{
29, 30, 33, 36, 40
};

        private static float[] Table35 = new float[]
{
30, 33, 36
};

        private static float[] Table36 = new float[]
{
33, 35, 37, 39, 41
};

        private static float[] Table37 = new float[]
{
40, 44, 48, 52, 56
};

        private static float[] Table38 = new float[]
{
40, 45, 50
};

        private static float[] Table39 = new float[]
{
52, 54, 56, 58
};


        private static float[] Table40 = new float[]
{
60, 62, 64
};

        private static float[] Table41 = new float[]
{
60, 62, 64, 66, 68
};

        private static float[] Table42 = new float[]
{
66, 68, 70
};

        private static float[] Table43 = new float[]
{
70, 72, 74, 76, 78
};

        private static float[] Table44 = new float[]
{
72, 74, 76, 78, 80
};

        private static float[] Table45 = new float[]
{
80, 82, 84, 86, 88
};

        private static float[] Table46 = new float[]
{
90, 92, 94, 96, 98
};

        private static float[] Table60 = new float[]
{
1, 1, 1, 1, 1
};

        private static float[] Table61 = new float[]
{
1, 2, 3, 4, 5
};



        private static int[] Table70 = new int[]//VIP POINTS
{
0,
130,
500,
1000,
2000,
3000,
6000,
11500,
22000,
30000,
45000,
70000,
125000,
170000,
210000,
280000,
330000,
390000,
440000,
500000
};

        private static float[] Table71 = new float[]//VIP PRODUCTIONBOOST
{
5,
6,
7,
8,
10,
12,
15,
18,
22,
30,
35,
40,
45,
50,
55,
60,
65,
70,
75,
80
};

        private static int[] Table72 = new int[]//VIP BUILDTIMEBONUS
{
6 * MINUTES,
8 * MINUTES,
10 * MINUTES,
12 * MINUTES,
15 * MINUTES,
20 * MINUTES,
30 * MINUTES,
45 * MINUTES,
60 * MINUTES,
90 * MINUTES,
120 * MINUTES,
150 * MINUTES,
180 * MINUTES,
210 * MINUTES,
240 * MINUTES,
240 * MINUTES,
240 * MINUTES,
240 * MINUTES,
240 * MINUTES,
240 * MINUTES
};

        private static int[] Table73 = new int[]//VIP HEALING MULTIPLIER
{
10,
12,
14,
16,
20,
24,
28,
32,
40,
45,
50,
55,
60,
65,
70,
70,
75,
80,
80
};

        private static int[] Table74 = new int[]//VIP TROOPATTACK
{
10,
15,
20,
20,
20,
20,
25,
25,
30,
30,
30,
30
};

        private static int[] Table75 = new int[]//VIP TROOPDEFENSE
{
10,
15,
15,
20,
20,
20,
25,
25,
30,
30,
30,
30
};

        private static int[] Table76 = new int[]//VIP INFIRMARYMULTIPLIER
{
30,
30,
30,
30,
30,
35,
35,
35,
35,
35,
35
};

        private static int[] Table77 = new int[]//VIP TROOPTRAINING
{
5,
10,
15,
20,
25,
25,
25,
25,
25,
25
};

        private static int[] Table78 = new int[]//VIP WAREHOUSESTORAGE MULTIPLIER
{
5,
10,
10,
10,
10,
10,
10
};

        private const int DAY = 24 * HOUR;
        private const int HOUR = 60 * MINUTES;
        private const int MINUTES = 60;
    }
}

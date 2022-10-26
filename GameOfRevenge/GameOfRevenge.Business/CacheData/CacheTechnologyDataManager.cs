using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models.Technology;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Business.Manager;
using GameOfRevenge.Common.Interface.Model;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Business.Manager.GameDef;
using GameOfRevenge.Common.Models.Boost;

namespace GameOfRevenge.Business.CacheData
{
    public static class CacheTechnologyDataManager
    {
        public const string TechnologyNotExist = "Technology does not exist";
        public const string TechnologyLevelNotExist = "Technology level data does not exist";

        private static bool isLoaded = false;
        private static List<MainTechnologyType> mainTechTypes;
        private static List<TechnologyType> technologyTypes;
        private static List<TechnologyDataRequirementRel> technologyInfos;
        //        private static List<SubTechnologyDataRequirementRel> subTechInfos;

        public static bool IsLoaded { get => isLoaded && technologyInfos != null; }
        public static IReadOnlyList<MainTechnologyType> MainTechnologyTypes { get { CheckLoadCacheMemory(); return mainTechTypes; } }
        public static IReadOnlyList<TechnologyType> TechnologyTypes { get { CheckLoadCacheMemory(); return technologyTypes; } }
        public static IReadOnlyList<IReadOnlyTechnologyDataRequirementRel> TechnologyInfos { get { CheckLoadCacheMemory(); return technologyInfos.ToList(); } }
        //        public static IReadOnlyList<IReadOnlySubTechnologyDataRequirementRel> SubTechnologyInfos { get { CheckLoadCacheMemory(); return subTechInfos.ToList(); } }


        public static IReadOnlyTechnologyTable GetTechnologyTable(int technologyId)
        {
            var data = TechnologyInfos.FirstOrDefault(x => x.Info.Id == technologyId)?.Info;
            if (data == null) throw new CacheDataNotExistExecption(TechnologyNotExist);
            else return data;
        }
        public static IReadOnlyTechnologyTable GetTechnologyTable(TechnologyType technologyType)
        {
            var data = TechnologyInfos.FirstOrDefault(x => x.Info.Code == technologyType)?.Info;
            if (data == null) throw new CacheDataNotExistExecption(TechnologyNotExist);
            else return data;
        }

        public static IReadOnlyTechnologyDataTable GetTechnologyDataTable(int technologyId, int level)
        {
            var technology = GetFullTechnologyLevelData(technologyId, level);
            var data = technology.Data;
            if (data == null) throw new CacheDataNotExistExecption(TechnologyLevelNotExist);
            else return data;
        }
        public static IReadOnlyTechnologyDataTable GetTechnologyDataTable(TechnologyType type, int level)
        {
            var technology = GetFullTechnologyLevelData(type, level);
            var data = technology.Data;
            if (data == null) throw new CacheDataNotExistExecption(TechnologyLevelNotExist);
            else return data;
        }

        public static IReadOnlyList<IReadOnlyDataRequirement> GetTechnologyDataRequirementsTable(int technologyId, int level)
        {
            var technology = GetFullTechnologyLevelData(technologyId, level);
            var data = technology.Requirements;
            if (data == null) data = new List<DataRequirement>();
            return data;
        }
        public static IReadOnlyList<IReadOnlyDataRequirement> GetTechnologyDataRequirementsTable(TechnologyType technologyType, int level)
        {
            var technology = GetFullTechnologyLevelData(technologyType, level);
            var data = technology.Requirements;
            if (data == null) data = new List<DataRequirement>();
            return data;
        }
        public static IReadOnlyList<IReadOnlyDataRequirement> GetTechnologyDataRequirementsTable(int technologyDataId)
        {
            foreach (var technologyInfo in TechnologyInfos)
            {
                if ((technologyInfo == null) || (technologyInfo.Levels == null) || (technologyInfo.Levels.Count == 0)) continue;

                foreach (var technologyDataInfo in technologyInfo.Levels)
                {
                    if ((technologyDataInfo == null) || (technologyDataInfo.Data == null)) continue;

                    if (technologyDataInfo.Data.DataId == technologyDataId) return technologyDataInfo.Requirements;
                }
            }

            return new List<DataRequirement>();
        }

        /*        public static IReadOnlySubTechnologyDataRequirementRel GetFullSubTechnologyData(int technologyId)
                {
                    var data = SubTechnologyInfos.FirstOrDefault(x => x.Info.Id == technologyId);
                    if (data == null) throw new CacheDataNotExistExecption(TechnologyNotExist);
                    else return data;
                }
                public static IReadOnlySubTechnologyDataRequirementRel GetFullSubTechnologyData(SubTechnologyType technologyType)
                {
                    var data = SubTechnologyInfos.FirstOrDefault(x => x.Info.Code == technologyType);
                    if (data == null) throw new CacheDataNotExistExecption(TechnologyNotExist);
                    else return data;

                }
                public static IReadOnlySubTechnologyDataRequirement GetFullSubTechnologyLevelData(int technologyId, int level)
                {
                    var technology = GetFullSubTechnologyData(technologyId);
                    var data = technology.Levels.FirstOrDefault(x => x.Data.Level == level);
                    if (data == null) throw new CacheDataNotExistExecption(TechnologyLevelNotExist);
                    else return data;
                }
                public static IReadOnlySubTechnologyDataRequirement GetFullSubTechnologyLevelData(SubTechnologyType technologyType, int level)
                {
                    var technology = GetFullSubTechnologyData(technologyType);
                    var data = technology.Levels.FirstOrDefault(x => x.Data.Level == level);
                    if (data == null) throw new CacheDataNotExistExecption(TechnologyLevelNotExist);
                    else return data;
                }*/



        public static IReadOnlyTechnologyDataRequirementRel GetFullTechnologyData(int technologyId)
        {
            var data = TechnologyInfos.FirstOrDefault(x => x.Info.Id == technologyId);
            if (data == null) throw new CacheDataNotExistExecption(TechnologyNotExist);
            else return data;
        }
        public static IReadOnlyTechnologyDataRequirementRel GetFullTechnologyData(TechnologyType technologyType)
        {
            var data = TechnologyInfos.FirstOrDefault(x => x.Info.Code == technologyType);
            if (data == null) throw new CacheDataNotExistExecption(TechnologyNotExist);
            else return data;

        }
        public static IReadOnlyTechnologyDataRequirement GetFullTechnologyLevelData(int technologyId, int level)
        {
            var technology = GetFullTechnologyData(technologyId);
            var data = technology.Levels.FirstOrDefault(x => x.Data.Level == level);
            if (data == null) throw new CacheDataNotExistExecption(TechnologyLevelNotExist);
            else return data;
        }
        public static IReadOnlyTechnologyDataRequirement GetFullTechnologyLevelData(TechnologyType technologyType, int level)
        {
            var technology = GetFullTechnologyData(technologyType);
            var data = technology.Levels.FirstOrDefault(x => x.Data.Level == level);
            if (data == null) throw new CacheDataNotExistExecption(TechnologyLevelNotExist);
            else return data;
        }


        private struct LevelRequirements
        {
            public int Level { get; set; }
            public int TimeTaken { get; set; }
            public List<DataRequirement> Requirements { get; set; }

            public LevelRequirements(int level, int timeTaken, List<DataRequirement> requirements)
            {
                Level = level;
                TimeTaken = timeTaken;
                Requirements = requirements;
            }
        }

        private static TechnologyDataRequirementRel GenerateTechData(TechnologyType type, LevelRequirements[] requirements)
        {
            var data = new TechnologyDataRequirementRel();
            data.Info = new TechnologyTable()
            {
                Id = (int)type,
                Code = type,
                Name = type.ToString()
            };

            data.Levels = new List<TechnologyDataRequirements>();
            foreach (var req in requirements)
            {
                var dataReq = new TechnologyDataRequirements()
                {
                    Data = new TechnologyDataTable()
                    {
                        Id = data.Info.Id,
                        Level = req.Level,
                        TimeTaken = req.TimeTaken
                    },
                    Requirements = req.Requirements
                };
                data.Levels.Add(dataReq);
            }

            return data;
        }

        #region Cache Check, Load and Clear
        public static async Task LoadCacheMemoryAsync()
        {
            ClearCache();

            mainTechTypes = new List<MainTechnologyType>((MainTechnologyType[])System.Enum.GetValues(typeof(MainTechnologyType)));
            mainTechTypes.Remove(MainTechnologyType.Unknown);

            technologyTypes = new List<TechnologyType>((TechnologyType[])System.Enum.GetValues(typeof(TechnologyType)));
            technologyTypes.Remove(TechnologyType.Unknown);
            technologyTypes.Clear();
            technologyTypes.Add(TechnologyType.ConstructionTechnology);

            var constructionTech = GenerateTechData(TechnologyType.ConstructionTechnology, new LevelRequirements[]
                {
                new LevelRequirements(1, 3 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Structure, (int)StructureType.Academy, 1),//resource type  //resource amount
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 1000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 500)
                    }),
                new LevelRequirements(2, 7 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 5000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 2500)
                    }),
                new LevelRequirements(3, 15 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Structure, (int)StructureType.Academy, 2),//resource type  //resource amount
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 10000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 5000)
                    }),
                new LevelRequirements(4, 22 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 50000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 25000)
                    }),
                new LevelRequirements(5, 45 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Structure, (int)StructureType.Academy, 3),//resource type  //resource amount
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 100000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 50000)
                    })
                }
            );

            var upkeepTech = GenerateTechData(TechnologyType.UpkeepTechnology, new LevelRequirements[]
                {
                new LevelRequirements(1, 3 * HOURS + 55 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Structure, (int)StructureType.Academy, 15),//resource type  //resource amount
                        new DataRequirement(DataType.ActiveBoost, (int)NewBoostType.TrainingSpeed, 1),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 300000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 150000)
                    }),
                new LevelRequirements(2, 5 * HOURS + 10 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 400000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 200000)
                    }),
                new LevelRequirements(3, 6 * HOURS + 25 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Structure, (int)StructureType.Academy, 16),//resource type  //resource amount
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 500000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 250000)
                    }),
                new LevelRequirements(4, 7 * HOURS + 45 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 600000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 300000)
                    }),
                new LevelRequirements(5, 9 * HOURS + 15 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 700000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 350000)
                    })
                }
            );

            var fastHealTech = GenerateTechData(TechnologyType.HealSpeedTechnology, new LevelRequirements[]
            {
                new LevelRequirements(1, 2 * HOURS + 05 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Structure, (int)StructureType.Academy, 15),//resource type  //resource amount
                        new DataRequirement(DataType.ActiveBoost, (int)NewBoostType.TrainingSpeed, 1),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 200000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 100000)
                    }),
                new LevelRequirements(2, 3 * HOURS + 55 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 300000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 150000)
                    }),
                new LevelRequirements(3, 5 * HOURS + 10 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Structure, (int)StructureType.Academy, 16),//resource type  //resource amount
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 400000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 200000)
                    }),
                new LevelRequirements(4, 6 * HOURS + 25 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 500000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 250000)
                    }),
                new LevelRequirements(5, 7 * HOURS + 45 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 600000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 300000)
                    })
                }
            );

            var researchTech = GenerateTechData(TechnologyType.ResearchSpeedTechnology, new LevelRequirements[]
            {
                new LevelRequirements(1, 7 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Structure, (int)TechnologyType.ConstructionTechnology, 1),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 10000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 5000)
                    }),
                new LevelRequirements(2, 15 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 50000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 25000)
                    }),
                new LevelRequirements(3, 25 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 100000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 50000)
                    }),
                new LevelRequirements(4, 50 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 200000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 100000)
                    }),
                new LevelRequirements(5, 1 * HOURS + 30 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 300000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 150000)
                    })
                }
            );

            var loadTech = GenerateTechData(TechnologyType.TroopLoadTechnology, new LevelRequirements[]
            {
                new LevelRequirements(1, 15 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Structure, (int)StructureType.Academy, 5),//resource type  //resource amount
                        new DataRequirement(DataType.ActiveBoost, (int)NewBoostType.ResearchSpeed, 1),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 50000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 25000)
                    }),
                new LevelRequirements(2, 25 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 100000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 50000)
                    }),
                new LevelRequirements(3, 50 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 200000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 100000)
                    }),
                new LevelRequirements(4, 1 * HOURS + 30 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Structure, (int)StructureType.Academy, 6),//resource type  //resource amount
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 300000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 150000)
                    }),
                new LevelRequirements(5, 2 * HOURS + 15 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 400000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 200000)
                    })
                }
            );

            var trainTech = GenerateTechData(TechnologyType.TrainSpeedTechnology, new LevelRequirements[]
            {
                new LevelRequirements(1, 2 * HOURS + 15 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Structure, (int)StructureType.Academy, 12),//resource type  //resource amount
                        new DataRequirement(DataType.ActiveBoost, (int)NewBoostType.StaminaRecovery, 1),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 120000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 60000)
                    }),
                new LevelRequirements(2, 3 * HOURS + 05 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 200000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 100000)
                    }),
                new LevelRequirements(3, 5 * HOURS + 55 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 300000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 150000)
                    }),
                new LevelRequirements(4, 6 * HOURS + 25 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Structure, (int)StructureType.Academy, 13),//resource type  //resource amount
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 400000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 200000)
                    }),
                new LevelRequirements(5, 7 * HOURS + 45 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Structure, (int)StructureType.Academy, 14),//resource type  //resource amount
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 500000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 250000)
                    })
                }
            );

            var infirmaryTech = GenerateTechData(TechnologyType.InfirmaryCapacityTechnology, new LevelRequirements[]
            {
                new LevelRequirements(1, 50 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Structure, (int)StructureType.Academy, 7),//resource type  //resource amount
                        new DataRequirement(DataType.ActiveBoost, (int)NewBoostType.TroopLoad, 1),
                        new DataRequirement(DataType.ActiveBoost, (int)NewBoostType.ResourceStorage, 1),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 75000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 40000)
                    }),
                new LevelRequirements(2, 1 * HOURS + 30 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 100000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 50000)
                    }),
                new LevelRequirements(3, 2 * HOURS + 15 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 200000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 100000)
                    }),
                new LevelRequirements(4, 3 * HOURS + 05 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 300000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 150000)
                    }),
                new LevelRequirements(5, 3 * HOURS + 55 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Structure, (int)StructureType.Academy, 8),//resource type  //resource amount
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 400000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 200000)
                    })
                }
            );

            var staimnaTech = GenerateTechData(TechnologyType.KingStaminaTechnology, new LevelRequirements[]
            {
                new LevelRequirements(1, 1 * HOURS + 30 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Structure, (int)StructureType.Academy, 9),//resource type  //resource amount
                        new DataRequirement(DataType.ActiveBoost, (int)NewBoostType.InfirmaryCapacity, 1),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 75000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 40000)
                    }),
                new LevelRequirements(2, 2 * HOURS + 15 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 100000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 50000)
                    }),
                new LevelRequirements(3, 3 * HOURS + 05 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 200000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 100000)
                    }),
                new LevelRequirements(4, 3 * HOURS + 55 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Structure, (int)StructureType.Academy, 10),//resource type  //resource amount
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 300000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 150000)
                    }),
                new LevelRequirements(5, 5 * HOURS + 10 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Structure, (int)StructureType.Academy, 11),//resource type  //resource amount
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 400000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 200000)
                    })
                }
            );

            var storageTech = GenerateTechData(TechnologyType.StorageTechnology, new LevelRequirements[]
            {
                new LevelRequirements(1, 25 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Structure, (int)StructureType.Academy, 5),//resource type  //resource amount
                        new DataRequirement(DataType.ActiveBoost, (int)NewBoostType.ResearchSpeed, 1),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 50000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 25000)
                    }),
                new LevelRequirements(2, 50 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 100000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 50000)
                    }),
                new LevelRequirements(3, 1 * HOURS + 30 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 200000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 100000)
                    }),
                new LevelRequirements(4, 2 * HOURS + 15 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Structure, (int)StructureType.Academy, 6),//resource type  //resource amount
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 300000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 150000)
                    }),
                new LevelRequirements(5, 3 * HOURS + 05 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 400000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 200000)
                    })
                }
            );


            var infantryatkTech = GenerateTechData(TechnologyType.BarracksAttackTechnology, new LevelRequirements[]
            {
                new LevelRequirements(1, 15 * MINUTES,
                    new List<DataRequirement>()
                    {
//                        new DataRequirement(DataType.ActiveBoost, (int)NewBoostType.MARCHSPEED, 1),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 2500),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 1500)
                    }),
                new LevelRequirements(2, 22 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 4000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 2000)
                    }),
                new LevelRequirements(3, 45 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 6000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 2200)
                    })
                }
            );


            var cavalryatkTech = GenerateTechData(TechnologyType.StableAttackTechnology, new LevelRequirements[]
            {
                new LevelRequirements(1, 22 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Structure, (int)StructureType.Academy, 6),//resource type  //resource amount
//                        new DataRequirement(DataType.ActiveBoost, (int)NewBoostType.MONSTERMARCH, 1),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 4500),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 2200)
                    }),
                new LevelRequirements(2, 45 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 5000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 2500)
                    }),
                new LevelRequirements(3, 1 * HOURS + 10 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 5500),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 3000)
                    })
                }
            );


            var siegeatkTech = GenerateTechData(TechnologyType.WorkshopAttackTechnology, new LevelRequirements[]
            {
                new LevelRequirements(1, 22 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Structure, (int)StructureType.Academy, 6),//resource type  //resource amount
//                        new DataRequirement(DataType.ActiveBoost, (int)NewBoostType.MONSTERMARCH, 1),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 4500),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 2200)
                    }),
                new LevelRequirements(2, 45 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 5000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 2500)
                    }),
                new LevelRequirements(3, 1 * HOURS + 10 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 5500),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 3000)
                    })
                }
            );

            var bowmenatkTech = GenerateTechData(TechnologyType.ShootingRangeAttackTechnology, new LevelRequirements[]
            {
                new LevelRequirements(1, 22 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Structure, (int)StructureType.Academy, 6),//resource type  //resource amount
//                        new DataRequirement(DataType.ActiveBoost, (int)NewBoostType.MONSTERMARCH, 1),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 4500),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 2200)
                    }),
                new LevelRequirements(2, 45 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 5000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 2500)
                    }),
                new LevelRequirements(3, 1 * HOURS + 10 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 5500),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 3000)
                    })
                }
            );



            var infantrydefTech = GenerateTechData(TechnologyType.BarracksDefenseTechnology, new LevelRequirements[]
            {
                new LevelRequirements(1, 15 * MINUTES,
                    new List<DataRequirement>()
                    {
//                        new DataRequirement(DataType.ActiveBoost, (int)NewBoostType.MARCHSPEED, 1),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 2500),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 1500)
                    }),
                new LevelRequirements(2, 22 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 4000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 2000)
                    }),
                new LevelRequirements(3, 45 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 6000),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 2200)
                    })
                }
            );

            var cavalrydefTech = GenerateTechData(TechnologyType.StableDefenseTechnology, new LevelRequirements[]
            {
                new LevelRequirements(1, 45 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Structure, (int)StructureType.Academy, 7),//resource type  //resource amount
                        new DataRequirement(DataType.ActiveBoost, (int)NewBoostType.CavalryAttack, 1),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 5500),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 3200)
                    }),
                new LevelRequirements(2, 1 * HOURS + 10 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Structure, (int)StructureType.Academy, 8),//resource type  //resource amount
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 5500),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 3000)
                    }),
                new LevelRequirements(3, 1 * HOURS + 55 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Structure, (int)StructureType.Academy, 9),//resource type  //resource amount
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 5500),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 3000)
                    })
                }
            );


            var siegedefTech = GenerateTechData(TechnologyType.WorkshopDefenseTechnology, new LevelRequirements[]
            {
                new LevelRequirements(1, 45 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Structure, (int)StructureType.Academy, 7),//resource type  //resource amount
                        new DataRequirement(DataType.ActiveBoost, (int)NewBoostType.SiegeAttack, 1),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 5500),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 3200)
                    }),
                new LevelRequirements(2, 1 * HOURS + 10 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Structure, (int)StructureType.Academy, 8),//resource type  //resource amount
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 5500),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 3000)
                    }),
                new LevelRequirements(3, 1 * HOURS + 55 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Structure, (int)StructureType.Academy, 9),//resource type  //resource amount
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 5500),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 3000)
                    })
                }
            );

            var bowmendefTech = GenerateTechData(TechnologyType.ShootingRangeDefenseTechnology, new LevelRequirements[]
            {
                new LevelRequirements(1, 45 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Structure, (int)StructureType.Academy, 7),//resource type  //resource amount
                        new DataRequirement(DataType.ActiveBoost, (int)NewBoostType.BowmenAttack, 1),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 5500),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 3200)
                    }),
                new LevelRequirements(2, 1 * HOURS + 10 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Structure, (int)StructureType.Academy, 8),//resource type  //resource amount
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 5500),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 3000)
                    }),
                new LevelRequirements(3, 1 * HOURS + 55 * MINUTES,
                    new List<DataRequirement>()
                    {
                        new DataRequirement(DataType.Structure, (int)StructureType.Academy, 9),//resource type  //resource amount
                        new DataRequirement(DataType.Resource, (int)ResourceType.Food, 5500),
                        new DataRequirement(DataType.Resource, (int)ResourceType.Wood, 3000)
                    })
                }
            );




            technologyInfos = new List<TechnologyDataRequirementRel>()
            {
                constructionTech, upkeepTech, fastHealTech, researchTech, loadTech, trainTech, infirmaryTech, staimnaTech, storageTech,
                infantryatkTech, cavalryatkTech, siegeatkTech, bowmenatkTech,
                infantrydefTech, cavalrydefTech, siegedefTech, bowmendefTech
            };

            isLoaded = true;
/*

            var resManager = new TechnologyManager();
            var response = await resManager.GetAllTechnologyDataRequirementRel();

            if (response.IsSuccess && response.HasData)
            {
                technologyTypes = new List<TechnologyType>();
                technologyInfos = response.Data;
                foreach (var technology in response.Data)
                {
                    if (technologyTypes.Contains(technology.Info.Code)) continue;
                    if (technology.Info.Code == TechnologyType.Unknown) continue;
                    technologyTypes.Add(technology.Info.Code);
                }

//                subTechTypes = new List<SubTechnologyType>((IEnumerable<SubTechnologyType>)System.Enum.GetValues(typeof(SubTechnologyType)));
                mainTechTypes = new List<SubTechnologyType>()
                {
                    SubTechnologyType.FoodProduction1, SubTechnologyType.WoodProduction1
                };

                subTechInfos = new List<SubTechnologyDataRequirementRel>();
                var (info, recordId) = GenerateSubTechnology(SubTechnologyType.FoodProduction1, "Food Production 1");
                subTechInfos.Add(info);

                (info, recordId) = GenerateSubTechnology(SubTechnologyType.FoodProduction1, "Wood Production 1", recordId);
                subTechInfos.Add(info);

                (info, recordId) = GenerateSubTechnology(SubTechnologyType.IronProduction1, "Iron Production 1", recordId);
                subTechInfos.Add(info);

                (info, recordId) = GenerateSubTechnology(SubTechnologyType.WallDefense1, "Wall Defense 1", recordId);
                subTechInfos.Add(info);

                (info, recordId) = GenerateSubTechnology(SubTechnologyType.DefenderAttack1, "Defender Attack 1", recordId);
                subTechInfos.Add(info);

                (info, recordId) = GenerateSubTechnology(SubTechnologyType.DefenderDefense1, "Defender Defense 1", recordId);
                subTechInfos.Add(info);

                (info, recordId) = GenerateSubTechnology(SubTechnologyType.DefenderHealth1, "Defender Health 1", recordId);
                subTechInfos.Add(info);

                (info, recordId) = GenerateSubTechnology(SubTechnologyType.MarchSpeed1, "March Speed 1", recordId);
                subTechInfos.Add(info);

                (info, recordId) = GenerateSubTechnology(SubTechnologyType.MarchCapacity1, "March Capacity 1", recordId);
                subTechInfos.Add(info);

                (info, recordId) = GenerateSubTechnology(SubTechnologyType.InfantryHealth1, "Infantry Health 1", recordId);
                subTechInfos.Add(info);

                (info, recordId) = GenerateSubTechnology(SubTechnologyType.InfantryAttack1, "Infantry Attack 1", recordId);
                subTechInfos.Add(info);

                (info, recordId) = GenerateSubTechnology(SubTechnologyType.TrainSpeed1, "Train Speed 1", recordId);
                subTechInfos.Add(info);

                (info, recordId) = GenerateSubTechnology(SubTechnologyType.FastHeal1, "Fast Heal 1", recordId);
                subTechInfos.Add(info);

                (info, recordId) = GenerateSubTechnology(SubTechnologyType.HospitalCapacity1, "Hospital Capacity 1", recordId);
                subTechInfos.Add(info);

                isLoaded = true;
            }
            else
            {
                ClearCache();
                throw new CacheDataNotExistExecption(response.Message);
            }*/
        }
        public static void LoadCacheMemory()
        {
            var tsk = LoadCacheMemoryAsync();
            tsk.Wait();
        }
        public static void CheckLoadCacheMemory()
        {
            if (isLoaded) return;
            else LoadCacheMemory();
        }
        public static async Task CheckLoadCacheMemoryAsync()
        {
            if (isLoaded) return;
            else await LoadCacheMemoryAsync();
        }
        public static void ClearCache()
        {
            isLoaded = false;

            if (technologyInfos != null)
            {
                technologyInfos.Clear();
                technologyInfos = null;
            }

            if (technologyTypes != null)
            {
                technologyTypes.Clear();
                technologyTypes = null;
            }

            if (mainTechTypes != null)
            {
                mainTechTypes.Clear();
                mainTechTypes = null;
            }
        }

        private const int DAYS = 24 * HOURS;
        private const int HOURS = 60 * MINUTES;
        private const int MINUTES = 60;
        #endregion

/*        static (SubTechnologyDataRequirementRel, int) GenerateSubTechnology(SubTechnologyType type, string name, int recordId = 1)
        {
            int seconds = 60;

            SubTechnologyDataRequirementRel info = new SubTechnologyDataRequirementRel();
            info.Info = new SubTechnologyTable()
            {
                Code = type,
                Id = (int)type,
                Name = name
            };
            info.Levels = new List<SubTechnologyDataRequirements>()
            {
                new SubTechnologyDataRequirements()
                {
                    Data = new SubTechnologyDataTable()
                    {
                        DataId = recordId,

                        Id = info.Info.Id,

                        Value = 1,
                        Level = 1,
                        TimeTaken = 3 * seconds
                    },
                    Requirements = new List<DataRequirement>()
                },
                new SubTechnologyDataRequirements()
                {
                    Data = new SubTechnologyDataTable()
                    {
                        DataId = recordId + 1,

                        Id = info.Info.Id,

                        Value = 2,
                        Level = 2,
                        TimeTaken = 7 * seconds
                    },
                    Requirements = new List<DataRequirement>()
                },
                new SubTechnologyDataRequirements()
                {
                    Data = new SubTechnologyDataTable()
                    {
                        DataId = recordId + 2,

                        Id = info.Info.Id,

                        Value = 3,
                        Level = 3,
                        TimeTaken = 15 * seconds
                    },
                    Requirements = new List<DataRequirement>()
                },
                new SubTechnologyDataRequirements()
                {
                    Data = new SubTechnologyDataTable()
                    {
                        DataId = recordId + 3,

                        Id = info.Info.Id,

                        Value = 4,
                        Level = 4,
                        TimeTaken = 22 * seconds
                    },
                    Requirements = new List<DataRequirement>()
                },
                new SubTechnologyDataRequirements()
                {
                    Data = new SubTechnologyDataTable()
                    {
                        DataId = recordId + 4,

                        Id = info.Info.Id,

                        Value = 5,
                        Level = 5,
                        TimeTaken = 45 * seconds
                    },
                    Requirements = new List<DataRequirement>()
                }
            };
            recordId += 5;

            return (info, recordId);
        }*/
    }
}

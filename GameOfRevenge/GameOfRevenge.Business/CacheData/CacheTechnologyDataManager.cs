using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models.Technology;
using GameOfRevenge.Business.Manager;
using GameOfRevenge.Common.Interface.Model;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Business.Manager.GameDef;

namespace GameOfRevenge.Business.CacheData
{
    public static class CacheTechnologyDataManager
    {
        public const string TechnologyNotExist = "Technology does not exist";
        public const string TechnologyLevelNotExist = "Technology level data does not exist";

        private static bool isLoaded = false;
        private static List<TechnologyDataRequirementRel> technologyInfos;
        private static List<TechnologyType> technologyTypes;
        private static List<SubTechnologyType> subTechTypes;
        private static List<SubTechnologyDataRequirementRel> subTechInfos;

        public static bool IsLoaded { get => isLoaded && technologyInfos != null; }
        public static IReadOnlyList<IReadOnlyTechnologyDataRequirementRel> TechnologyInfos { get { CheckLoadCacheMemory(); return technologyInfos.ToList(); } }
        public static IReadOnlyList<TechnologyType> TechnologyTypes { get { CheckLoadCacheMemory(); return technologyTypes; } }
        public static IReadOnlyList<SubTechnologyType> SubTechnologyTypes { get { CheckLoadCacheMemory(); return subTechTypes; } }
        public static IReadOnlyList<IReadOnlySubTechnologyDataRequirementRel> SubTechnologyInfos { get { CheckLoadCacheMemory(); return subTechInfos.ToList(); } }


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
                if (technologyInfo == null || technologyInfo.Levels == null || technologyInfo.Levels.Count <= 0) continue;
                foreach (var technologyDataInfo in technologyInfo.Levels)
                {
                    if (technologyDataInfo == null || technologyDataInfo.Data == null) continue;
                    if (technologyDataInfo.Data.DataId == technologyDataId) return technologyDataInfo.Requirements;
                }
            }

            return new List<DataRequirement>();
        }

        public static IReadOnlySubTechnologyDataRequirementRel GetFullSubTechnologyData(int technologyId)
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
        }



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



        #region Cache Check, Load and Clear
        public static async Task LoadCacheMemoryAsync()
        {
            ClearCache();

            var resManager = new TechnologyManager();
            var response = await resManager.GetAllTechnologyDataRequirementRel();

            if (response.IsSuccess && response.HasData)
            {
                technologyTypes = new List<TechnologyType>();
                technologyInfos = response.Data;
                foreach (var technology in response.Data)
                {
                    if (technologyTypes.Contains(technology.Info.Code)) continue;
                    if (technology.Info.Code == TechnologyType.Other) continue;
                    technologyTypes.Add(technology.Info.Code);
                }

//                subTechTypes = new List<SubTechnologyType>((IEnumerable<SubTechnologyType>)System.Enum.GetValues(typeof(SubTechnologyType)));
                subTechTypes = new List<SubTechnologyType>()
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
            }
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
        }
        #endregion

        static (SubTechnologyDataRequirementRel, int) GenerateSubTechnology(SubTechnologyType type, string name, int recordId = 1)
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
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExitGames.Logging;
using GameOfRevenge.Business.Manager;
using GameOfRevenge.Business.Manager.GameDef;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Interface.Model;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Business.CacheData
{
    public static class CacheStructureDataManager
    {
        public const string StructureNotExist = "Structure does not exist";
        public const string StructureLevelNotExist = "Structure level data does not exist";

        private static bool isLoaded = false;
        private static List<StructureDataRequirementRel> structureInfos;
        private static List<BuildingInfoData> structureInfof;
        private static List<StructureType> structureTypes;

        public static bool IsLoaded { get => isLoaded && structureInfos != null; }
        public static List<BuildingInfoData> StructureInfoFactory
        {
            get
            {
                CheckLoadCacheMemory();
                return LoadInfoOnly();
            }
        }
        public static IReadOnlyList<IReadOnlyStructureDataRequirementRel> StructureInfos
        {
            get
            {
                CheckLoadCacheMemory();
                return structureInfos;//.ToList();
            }
        }
        public static IReadOnlyList<StructureType> StructureTypes
        {
            get
            {
                CheckLoadCacheMemory();
                return structureTypes;
            }
        }


        public static IReadOnlyStructureTable GetStructureTable(int structureId)
        {
            var data = StructureInfos.FirstOrDefault(x => x.Info.Id == structureId)?.Info;
            if (data == null) throw new CacheDataNotExistExecption(StructureNotExist);
            else return data;
        }
        public static IReadOnlyStructureTable GetStructureTable(StructureType structureType)
        {
            var data = StructureInfos.FirstOrDefault(x => x.Info.Code == structureType)?.Info;
            if (data == null) throw new CacheDataNotExistExecption(StructureNotExist);
            else return data;
        }

        public static IReadOnlyStructureDataTable GetStructureDataTable(int structureId, int level)
        {
            var structure = GetFullStructureLevelData(structureId, level);
            var data = structure.Data;
            if (data == null) throw new CacheDataNotExistExecption(StructureLevelNotExist);
            else return data;
        }
        public static IReadOnlyStructureDataTable GetStructureDataTable(StructureType type, int level)
        {
            var structure = GetFullStructureLevelData(type, level);
            var data = structure.Data;
            if (data == null) throw new CacheDataNotExistExecption(StructureLevelNotExist);
            else return data;
        }

        public static IReadOnlyList<IReadOnlyDataRequirement> GetStructureDataRequirementsTable(int structureId, int level)
        {
            var structure = GetFullStructureLevelData(structureId, level);
            var data = structure.Requirements;
            if (data == null) data = new List<DataRequirement>();
            return data;
        }
        public static IReadOnlyList<IReadOnlyDataRequirement> GetStructureDataRequirementsTable(StructureType structureType, int level)
        {
            var structure = GetFullStructureLevelData(structureType, level);
            var data = structure.Requirements;
            if (data == null) data = new List<DataRequirement>();
            return data;
        }
        public static IReadOnlyList<IReadOnlyDataRequirement> GetStructureDataRequirementsTable(int structureDataId)
        {
            foreach (var structureInfo in StructureInfos)
            {
                if (structureInfo == null || structureInfo.Levels == null || structureInfo.Levels.Count <= 0) continue;
                foreach (var structureDataInfo in structureInfo.Levels)
                {
                    if (structureDataInfo == null || structureDataInfo.Data == null) continue;
                    if (structureDataInfo.Data.DataId == structureDataId) return structureDataInfo.Requirements;
                }
            }

            return new List<DataRequirement>();
        }

        public static IReadOnlyStructureDataRequirementRel GetFullStructureData(int structureId)
        {
            var data = StructureInfos.FirstOrDefault(x => x.Info.Id == structureId);
            if (data == null) throw new CacheDataNotExistExecption(StructureNotExist);
            else return data;
        }
        public static IReadOnlyStructureDataRequirementRel GetFullStructureData(StructureType structureType)
        {
            var data = StructureInfos.FirstOrDefault(x => x.Info.Code == structureType);
            if (data == null) throw new CacheDataNotExistExecption(StructureNotExist);
            else return data;
        }
        public static IReadOnlyStructureDataRequirement GetFullStructureLevelData(int structureId, int level)
        {
            var structure = GetFullStructureData(structureId);
            var data = structure.Levels.FirstOrDefault(x => x.Data.Level == level);
            if (data == null) throw new CacheDataNotExistExecption(StructureLevelNotExist);
            else return data;
        }
        public static IReadOnlyStructureDataRequirement GetFullStructureLevelData(StructureType structureType, int level)
        {
            var structure = GetFullStructureData(structureType);
            var data = structure.Levels.FirstOrDefault(x => x.Data.Level == level);
            if (data == null) throw new CacheDataNotExistExecption(StructureLevelNotExist);
            else return data;
        }

        public static (int, int) GetBoostResourceGenerationTime(int location, int castleLevel)
        {
            var seconds = 5 * 60;
            var percentage = 10;
            if (location >= 50)
            {
                seconds = 8 * 3600;
                percentage = castleLevel;
            }

            return (seconds, percentage);
        }

        #region Cache Check, Load and Clear
        public static async Task<Response> StoreData(int type, string data)
        {
            var structureManager = new StructureManager();
            return await structureManager.StoreData(type, data);
        }

        public static async Task LoadCacheMemoryAsync()
        {
            ClearCache();

            var structureManager = new StructureManager();
            var response = await structureManager.GetAllStructDataRequirementRel();
            if (response.IsSuccess && response.HasData)
            {

                structureInfos = response.Data;
                structureTypes = new List<StructureType>();
                foreach (var structure in response.Data)
                {
                    if ((structure.Info != null) &&
                        !structureTypes.Contains(structure.Info.Code) &&
                        (structure.Info.Code != StructureType.Unknown))
                    {
                        structureTypes.Add(structure.Info.Code);
                    }
                }
/*                foreach (var structures in structureInfos)
                {
                    foreach (var structure in structures.Levels)
                    {
                        //TODO: put these overrided values on database
                        if ((structure.Data.Level == 1) &&
                            ((structures.Info.Code == StructureType.CityCounsel) ||
                            (structures.Info.Code == StructureType.Gate) ||
                            (structures.Info.Code == StructureType.Warehouse)))
                        {
                            structure.Data.TimeToBuild = 0;
                        }
                        //#if DEBUG
                        //                        else structure.Data.TimeToBuild = 60;
                        //#endif
                    }
                }*/

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

            LoadCacheMemory();
        }
        public static async Task CheckLoadCacheMemoryAsync()
        {
            if (isLoaded) return;

            await LoadCacheMemoryAsync();
        }
        public static void ClearCache()
        {
            isLoaded = false;

            if (structureInfos != null)
            {
                structureInfos.Clear();
                structureInfos = null;
            }

            if (structureTypes != null)
            {
                structureTypes.Clear();
                structureTypes = null;
            }
            if (structureInfof != null)
            {
                structureInfof.Clear();
                structureInfof = null;
            }
        }
        #endregion


        private static List<BuildingInfoData> LoadInfoOnly()
        {
            if ((structureInfof == null) || (structureInfof.Count < 1))
            {
                structureInfof = new List<BuildingInfoData>();

                foreach (var structure in StructureInfos)
                {
                    List<string> columns = LoadDescriptionInfoHeaders(structure.Info.Code);
                    if ((columns == null) || (columns.Count < 1)) continue;

                    var table = new InfoDataTable()
                    {
                        Columns = columns,
                        Rows = LoadDescriptionInfoRows(structure.Info.Code, structure.Levels)
                    };

                    var structureInfo = new BuildingInfoData()
                    {
                        StructureId = structure.Info.Id,
                        Code = structure.Info.Code.ToString(),
                        StructureType = structure.Info.Code,
                        Name = structure.Info.Name,
                        Table = table,
                        Description = structure.Info.Description,
                    };

                    structureInfof.Add(structureInfo);
                }
            }


            return structureInfof;
        }

        private static List<string> LoadDescriptionInfoHeaders(StructureType type)
        {
            switch (type)
            {
                case StructureType.CityCounsel: return new List<string>() { "Level", "Power" };
                case StructureType.Gate: return new List<string>() { "Level", "Traps", "Defense" };
                case StructureType.WatchTower: return new List<string>() { "Level", "Spying" };
                case StructureType.Blacksmith: return new List<string>() { "Level" };
                case StructureType.Embassy: return new List<string>() { "Level" };
                case StructureType.Warehouse: return new List<string>() { "Level", "Food", "Wood", "Ore" };
                case StructureType.Academy: return new List<string>() { "Level", "Technology", "Boost" };

                case StructureType.Farm: return new List<string>() { "Level", "Hourly Yeild", "Capacity", "Total Collect" };
                case StructureType.Sawmill: return new List<string>() { "Level", "Hourly Yeild", "Capacity", "Total Collect" };
                case StructureType.Mine: return new List<string>() { "Level", "Hourly Yeild", "Capacity", "Total Collect" };

                case StructureType.InfantryCamp: return new List<string>() { "Level", "Population", "Training Speed" };
                case StructureType.Infirmary: return new List<string>() { "Level", "Wounded Capacity" };

                case StructureType.Barracks:
                case StructureType.ShootingRange:
                case StructureType.Stable:
                case StructureType.Workshop: return new List<string>() { "Level", "Troop", "Health", "Attack" };

                case StructureType.Market: return new List<string>() { "Level", "Max Caravan Load" };

                case StructureType.FriendshipHall:
                case StructureType.TrainingHeroes: return new List<string>() { "Level" };

                default: return null;
            }
        }

        private static List<List<string>> LoadDescriptionInfoRows(StructureType type, IReadOnlyList<IReadOnlyStructureDataRequirement> levels)
        {
            var lst = new List<List<string>>();

            foreach (var level in levels)
            {
                //TimeSpan t = TimeSpan.FromSeconds(level.Data.TimeToBuild);
                //string timeInString = string.Format("{0:D2}h:{1:D2}m:{2:D2}s", t.Hours, t.Minutes, t.Seconds);

                List<string> item = new List<string>
                {
                    level.Data.Level.ToString()
                };

                switch (type)
                {
                    case StructureType.CityCounsel:
                        item.Add((1000 + level.Data.Level * 100).ToString());
                        break;
                    case StructureType.Gate:
                        item.Add(level.Data.Level.ToString());
                        item.Add(level.Data.HitPoint.ToString());
                        break;
                    case StructureType.WatchTower:
                        if (level.Data.Level == 1) item.Add("Reveals incoming troops from Player name.");
                        else if (level.Data.Level == 3) item.Add("Reveals the precise location of the incoming troops' origin.");
                        else if (level.Data.Level == 7) item.Add("Reveals the estimated time of arrival for incoming troops.");
                        else if (level.Data.Level == 11) item.Add("Reveals the total size of the incoming troops.");
                        else if (level.Data.Level == 17) item.Add("Reveals the exact king level of the incoming troops.");
                        else if (level.Data.Level == 19) item.Add("Reveals the types of soldiers in the incoming troops.");
                        else if (level.Data.Level == 23) item.Add("Reveals the number of each soldier type in the incoming troops.");
                        else if (level.Data.Level == 25) item.Add("Reveals the amount of heroes in the dispatch.");
                        else if (level.Data.Level == 30) item.Add("Reveals the types of heroes in the dispatch.");
                        else item.Add(string.Empty);
                        break;
                    case StructureType.Blacksmith:
                        break;
                    case StructureType.Embassy:
                        break;
                    case StructureType.Warehouse:
                        item.Add(level.Data.SafeDeposit.ToString());
                        item.Add(level.Data.SafeDeposit.ToString());
                        item.Add(level.Data.SafeDeposit.ToString());
                        break;
                    case StructureType.Academy:
                        item.AddRange(TechnologyRequirementDecriptionInfo(level));
                        break;
                    case StructureType.Farm:
                        item.Add((level.Data.FoodProduction * 3600).ToString());
                        item.Add(level.Data.ResourceCapacity.ToString());
                        item.Add(level.Data.ResourceCapacity.ToString());
                        break;
                    case StructureType.Sawmill:
                        item.Add((level.Data.WoodProduction * 3600).ToString());
                        item.Add(level.Data.ResourceCapacity.ToString());
                        item.Add(level.Data.ResourceCapacity.ToString());
                        break;
                    case StructureType.Mine:
                        item.Add((level.Data.OreProduction * 3600).ToString());
                        item.Add(level.Data.ResourceCapacity.ToString());
                        item.Add(level.Data.ResourceCapacity.ToString());
                        break;
                    case StructureType.InfantryCamp:
                        item.Add(level.Data.PopulationSupport.ToString());
                        item.Add(level.Data.Level.ToString() + "%");
                        break;
                    case StructureType.Barracks:
                    case StructureType.ShootingRange:
                    case StructureType.Stable:
                    case StructureType.Workshop:
                        item.AddRange(TroopAddRequirementDecriptionInfo(type, level));
                        break;
                    case StructureType.Infirmary:
                        item.Add(level.Data.WoundedCapacity.ToString());
                        break;
                    case StructureType.Market:
                        if (level.Data.Level <= 20) item.Add((level.Data.Level * 20000).ToString());
                        else if (level.Data.Level <= 25) item.Add((400000 + ((level.Data.Level-20) * 40000)).ToString());
                        else if (level.Data.Level == 26) item.Add("660000");
                        else if (level.Data.Level == 27) item.Add("720000");
                        else if (level.Data.Level == 28) item.Add("800000");
                        else if (level.Data.Level == 29) item.Add("900000");
                        else if (level.Data.Level == 30) item.Add("1000000");
                        break;
                }

                //item.Add(timeInString);
                lst.Add(item);
            }

            return lst;
        }

        private static string[] TechnologyRequirementDecriptionInfo(IReadOnlyStructureDataRequirement level)
        {
            var structureId = GetFullStructureData(StructureType.Academy).Info.Id;
            var techs = CacheTechnologyDataManager.TechnologyInfos;
            string[] strs = new string[2];

            foreach (var tech in techs)
            {
                foreach (var techdata in tech.Levels)
                {
                    foreach (var req in techdata.Requirements)
                    {
                        if (req.DataType == DataType.Structure && req.ValueId == structureId && req.Value == level.Data.Level)
                        {
                            if (!string.IsNullOrWhiteSpace(strs[0])) strs[0] += Environment.NewLine;
                            if (!string.IsNullOrWhiteSpace(strs[1])) strs[1] += Environment.NewLine;

                            strs[0] += tech.Info.Name + " " + techdata.Data.Level;
                            strs[1] += techdata.Data.Level + "%";
                        }
                    }
                }
            }

            return strs;
        }

        private static string[] TroopAddRequirementDecriptionInfo(StructureType type, IReadOnlyStructureDataRequirement level)
        {
            TroopType troopType = TroopType.Other;
            var troopBuildingRelation = CacheTroopDataManager.TroopBuildingRelation.FirstOrDefault(x => x.Structure == type);
            if (troopBuildingRelation != null) troopType = troopBuildingRelation.Troops.FirstOrDefault();
            var troop = CacheTroopDataManager.GetFullTroopData(troopType);

            string[] strs = new string[3];
            if (troop != null)
            {
                var structureId = GetFullStructureData(troopBuildingRelation.Structure).Info.Id;
                foreach (var data in troop.Levels)
                {
                    foreach (var req in data.Requirements)
                    {
                        if ((req.DataType == DataType.Structure) && (req.ValueId == structureId) && (req.Value == level.Data.Level))
                        {
                            strs[0] += troop.Info.Name + " Lvl." + data.Data.Level;
                            strs[1] += data.Data.Health;// Power;
                            strs[2] += data.Data.AttackDamage;

                            return strs;
                        }
                    }
                }
            }

            return strs;
        }
    }
}

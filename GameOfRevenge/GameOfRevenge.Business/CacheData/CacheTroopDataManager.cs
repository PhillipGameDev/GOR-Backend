using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models.Troop;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Business.Manager.GameDef;

namespace GameOfRevenge.Business.CacheData
{
    public static class CacheTroopDataManager
    {
        public const string TroopNotExist = "Troop does not exist";
        public const string TroopLevelNotExist = "Troop level data does not exist";

        private static bool isLoaded = false;
        private static List<TroopDataRequirementRel> troopInfos;
        private static List<TroopBuildingRelation> troopBuildingRel;
        private static List<TroopType> troopTypes;

        public static bool IsLoaded { get => isLoaded && troopInfos != null; }
        public static IReadOnlyList<IReadOnlyTroopDataRequirementRel> TroopInfos { get { CheckLoadCacheMemory(); return troopInfos; } }
        public static IReadOnlyList<TroopType> TroopTypes { get { CheckLoadCacheMemory(); return troopTypes; } }
        public static IReadOnlyList<IReadOnlyTroopBuildingRelation> TroopBuildingRelation { get { CheckLoadCacheMemory(); return troopBuildingRel; } }


        public static IReadOnlyTroopTable GetTroopTable(int troopId)
        {
            var data = TroopInfos.FirstOrDefault(x => x.Info.Id == troopId)?.Info;
            if (data == null) throw new CacheDataNotExistExecption(TroopNotExist);
            else return data;
        }
        public static IReadOnlyTroopTable GetTroopTable(TroopType troopType)
        {
            var data = TroopInfos.FirstOrDefault(x => x.Info.Code == troopType)?.Info;
            if (data == null) throw new CacheDataNotExistExecption(TroopNotExist);
            else return data;
        }

        public static IReadOnlyTroopDataTable GetTroopDataTable(int troopId, int level)
        {
            var Troop = GetFullTroopLevelData(troopId, level);
            var data = Troop.Data;
            if (data == null) throw new CacheDataNotExistExecption(TroopLevelNotExist);
            else return data;
        }
        public static IReadOnlyTroopDataTable GetTroopDataTable(TroopType type, int level)
        {
            var Troop = GetFullTroopLevelData(type, level);
            var data = Troop.Data;
            if (data == null) throw new CacheDataNotExistExecption(TroopLevelNotExist);
            else return data;
        }

        public static IReadOnlyList<IReadOnlyDataRequirement> GetTroopDataRequirementsTable(int troopId, int level)
        {
            var Troop = GetFullTroopLevelData(troopId, level);
            var data = Troop.Requirements;
            if (data == null) data = new List<DataRequirement>();
            return data;
        }
        public static IReadOnlyList<IReadOnlyDataRequirement> GetTroopDataRequirementsTable(TroopType troopType, int level)
        {
            var Troop = GetFullTroopLevelData(troopType, level);
            var data = Troop.Requirements;
            if (data == null) data = new List<DataRequirement>();
            return data;
        }
        public static IReadOnlyList<IReadOnlyDataRequirement> GetTroopDataRequirementsTable(int troopDataId)
        {
            foreach (var TroopInfo in TroopInfos)
            {
                if (TroopInfo == null || TroopInfo.Levels == null || TroopInfo.Levels.Count <= 0) continue;
                foreach (var TroopDataInfo in TroopInfo.Levels)
                {
                    if (TroopDataInfo == null || TroopDataInfo.Data == null) continue;
                    if (TroopDataInfo.Data.DataId == troopDataId) return TroopDataInfo.Requirements;
                }
            }

            return new List<DataRequirement>();
        }

        public static IReadOnlyTroopDataRequirementRel GetFullTroopData(int troopId)
        {
            var data = TroopInfos.FirstOrDefault(x => x.Info.Id == troopId);
            if (data == null) throw new CacheDataNotExistExecption(TroopNotExist);
            else return data;
        }
        public static IReadOnlyTroopDataRequirementRel GetFullTroopData(TroopType troopType)
        {
            var data = TroopInfos.FirstOrDefault(x => x.Info.Code == troopType);
            if (data == null) throw new CacheDataNotExistExecption(TroopNotExist);
            else return data;

        }
        public static IReadOnlyTroopDataRequirements GetFullTroopLevelData(int troopId, int level)
        {
            var Troop = GetFullTroopData(troopId);
            var data = Troop.Levels.FirstOrDefault(x => x.Data.Level == level);
            if (data == null) throw new CacheDataNotExistExecption(TroopLevelNotExist);
            else return data;
        }
        public static IReadOnlyTroopDataRequirements GetFullTroopLevelData(TroopType troopType, int level)
        {
            var Troop = GetFullTroopData(troopType);
            var data = Troop.Levels.FirstOrDefault(x => x.Data.Level == level);
            if (data == null) throw new CacheDataNotExistExecption(TroopLevelNotExist);
            else return data;
        }

        #region Cache Check, Load and Clear
        public static async Task LoadCacheMemoryAsync()
        {
            ClearCache();

            var resManager = new TroopManager();
            var response = await resManager.GetAllTroopDataRequirementRel();

            if (response.IsSuccess && response.HasData)
            {
                troopTypes = new List<TroopType>();
                troopInfos = response.Data;

                foreach (var troop in response.Data)
                {
                    if (troopTypes.Contains(troop.Info.Code)) continue;
                    if (troop.Info.Code == TroopType.Other) continue;
                    troopTypes.Add(troop.Info.Code);
                }

                troopBuildingRel = new List<TroopBuildingRelation>()
                {
                    new TroopBuildingRelation(StructureType.Barracks, new List<TroopType>() { TroopType.Swordsman } ),
                    new TroopBuildingRelation(StructureType.ShootingRange, new List<TroopType>() { TroopType.Archer } ),
                    new TroopBuildingRelation(StructureType.Stable, new List<TroopType>() { TroopType.Knight } ),
                    new TroopBuildingRelation(StructureType.Workshop, new List<TroopType>() { TroopType.Slingshot } )
                };

//#if DEBUG
//                foreach (var troops in troopInfos)
//                {
//                    foreach (var troop in troops.Levels)
//                    {
//                        troop.Data.TraningTime = 5;
//                    }
//                }
//#endif
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

            if (troopInfos != null)
            {
                troopInfos.Clear();
                troopInfos = null;
            }

            if (troopTypes != null)
            {
                troopTypes.Clear();
                troopTypes = null;
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameOfRevenge.Business.Manager;
using GameOfRevenge.Business.Manager.GameDef;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models.Hero;

namespace GameOfRevenge.Business.CacheData
{
    public static class CacheHeroDataManager
    {
        public const string HeroNotExist = "Hero item does not exist";

        private static bool isLoaded = false;
        private static List<HeroDataRequirementRel> HeroInfo = null;
        private static List<string> heroTypes;
        private static List<List<HeroDataRel>> heroDataRel;

        public static bool IsLoaded { get => isLoaded && (HeroInfo != null) && (heroDataRel != null); }
        public static IReadOnlyList<IReadOnlyHeroDataRequirementRel> HeroInfos { get { CheckLoadCacheMemory(); return HeroInfo.ToList(); } }
        public static IReadOnlyList<string> ItemTypes { get { CheckLoadCacheMemory(); return heroTypes; } }
        public static List<List<HeroDataRel>> HeroDataRelation { get { CheckLoadCacheMemory(); return heroDataRel; } }

        public static List<HeroDataRel> GetHeroDataRelations(HeroType heroType)
        {
            var hero = GetFullHeroData(heroType.ToString());
            return HeroDataRelation.Find(x => x[0].HeroId == hero.Info.HeroId);
        }

        public static int GetHeroDataRelationID(HeroType heroType, int statType)
        {
            var heroDataTypes = GetHeroDataRelations(heroType);
            return heroDataTypes.Find(x => x.StatType == statType).Id;
        }

        public static IReadOnlyHeroDataRequirementRel GetFullHeroDataID(int heroId)
        {
            var item = HeroInfos.FirstOrDefault(x => x.Info.HeroId == heroId);
            if (item == null)
                throw new CacheDataNotExistExecption(HeroNotExist);
            else
                return item;
        }

        public static IReadOnlyHeroDataRequirementRel GetFullHeroData(string heroType)
        {
            var item = HeroInfos.First(x => x.Info.Code == heroType);
            if (item == null)
                throw new CacheDataNotExistExecption(HeroNotExist+" "+heroType);
            else
                return item;
        }


        #region Cache Check, Load and Clear
        public static async Task LoadCacheMemoryAsync()
        {
            ClearCache();

            var resManager = new HeroManager();
            var response = await resManager.GetAllHeroDatas();

            if (response.IsSuccess)
            {
                HeroInfo = response.Data;
                heroTypes = new List<string>();
                foreach (var item in response.Data)
                {
                    if (string.IsNullOrWhiteSpace(item.Info.Code)) continue;
                    if (item.Info.Code == "Other") continue;
                    if (heroTypes.Contains(item.Info.Code)) continue;
                    heroTypes.Add(item.Info.Code);
                }
            }
            else
            {
                ClearCache();
                throw new CacheDataNotExistExecption(response.Message);
            }

            var response2 = await resManager.GetAllHeroDataRelation();
            if (response2.IsSuccess)
            {
                heroDataRel = response2.Data.GroupBy(x => x.HeroId).Select(y => y.ToList()).ToList();
                isLoaded = true;
            }
            else
            {
                ClearCache();
                throw new CacheDataNotExistExecption(response2.Message);
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

            if (HeroInfo != null)
            {
                HeroInfo.Clear();
                HeroInfo = null;
            }

            if (heroTypes != null)
            {
                heroTypes.Clear();
                heroTypes = null;
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameOfRevenge.Business.Manager;
using GameOfRevenge.Business.Manager.GameDef;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Academy;
using GameOfRevenge.Common.Models.Inventory;

namespace GameOfRevenge.Business.CacheData
{
    public static class CacheAcademyDataManager
    {
        public const string AcademyNotExist = "Academy Data does not exist";

        private static bool isLoaded = false;

        private static List<AcademyItemTable> academyItems = null;
        private static List<AcademyRequirementTable> academyRequirements = null;

        public static bool IsLoaded { get => isLoaded && academyItems != null && academyRequirements != null; }

        public static IReadOnlyList<AcademyItemTable> AllAcademyItems { get { CheckCacheMemory(); return academyItems.ToList(); } }
        public static IReadOnlyList<IReadOnlyAcademyRequirementTable> AllAcademyRequirements { get { CheckCacheMemory(); return academyRequirements.ToList(); } }

        #region Cache Check, Load and Clear

        public static async Task LoadCacheMemoryAsync()
        {
            var academyManager = new AcademyManager();

            var itemsResp = await academyManager.GetAllAcademyItem();
            if (!itemsResp.IsSuccess)
            {
                ClearCache();
                throw new CacheDataNotExistExecption(itemsResp.Message);
            }
            academyItems = itemsResp.Data;

            var dataResp = await academyManager.GetAllAcademyRequirement();
            if (!dataResp.IsSuccess)
            {
                ClearCache();
                throw new CacheDataNotExistExecption(dataResp.Message);
            }
            academyRequirements = dataResp.Data;

            isLoaded = true;
        }

        public static void LoadCacheMemory()
        {
            var tsk = LoadCacheMemoryAsync();
            tsk.Wait();
        }

        public static void CheckCacheMemory()
        {
            if (isLoaded) return;

            var tsk = LoadCacheMemoryAsync();
            tsk.Wait();
        }

        public static void ClearCache()
        {
            isLoaded = false;

            if (academyItems != null) { academyItems.Clear(); academyItems = null; }
            if (academyRequirements != null) { academyRequirements.Clear(); academyRequirements = null; }
        }
        #endregion
    }
}

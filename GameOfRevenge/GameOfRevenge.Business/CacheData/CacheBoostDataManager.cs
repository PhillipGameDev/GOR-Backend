using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameOfRevenge.Business.Manager.GameDef;
using GameOfRevenge.Common.Models.Boost;

namespace GameOfRevenge.Business.CacheData
{
    public static class CacheBoostDataManager
    {
        private const string StructureNotExist = "Boost does not exist";
        private static bool isLoaded = false;
        private static List<BoostTypeRel> boostInfos = null;

        public static IReadOnlyList<IReadOnlyBoostTypeRel> BoostInfos { get { if (boostInfos == null) LoadCacheMemory(); return boostInfos.ToList(); } }

        public static IReadOnlyBoostTypeRel GetFullBoostDataByTypeId(int boostId)
        {
            var data = BoostInfos.FirstOrDefault(x => x.Info.BoostTypeId == boostId);
            if (data == null) throw new CacheDataNotExistExecption(StructureNotExist);
            else return data;
        }

        public static IReadOnlyBoostTypeRel GetFullBoostDataByBoostId(int boostId)
        {
            var data = BoostInfos.FirstOrDefault(x => x.Values.Where(y => y.BoostId == boostId) != null);
            if (data == null) throw new CacheDataNotExistExecption(StructureNotExist);
            else return data;
        }

        public static IReadOnlyBoostTypeRel GetFullBoostDataByType(BoostType boostType)
        {
            var data = BoostInfos.FirstOrDefault(x => x.Info.BoostType == boostType);
            if (data == null) throw new CacheDataNotExistExecption(StructureNotExist);
            else return data;
        }

        public static IReadOnlyBoostTable GetBoostIdByValueData(int boostId, int percentage)
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
        }

        public static async Task LoadCacheMemoryAsync()
        {
            isLoaded = false;

            var resManager = new BoostManager();
            var response = await resManager.GetAllBoostRelData();

            if (response.IsSuccess)
            {
                boostInfos = response.Data;
                isLoaded = true;
            }
            else
            {
                throw new CacheDataNotExistExecption(response.Message);
            }
        }

        public static void LoadCacheMemory()
        {
            var tsk = LoadCacheMemoryAsync();
            tsk.Wait();
        }

        public static void ClearCache()
        {
            if (boostInfos != null)
            {
                boostInfos.Clear();
                boostInfos = null;
            }
        }
    }
}

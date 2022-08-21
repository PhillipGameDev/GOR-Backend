using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameOfRevenge.Business.Manager.GameDef;
using GameOfRevenge.Common.Models.Boost;
using GameOfRevenge.Common.Models.Inventory;

namespace GameOfRevenge.Business.CacheData
{
    public static class CacheBoostDataManager
    {
        private const string StructureNotExist = "Boost does not exist";
        private static bool isLoaded = false;
        //        private static List<BoostTypeRel> boostInfos = null;
        private static List<BoostTypeTable> boostInfos = null;
        private static List<BoostType> boostTypes;

        public static IReadOnlyList<IReadOnlyBoostTypeTable> BoostInfos { get { LoadCacheMemory(); return boostInfos.ToList(); } }

        public static List<BoostType> BoostTypes { get { LoadCacheMemory(); return boostTypes; } }

//        public static IReadOnlyList<IReadOnlyBoostTypeRel> BoostInfos { get { if (boostInfos == null) LoadCacheMemory(); return boostInfos.ToList(); } }

//        public static IReadOnlyList<IReadOnlyBuffItemRel> BuffItemRelations { get { LoadCacheMemory(); return boostTypes; } }

        public static IReadOnlyBoostTypeTable GetFullBoostDataByTypeId(int boostId)
        {
            var data = BoostInfos.FirstOrDefault(x => x.BoostTypeId == boostId);
            if (data == null) throw new CacheDataNotExistExecption(StructureNotExist);
            else return data;
        }

/*        public static IReadOnlyBoostTypeTable GetFullBoostDataByBoostId(int boostId)
        {
            var data = BoostInfos.FirstOrDefault(x => x.Values.Where(y => y.BoostId == boostId) != null);
            if (data == null) throw new CacheDataNotExistExecption(StructureNotExist);
            else return data;
        }*/

        public static IReadOnlyBoostTypeTable GetFullBoostDataByType(BoostType boostType)
        {
            var data = BoostInfos.FirstOrDefault(x => x.BoostType == boostType);
            if (data == null) throw new CacheDataNotExistExecption(StructureNotExist);
            else return data;
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

            var tsk = LoadCacheMemoryAsync();
            tsk.Wait();
        }

        public static async Task LoadCacheMemoryAsync()
        {
            isLoaded = false;

            var resManager = new BoostManager();
            var response = await resManager.GetAllBoostTypes();// GetAllBoostRelData();

            if (response.IsSuccess)
            {
                boostTypes = new List<BoostType>((BoostType[])Enum.GetValues(typeof(BoostType)));
//                boostTypes = LoadBuffs();
                boostInfos = response.Data;
                isLoaded = true;
            }
            else
            {
                throw new CacheDataNotExistExecption(response.Message);
            }
        }

        public static void ClearCache()
        {
            if (boostTypes != null)
            {
                boostTypes.Clear();
                boostTypes = null;
            }

            if (boostInfos != null)
            {
                boostInfos.Clear();
                boostInfos = null;
            }
            isLoaded = false;
        }
    }
}

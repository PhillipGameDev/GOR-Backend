using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameOfRevenge.Business.Manager;
using GameOfRevenge.Business.Manager.GameDef;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Interface.Model;
using GameOfRevenge.Common.Models;

namespace GameOfRevenge.Business.CacheData
{
    public static class CacheResourceDataManager
    {
        private static bool isLoaded = false;
        private static List<ResourceTable> resourceInfos = null;
        private static ResourceTable food = null;
        private static ResourceTable wood = null;
        private static ResourceTable ore = null;
        private static ResourceTable gems = null;
        private static ResourceTable gold = null;

        private static ResourceTable steel = null;
        private static ResourceTable stone = null;
        private static ResourceTable ruby = null;

        public static IReadOnlyList<IReadOnlyResourceTable> ResourceInfos { get { if (resourceInfos == null) LoadCacheMemory(); return resourceInfos.ToList(); } }
        public static IReadOnlyResourceTable Food { get { if (food == null) LoadCacheMemory(); return food; } }
        public static IReadOnlyResourceTable Wood { get { if (wood == null) LoadCacheMemory(); return wood; } }
        public static IReadOnlyResourceTable Ore { get { if (ore == null) LoadCacheMemory(); return ore; } }
        public static IReadOnlyResourceTable Gems { get { if (gems == null) LoadCacheMemory(); return gems; } }
        public static IReadOnlyResourceTable Gold { get { if (gold == null) LoadCacheMemory(); return gold; } }

        public static IReadOnlyResourceTable Steel { get { if (steel == null) LoadCacheMemory(); return steel; } }
        public static IReadOnlyResourceTable Stone { get { if (stone == null) LoadCacheMemory(); return stone; } }
        public static IReadOnlyResourceTable Ruby { get { if (ruby == null) LoadCacheMemory(); return ruby; } }

        public static IReadOnlyDataRequirement NewGemRequirement(int value)
        {
            return new DataRequirement()
            {
                ValueId = Gems.Id, 
                DataType = DataType.Resource, 
                Value = value 
            };
        }

        public static IReadOnlyResourceTable GetResourceData(int id)
        {
            if (id <= 0) throw new InvalidModelExecption("Invalid id was provided");
            if (resourceInfos == null) LoadCacheMemory();
            var res = resourceInfos.Find(x => x.Id == id);
            if (res == null) throw new DataNotExistExecption($"Resource of id:{id} was not found");
            else return res;
        }

        public static IReadOnlyResourceTable GetResourceData(ResourceType type)
        {
            if (type == ResourceType.Other) throw new InvalidModelExecption("Invalid type was provided");
            if (resourceInfos == null) LoadCacheMemory();
            var res = resourceInfos.Find(x => (x.Code == type));
            if (res == null) throw new DataNotExistExecption($"Resource of type:{type} was not found");
            else return res;
        }

        public static async Task LoadCacheMemoryAsync()
        {
            isLoaded = false;

            var resManager = new ResourceManager();
            var response = await resManager.GetAllResources();

            if (response.IsSuccess)
            {
                resourceInfos = response.Data;
                food = resourceInfos.Find(x => (x.Code == ResourceType.Food));
                wood = resourceInfos.Find(x => (x.Code == ResourceType.Wood));
                ore = resourceInfos.Find(x => (x.Code == ResourceType.Ore));
                gems = resourceInfos.Find(x => (x.Code == ResourceType.Gems));
                gold = resourceInfos.Find(x => (x.Code == ResourceType.Gold));

                steel = resourceInfos.Find(x => (x.Code == ResourceType.Red));
                stone = resourceInfos.Find(x => (x.Code == ResourceType.Green));
                ruby = resourceInfos.Find(x => (x.Code == ResourceType.Blue));

                isLoaded = true;
            }
            else
            {
                throw new DataNotExistExecption(response.Message);
            }
        }

        public static void LoadCacheMemory()
        {
            var tsk = LoadCacheMemoryAsync();
            tsk.Wait();
        }

        public static void ClearCache()
        {
            if (resourceInfos != null)
            {
                resourceInfos.Clear();
                resourceInfos = null;
            }

            food = null;
            wood = null;
            ore = null;
            gems = null;
            gold = null;
            steel = null;
            stone = null;
            ruby = null;
        }
    }
}

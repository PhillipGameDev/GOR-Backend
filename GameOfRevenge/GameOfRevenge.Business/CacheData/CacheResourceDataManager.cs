﻿using System.Collections.Generic;
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

        public static IReadOnlyList<IReadOnlyResourceTable> ResourceInfos { get { if (resourceInfos == null) LoadCacheMemory(); return resourceInfos.ToList(); } }
        public static IReadOnlyResourceTable Food { get { if (food == null) LoadCacheMemory(); return food; } }
        public static IReadOnlyResourceTable Wood { get { if (wood == null) LoadCacheMemory(); return wood; } }
        public static IReadOnlyResourceTable Ore { get { if (ore == null) LoadCacheMemory(); return ore; } }
        public static IReadOnlyResourceTable Gems { get { if (gems == null) LoadCacheMemory(); return gems; } }

        public static IReadOnlyDataRequirement GetGemReq(int value)
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
            var res = resourceInfos.Where(x => x.Id == id).FirstOrDefault();
            if (res == null) throw new DataNotExistExecption($"Resource of id:{id} was not found");
            else return res;
        }

        public static IReadOnlyResourceTable GetResourceData(ResourceType type)
        {
            if (type == ResourceType.Other) throw new InvalidModelExecption("Invalid type was provided");
            if (resourceInfos == null) LoadCacheMemory();
            var res = resourceInfos.Where(x => x.Code == type).FirstOrDefault();
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
                food = resourceInfos.Where(x => x.Code == ResourceType.Food).FirstOrDefault();
                wood = resourceInfos.Where(x => x.Code == ResourceType.Wood).FirstOrDefault();
                ore = resourceInfos.Where(x => x.Code == ResourceType.Ore).FirstOrDefault();
                gems = resourceInfos.Where(x => x.Code == ResourceType.Gems).FirstOrDefault();

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
        }
    }
}

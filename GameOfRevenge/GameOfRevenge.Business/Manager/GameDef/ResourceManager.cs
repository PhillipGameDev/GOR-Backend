using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Services;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Interface.Model;
using GameOfRevenge.Business.CacheData;

namespace GameOfRevenge.Business.Manager.GameDef
{
    public class ResourceManager : BaseManager, IResourceManager
    {
        public async Task<Response<List<ResourceTable>>> GetAllResources()
        {
            try
            {
                return await Db.ExecuteSPMultipleRow<ResourceTable>("GetAllResources");
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<ResourceTable>>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<ResourceTable>>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }
    }
}

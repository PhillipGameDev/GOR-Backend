using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Services;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Inventory;

namespace GameOfRevenge.Business.Manager.GameDef
{
    public class InventoryManager : BaseManager
    {
        public async Task<Response<List<InventoryDataTable>>> GetAllInventoryItems()
        {
            try
            {
                return await Db.ExecuteSPMultipleRow<InventoryDataTable>("GetAllInventoryItems");
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<InventoryDataTable>>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<InventoryDataTable>>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

/*        public async Task<Response<List<DataRequirement>>> GetAllInventoryRequirements()
        {
            try
            {
                return await Db.ExecuteSPMultipleRow<DataRequirement>("GetAllInventoryRequirements");
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<DataRequirement>>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<DataRequirement>>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }

        }*/

/*        public async Task<Response<List<InventoryTable>>> GetAllInventoryItemDatas()
        {
            try
            {
                var infos = await GetAllInventoryItems();
                var reqs = await GetAllInventoryRequirements();
                if (!infos.IsSuccess) throw new InvalidModelExecption(infos.Message);
                if (!reqs.IsSuccess) throw new InvalidModelExecption(reqs.Message);
                var resp = new Response<List<InventoryTable>>(new List<InventoryTable>(), infos.Case, infos.Message);

                foreach (var info in infos.Data)
                {
                    var data = new InventoryDataRequirementRel
                    {
                        Info = info,
                        Requirements = new List<DataRequirement>()
                    };

                    foreach (var req in reqs.Data)
                    {
                        if (info.Id == req.DataId)
                        {
                            data.Requirements.Add(req);
                        }
                    }

                    resp.Data.Add(data);
                }

                return resp;
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<InventoryDataRequirementRel>>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<InventoryDataRequirementRel>>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }*/
    }
}

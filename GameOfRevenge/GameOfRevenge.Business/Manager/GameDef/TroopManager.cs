using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Troop;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameOfRevenge.Business.Manager.GameDef
{
    public class TroopManager : BaseManager, ITroopManager
    {
        public async Task<Response<List<TroopDataRequirementRel>>> GetAllTroopDataRequirementRel()
        {
            try
            {
                var respTroops = await GetAllTroops();
                var respTroopDatas = await GetAllTroopDatas();
                var respTroopReqs = await GetAllTroopDataRequirements();

                if (respTroops.IsSuccess && respTroopDatas.IsSuccess && respTroopReqs.IsSuccess)
                {
                    var troops = respTroops.Data;
                    var troopDatas = respTroopDatas.Data;
                    var troopReqs = respTroopReqs.Data;
                    var finalData = new List<TroopDataRequirementRel>();

                    foreach (var item in troops)
                    {
                        var structDataReqRel = new TroopDataRequirementRel
                        {
                            Info = item,
                            Levels = new List<TroopDataRequirements>()
                        };

                        foreach (var itemData in troopDatas.Where(x => x.InfoId == item.Id))
                        {
                            var structDataReq = new TroopDataRequirements
                            {
                                Data = itemData,
                                Requirements = troopReqs.Where(x => x.DataId == itemData.DataId).ToList()
                            };

                            structDataReqRel.Levels.Add(structDataReq);
                        }

                        finalData.Add(structDataReqRel);
                    }

                    return new Response<List<TroopDataRequirementRel>>(100, "Fetched all troop data")
                    {
                        Data = finalData,
                        PageDetails = null
                    };
                }
                else
                {
                    return new Response<List<TroopDataRequirementRel>>(0, "Server Error");
                }
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<TroopDataRequirementRel>>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<TroopDataRequirementRel>>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<List<TroopDataTable>>> GetAllTroopDatas()
        {
            try
            {
                return await Db.ExecuteSPMultipleRow<TroopDataTable>("GetAllTroopDatas");
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<TroopDataTable>>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<TroopDataTable>>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<List<DataRequirement>>> GetAllTroopDataRequirements()
        {
            try
            {
                return await Db.ExecuteSPMultipleRow<DataRequirement>("GetAllTroopDataRequirements");
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
        }

        public async Task<Response<List<TroopTable>>> GetAllTroops()
        {
            try
            {
                return await Db.ExecuteSPMultipleRow<TroopTable>("GetAllTroops");
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<TroopTable>>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<TroopTable>>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }
    }
}

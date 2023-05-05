using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Technology;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Services;

namespace GameOfRevenge.Business.Manager.GameDef
{
    public class TechnologyManager : BaseManager, ITechnologyManager
    {
        public async Task<Response<List<TechnologyDataRequirementRel>>> GetAllTechnologyDataRequirementRel()
        {
            try
            {
                var respTech = await GetAllTechnologys();
                var respTechDatas = await GetAllTechnologyDatas();
                var respTechReqs = await GetAllTechnologyDataRequirements();

                if (respTech.IsSuccess && respTechDatas.IsSuccess && respTechReqs.IsSuccess)
                {
                    var Technologys = respTech.Data;
                    var techDatas = respTechDatas.Data;
                    var techReqs = respTechReqs.Data;

                    var finalData = new List<TechnologyDataRequirementRel>();

                    foreach (var item in Technologys)
                    {

                        var lvls = new List<TechnologyDataRequirements>();

                        foreach (var itemData in techDatas.Where(x => x.InfoId == item.Id))
                        {
                            var requirements = techReqs.Where(x => x.DataId == itemData.DataId).ToList();
                            lvls.Add(new TechnologyDataRequirements(itemData, requirements));
                        }

                        var techDataReqRel = new TechnologyDataRequirementRel
                        {
                            Info = item,
                            Levels = lvls
                        };

                        finalData.Add(techDataReqRel);
                    }


                    return new Response<List<TechnologyDataRequirementRel>>(100, "Fetched all technology data")
                    {
                        Data = finalData,
                        PageDetails = null
                    };
                }
                else
                {
                    return new Response<List<TechnologyDataRequirementRel>>(0, "Error fetching technology data");
                }
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<TechnologyDataRequirementRel>>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<TechnologyDataRequirementRel>>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<List<TechnologyDataTable>>> GetAllTechnologyDatas()
        {
            try
            {
                return await Db.ExecuteSPMultipleRow<TechnologyDataTable>("GetAllTechnologyData");
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<TechnologyDataTable>>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<TechnologyDataTable>>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<List<DataRequirement>>> GetAllTechnologyDataRequirements()
        {
            try
            {
                return await Db.ExecuteSPMultipleRow<DataRequirement>("GetAllTechnologyDataRequirement");
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

        public async Task<Response<List<TechnologyTable>>> GetAllTechnologys()
        {
            try
            {
                return await Db.ExecuteSPMultipleRow<TechnologyTable>("GetAllTechnology");
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<TechnologyTable>>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<TechnologyTable>>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }
    }
}

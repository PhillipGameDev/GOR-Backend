using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Services;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common.Models.Troop;

namespace GameOfRevenge.Business.Manager.GameDef
{
    public class StructureManager : BaseManager, IStructureManager
    {
        public async Task<Response<List<TroopTable>>> GetAllStructDataRequirementRel2()
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

        public async Task<Response<List<StructureDataRequirementRel>>> GetAllStructDataRequirementRel()
        {
            System.Console.WriteLine("ENTRO ACA");
            try
            {
                var respStructs = await GetAllStructures();
                var respStructDatas = await GetAllStructureLevelDatas();
                var respStructReqs = await GetAllStructureLevelRequirements();
                var respStructLocs = await GetAllStructureLocation();
                var respStructBuildLimit = await GetAllStructureBuildLimit();

                if (respStructs.IsSuccess && respStructDatas.IsSuccess && respStructReqs.IsSuccess && respStructLocs.IsSuccess && respStructBuildLimit.IsSuccess)
                {
                    var structs = respStructs.Data;
                    var structDatas = respStructDatas.Data;
                    var structReqs = respStructReqs.Data;
                    var structLocs = respStructLocs.Data;
                    var finalData = new List<StructureDataRequirementRel>();

                    foreach (var structure in structs)
                    {
                        var structDataReqRel = new StructureDataRequirementRel
                        {
                            Info = structure,
                            Levels = new List<StructureDataRequirement>(),
                            Locations = new List<int>(),
                            BuildLimit = new Dictionary<string, int>()
                        };

                        foreach (var itemData in structDatas.Where(x => x.Id == structure.Id))
                        {
                            var structDataReq = new StructureDataRequirement
                            {
                                Data = itemData,
                                Requirements = structReqs.Where(x => x.DataId == itemData.DataId).ToList()
                            };

                            structDataReqRel.Levels.Add(structDataReq);
                        }

                        foreach (var loc in structLocs)
                        {
                            if (loc.StructureType == structure.Code)
                            {
                                structDataReqRel.Locations.Add(loc.Location);
                            }
                        }

                        foreach (var buildLimit in respStructBuildLimit.Data)
                        {
                            if (buildLimit.BuildStructure == structure.Code)
                            {
                                structDataReqRel.BuildLimit.Add(buildLimit.TownHallLevel.ToString(), buildLimit.MaxBuildCount);
                            }
                        }

                        finalData.Add(structDataReqRel);
                    }

                    return new Response<List<StructureDataRequirementRel>>(CaseType.Success, "Fetched all structure data")
                    {
                        Data = finalData,
                        PageDetails = null
                    };
                }
                else
                {
                    return new Response<List<StructureDataRequirementRel>>(CaseType.Invalid, "Server Error");
                }
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<StructureDataRequirementRel>>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<StructureDataRequirementRel>>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<List<StructureTable>>> GetAllStructures() => await Db.ExecuteSPMultipleRow<StructureTable>("GetAllStructures");
        public async Task<Response<List<DataRequirement>>> GetAllStructureLevelRequirements() => await Db.ExecuteSPMultipleRow<DataRequirement>("GetAllStructureLevelRequirements");
        public async Task<Response<List<StructureDataTable>>> GetAllStructureLevelDatas() => await Db.ExecuteSPMultipleRow<StructureDataTable>("GetAllStructureLevelDatas");
        public async Task<Response<List<StructureBuildLimitTable>>> GetAllStructureBuildLimit() => await Db.ExecuteSPMultipleRow<StructureBuildLimitTable>("GetAllStructureBuildLimit");
        public async Task<Response<List<StructureLocationRelTable>>> GetAllStructureLocation() => await Db.ExecuteSPMultipleRow<StructureLocationRelTable>("GetAllStructureLocation");
    }
}

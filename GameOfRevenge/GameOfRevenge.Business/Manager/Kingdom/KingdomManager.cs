using System;
using System.Threading.Tasks;
using GameOfRevenge.Common.Services;
using GameOfRevenge.Common;
using System.Collections.Generic;
using Newtonsoft.Json;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Models.Kingdom;

namespace GameOfRevenge.Business.Manager.Kingdom
{
    public class KingdomManager : BaseManager, IKingdomManager
    {
        public async Task<Response<WorldTable>> CreateWorld() => await CreateWorld(Guid.NewGuid().ToString());
        public async Task<Response<WorldTable>> CreateWorld(string code)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "Name", "WorldName" },
                    { "Code", code }
                };

                return await Db.ExecuteSPSingleRow<WorldTable>("CreateWorld", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<WorldTable>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<WorldTable>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<WorldTable>> GetWorld(int id)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "WorldId", id },
                };

                return await Db.ExecuteSPSingleRow<WorldTable>("GetWorldById", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<WorldTable>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<WorldTable>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<WorldTable>> GetWorld(string code)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "WorldCode", code },
                };

                return await Db.ExecuteSPSingleRow<WorldTable>("GetWorldByCode", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<WorldTable>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<WorldTable>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<WorldDataTable>> GetWorldTileData(int id)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "TileId", id },
                };
                return await Db.ExecuteSPSingleRow<WorldDataTable>("GetWorldTileData", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<WorldDataTable>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<WorldDataTable>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<List<WorldDataTable>>> GetWorldTilesData(int id)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "WorldId", id },
                };
                return await Db.ExecuteSPMultipleRow<WorldDataTable>("GetWorldTilesData", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<WorldDataTable>>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<WorldDataTable>>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<WorldDataTable>> UpdateWorldTileData(int id, int x, int y, WorldTileData data)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "WorldId", id },
                    { "X", x },
                    { "Y", y },
                    { "Data", JsonConvert.SerializeObject(data) },
                };

                return await Db.ExecuteSPSingleRow<WorldDataTable>("UpdateWorldTileData", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<WorldDataTable>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<WorldDataTable>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }
    }
}

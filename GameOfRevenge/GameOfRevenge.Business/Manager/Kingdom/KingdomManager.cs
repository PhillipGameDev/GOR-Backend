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
using GameOfRevenge.Common.Models;

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

        public async Task<Response<WorldTable>> GetWorld(int worldId)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "WorldId", worldId }
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
                    { "WorldCode", code }
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

        public async Task<Response<IntValue>> GetWorldTileCount(int worldId)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "WorldId", worldId }
                };

                return await Db.ExecuteSPSingleRow<IntValue>("GetWorldTileCount", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<IntValue>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<IntValue>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<List<PlayerID>>> GetWorldZonePlayers(int minX, int minY, int maxX, int maxY)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "MinX", minX },
                    { "MinY", minY },
                    { "MaxX", maxX },
                    { "MaxY", maxY },
                    { "GetCoords", 1 },
                    { "GetTileId", 1 }
                };

                return await Db.ExecuteSPMultipleRow<PlayerID>("GetWorldZonePlayers", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<PlayerID>>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<PlayerID>>()
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

        public async Task<Response<WorldTable>> UpdateWorld(int worldId, int? currentZone)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "WorldId", worldId }
                };
                if (currentZone != null) spParams.Add("CurrentZone", currentZone);

                return await Db.ExecuteSPSingleRow<WorldTable>("UpdateWorld", spParams);
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

        public async Task<Response<WorldDataTable>> UpdateWorldTileData(int x, int y, int? tileId = null, int? worldId = null)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "X", x },
                    { "Y", y }
//                    { "Data", JsonConvert.SerializeObject(data) },
                };
                if (tileId != null) spParams.Add("WorldTileId", tileId);
                if (worldId != null) spParams.Add("WorldId", worldId);

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

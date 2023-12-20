using System.Threading.Tasks;
using System.Collections.Generic;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common.Models.Monster;
using GameOfRevenge.Common.Models.Inventory;
using GameOfRevenge.Common.Services;
using GameOfRevenge.Common;
using System;
using GameOfRevenge.Common.Models.Kingdom;
using System.Linq;
using GameOfRevenge.Business.CacheData;

namespace GameOfRevenge.Business.Manager.GameDef
{
    public class MonsterManager : BaseManager, IMonsterManager
    {
        public async Task<Response<List<MonsterDataTable>>> GetAllMonsterData()
        {
            try
            {
                return await Db.ExecuteSPMultipleRow<MonsterDataTable>("GetAllMonsterDatas");
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<MonsterDataTable>>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<MonsterDataTable>>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<List<MonsterTable>>> GetMonsterWorldData(int worldId = 0)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "WorldId", worldId }
                };

                return await Db.ExecuteSPMultipleRow<MonsterTable>("GetMonsterWorldData", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<MonsterTable>>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<MonsterTable>>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<List<MonsterTable>>> GetMonstersByWorldTileId(int worldTileId = 0)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "WorldTileId", worldTileId }
                };

                return await Db.ExecuteSPMultipleRow<MonsterTable>("GetMonsterWorldData", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<MonsterTable>>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<MonsterTable>>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<MonsterTable>> GetNearestMonsterByPlayerId(int playerId = 0)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId }
                };

                return await Db.ExecuteSPSingleRow<MonsterTable>("GetNearestMonsterByPlayerId", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<MonsterTable>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<MonsterTable>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<(int, int)> AddNewMonster(WorldTable world, List<MonsterTable> monsters, int x, int y, Random random, Action<string> log = null)
        {
            var monsterData = CacheMonsterManager.AllItems[random.Next(CacheMonsterManager.AllItems.Count)];

            var resp = await AddMonsterToWorld(world.Id, monsterData.Id, x, y);

            log?.Invoke(resp.Message + "," + resp.IsSuccess + "," + x + "," + y + "," + monsterData.Id + ", " + world.Id);

            return (x, y);
        }

        public async Task<Response> AddMonsterToWorld(int worldId, int monsterId, int x, int y)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "WorldId", worldId },
                    { "MonsterDataId", monsterId },
                    { "X", x },
                    { "Y", y }
                };

                return await Db.ExecuteSPNoData("AddMonsterToWorld", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response()
                {
                    Case = 200,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<MonsterTable>> GetFullMonsterData(int monsterId)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "MonsterId", monsterId }
                };

                return await Db.ExecuteSPSingleRow<MonsterTable>("GetMonsterWorldData", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<MonsterTable>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<MonsterTable>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response> UpdateMonsterHealth(int monsterId, int health)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "MonsterWorldId", monsterId },
                    { "Health", health }
                };

                return await Db.ExecuteSPNoData("UpdateMonsterHealthWithRespawn", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response()
                {
                    Case = 200,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }
    }
}

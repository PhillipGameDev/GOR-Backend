﻿using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Services;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Kingdom;
using GameOfRevenge.Business.Manager.GameDef;
using GameOfRevenge.Common.Models.Monster;

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

        public async Task AddMonsters(WorldTable world, int zone)
        {
            var monsterManager = new MonsterManager();
            var resp = await monsterManager.GetMonsterWorldData(world.Id);

            if (!resp.IsSuccess) return;

            var monsters = resp.Data;
            int offsetX = (zone % world.ZoneX) * world.ZoneSize;
            int offsetY = (zone / world.ZoneY) * world.ZoneSize;

            var random = new Random();

            for (int x = 0; x < world.ZoneSize; x++)
            {
                for (int y = 0; y < world.ZoneSize; y++)
                {
                    int finalX = offsetX + x, finalY = offsetY + y;

                    for (int l = monsters.FindAll(m => m.X == finalX && m.Y == finalY).Count; l < 3; l++)
                    {
                        var ms = await monsterManager.AddNewMonster(world, monsters, finalX, finalY, random);
                        monsters.Add(new MonsterTable()
                        {
                            X = ms.Item1,
                            Y = ms.Item2
                        });
                    }
                }
            }
        }

        public async Task<Response<ZoneFortressTable>> AddZoneFortress(int worldId, int zoneIndex)
        {
            var spParams = new Dictionary<string, object>()
            {
                { "WorldId", worldId },
                { "ZoneIndex", zoneIndex }
            };

            try
            {
                var resp = await Db.ExecuteSPSingleRow<ZoneFortressTable>("AddZoneFortress", spParams);
                if (!resp.IsSuccess || !resp.HasData) throw new InvalidModelExecption(resp.Message);

                var fortressResp = resp.Data;

                return new Response<ZoneFortressTable>()
                {
                    Case = resp.Case,
                    Data = resp.Data,
                    Message = resp.Message
                };
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<ZoneFortressTable>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<ZoneFortressTable>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<List<ZoneFortressTable>>> GetAllZoneFortress()
        {
            try
            {
                return await Db.ExecuteSPMultipleRow<ZoneFortressTable>("GetAllZoneFortress", null);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<ZoneFortressTable>>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<ZoneFortressTable>>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<ZoneFortress>> GetZoneFortressByIndex(int playerId, int zoneIndex)
        {
            var spParams = new Dictionary<string, object>()
            {
                { "PlayerId", playerId },
                { "ZoneIndex", zoneIndex }
            };

            try
            {
                var resp = await Db.ExecuteSPSingleRow<ZoneFortressTable>("GetZoneFortressByIndex", spParams);
                if (!resp.IsSuccess || !resp.HasData) throw new InvalidModelExecption(resp.Message);

                var fortressResp = resp.Data;

                ZoneFortress fortress = null;
                if (!string.IsNullOrEmpty(fortressResp.Data))
                {
                    fortress = Newtonsoft.Json.JsonConvert.DeserializeObject<ZoneFortress>(fortressResp.Data);
                }
                if (fortress == null) fortress = new ZoneFortress();

                fortress.ZoneFortressId = fortressResp.ZoneFortressId;
                fortress.WorldId = fortressResp.WorldId;
                fortress.ZoneIndex = fortressResp.ZoneIndex;
                fortress.HitPoints = fortressResp.HitPoints;
                fortress.Attack = fortressResp.Attack;
                fortress.Defense = fortressResp.Defense;
                fortress.Finished = fortressResp.Finished;
                fortress.ClanId = fortressResp.ClanId;
                fortress.PlayerId = fortressResp.PlayerId;
                fortress.Name = fortressResp.Name;

                return new Response<ZoneFortress>()
                {
                    Case = resp.Case,
                    Data = fortress,
                    Message = resp.Message
                };
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<ZoneFortress>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<ZoneFortress>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<ZoneFortress>> GetZoneFortressById(int zoneFortressId)
        {
            var spParams = new Dictionary<string, object>()
            {
                { "ZoneFortressId", zoneFortressId }
            };

            try
            {
                var resp = await Db.ExecuteSPSingleRow<ZoneFortressTable>("GetZoneFortressById", spParams);
                if (!resp.IsSuccess || !resp.HasData) throw new InvalidModelExecption(resp.Message);

                var fortressResp = resp.Data;

                ZoneFortress fortress = null;
                if (!string.IsNullOrEmpty(fortressResp.Data))
                {
                    fortress = Newtonsoft.Json.JsonConvert.DeserializeObject<ZoneFortress>(fortressResp.Data);
                }
                if (fortress == null) fortress = new ZoneFortress();

                fortress.ZoneFortressId = fortressResp.ZoneFortressId;
                fortress.WorldId = fortressResp.WorldId;
                fortress.ZoneIndex = fortressResp.ZoneIndex;
                fortress.HitPoints = fortressResp.HitPoints;
                fortress.Attack = fortressResp.Attack;
                fortress.Defense = fortressResp.Defense;
                fortress.Finished = fortressResp.Finished;
                fortress.ClanId = fortressResp.ClanId;
                fortress.PlayerId = fortressResp.PlayerId;
                fortress.Name = fortressResp.Name;

                return new Response<ZoneFortress>()
                {
                    Case = resp.Case,
                    Data = fortress,
                    Message = resp.Message
                };
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<ZoneFortress>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<ZoneFortress>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<ZoneFortress>> UpdateZoneFortress(int zoneFortressId, int? hitPoints = null,
                                                    int? attack = null, int? defense = null,
                                                    int? playerId = null, bool? finished = null,
                                                    string data = null)
        {
            var spParams = new Dictionary<string, object>()
            {
                { "ZoneFortressId", zoneFortressId }
            };
            if (hitPoints != null) spParams.Add("HitPoints", hitPoints);
            if (attack != null) spParams.Add("Attack", attack);
            if (defense != null) spParams.Add("Defense", defense);
            if (playerId != null) spParams.Add("PlayerId", playerId);
            if (finished != null) spParams.Add("Finished", finished);
            if (data != null) spParams.Add("Data", data);

            try
            {
                var resp = await Db.ExecuteSPSingleRow<ZoneFortressTable>("UpdateZoneFortress", spParams);
                if (!resp.IsSuccess || !resp.HasData) throw new InvalidModelExecption(resp.Message);

                var fortressResp = resp.Data;

                ZoneFortress fortress = null;
                if (!string.IsNullOrEmpty(fortressResp.Data))
                {
                    fortress = Newtonsoft.Json.JsonConvert.DeserializeObject<ZoneFortress>(fortressResp.Data);
                }
                if (fortress == null) fortress = new ZoneFortress();

                fortress.ZoneFortressId = fortressResp.ZoneFortressId;
                fortress.WorldId = fortressResp.WorldId;
                fortress.ZoneIndex = fortressResp.ZoneIndex;
                fortress.HitPoints = fortressResp.HitPoints;
                fortress.Attack = fortressResp.Attack;
                fortress.Defense = fortressResp.Defense;
                fortress.Finished = fortressResp.Finished;
                fortress.ClanId = fortressResp.ClanId;
                fortress.PlayerId = fortressResp.PlayerId;
                fortress.Name = fortressResp.Name;

                return new Response<ZoneFortress>()
                {
                    Case = resp.Case,
                    Data = fortress,
                    Message = resp.Message
                };
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<ZoneFortress>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<ZoneFortress>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<GloryKingdomData>> GetGloryKingdomDetails()
        {
            try
            {
                return await Db.ExecuteSPSingleRow<GloryKingdomData>("GetGloryKingdomDetails", null);
            }
            catch (Exception ex)
            {
                return new Response<GloryKingdomData>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<GloryKingdomData>> CreateGloryKingdomEvent(DateTime startTime, DateTime endTime)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "StartTime", startTime },
                    { "EndTime", endTime }
                };

                return await Db.ExecuteSPSingleRow<GloryKingdomData>("AddGloryKingdomDetails", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<GloryKingdomData>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<GloryKingdomData>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }
    }
}

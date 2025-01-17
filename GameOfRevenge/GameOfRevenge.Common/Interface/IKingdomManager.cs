﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Kingdom;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Common.Interface
{
    public interface IKingdomManager
    {
        Task<Response<WorldTable>> CreateWorld();
        Task<Response<WorldTable>> CreateWorld(string code);
        Task<Response<WorldTable>> GetWorld(int worldId);
        Task<Response<WorldTable>> GetWorld(string code);
        Task<Response<WorldTable>> UpdateWorld(int worldId, int? currentZone);
        Task<Response<IntValue>> GetWorldTileCount(int worldId);
        Task<Response<List<PlayerID>>> GetWorldZonePlayers(int minX, int minY, int maxX, int maxY);
        Task<Response<WorldDataTable>> GetWorldTileData(int id);
        Task<Response<List<WorldDataTable>>> GetWorldTilesData(int id);
        Task<Response<WorldDataTable>> UpdateWorldTileData(int x, int y, int? tileId = null, int? worldId = null);

        Task AddMonsters(WorldTable world, int zone);
        Task<Response<ZoneFortressTable>> AddZoneFortress(int worldId, int zoneIndex);
        Task<Response<List<ZoneFortressTable>>> GetAllZoneFortress();
        Task<Response<ZoneFortress>> GetZoneFortressByIndex(int playerId, int zoneIndex);
        Task<Response<ZoneFortress>> GetZoneFortressById(int zoneFortressId);
        Task<Response<ZoneFortress>> UpdateZoneFortress(int zoneFortressId, int? hitPoints = null,
                                                            int? attack = null, int? defense = null,
                                                            int? playerId = null, bool? finished = null,
                                                            string data = null);

        Task<Response<GloryKingdomData>> GetGloryKingdomDetails();
        Task<Response<GloryKingdomData>> CreateGloryKingdomEvent(DateTime startTime, DateTime endTime);
    }
}

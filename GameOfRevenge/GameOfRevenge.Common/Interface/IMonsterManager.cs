using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Kingdom;
using GameOfRevenge.Common.Models.Monster;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Common.Interface
{
    public interface IMonsterManager
    {
        Task<Response<List<MonsterDataTable>>> GetAllMonsterData();
        Task<Response<List<MonsterTable>>> GetMonsterWorldData(int worldId = 0);
        Task<Response<List<MonsterTable>>> GetMonstersByWorldTileId(int worldTileId = 0);
        Task<Response<MonsterTable>> GetNearestMonsterByPlayerId(int playerId = 0);
        Task<Response> UpdateMonsterHealth(int monsterId, int health);
        Task<(int, int)> AddNewMonster(WorldTable world, List<MonsterTable> monsters, int x, int y, Random random, Action<string> log = null);
        Task<Response> AddMonsterToWorld(int worldId, int monsterId, int x, int y);
    }
}

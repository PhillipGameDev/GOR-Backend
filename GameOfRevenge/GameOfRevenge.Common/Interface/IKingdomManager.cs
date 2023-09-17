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
    }
}

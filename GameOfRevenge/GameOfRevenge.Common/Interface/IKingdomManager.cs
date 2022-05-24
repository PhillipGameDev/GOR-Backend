using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common.Models.Kingdom;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Common.Interface
{
    public interface IKingdomManager
    {
        Task<Response<WorldTable>> CreateWorld();
        Task<Response<WorldTable>> CreateWorld(string code);
        Task<Response<WorldTable>> GetWorld(int id);
        Task<Response<WorldTable>> GetWorld(string code);
        Task<Response<List<WorldDataTable>>> GetWorldTileData(int id);
        Task<Response<WorldDataTable>> UpdateWorldTileData(int id, int x, int y, WorldTileData data);
    }
}

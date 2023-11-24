using System.Collections.Generic;
using System.Threading.Tasks;
using ExitGames.Concurrency.Fibers;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Kingdom;
using GameOfRevenge.Common.Models.Monster;
using GameOfRevenge.GameHandlers;
using GameOfRevenge.Model;

namespace GameOfRevenge.Interface
{
    public interface IWorld
    {
        int WorldId { get; }
        string Name { get; }
        int ZoneSize { get; }
        int TilesX { get; }
        int TilesY { get; }
        Region[][] WorldRegions { get; }
//        Vector TileDimensions { get; }
        IPlayersManager PlayersManager { get; }

        List<PlayerID> WorldPlayers { get; }
        List<MonsterTable> WorldMonsters { get; }
        List<ZoneFortressTable> WorldForts { get; }

        IFiber WorldFiber { get; }

        Task<(PlayerInstance, IInterestArea)> AllocatePlayerAsync(int playerId, PlayerInfo playerInfo);
        bool CheckSameZone(Region p1, Region p2);
        int GetDistance(Region p1, Region p2);
        int GetDistance(int p1x, int p1y, int p2x, int p2y);
        void Dispose();
    }
}

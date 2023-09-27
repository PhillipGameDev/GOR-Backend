using System.Collections.Generic;
using System.Threading.Tasks;
using ExitGames.Concurrency.Fibers;
using GameOfRevenge.Common.Models;
using GameOfRevenge.GameHandlers;
using GameOfRevenge.Model;

namespace GameOfRevenge.Interface
{
    public interface IWorld
    {
        int WorldId { get; }
        string Name { get; }
        int SizeX { get; }
        int SizeY { get; }
        Region[][] WorldRegions { get; }
        Vector TileDimensions { get; }
        IPlayersManager PlayersManager { get; }

        List<PlayerID> WorldPlayers { get; }
        List<EntityID> WorldMonsters { get; }

        IFiber WorldFiber { get; }

        Task<(PlayerInstance, IInterestArea)> AllocatePlayerAsync(int playerId, PlayerInfo playerInfo);
        bool CheckSameZone(Region p1, Region p2);
        int GetDistance(Region p1, Region p2);
        int GetDistance(int p1x, int p1y, int p2x, int p2y);
        void Dispose();
    }
}

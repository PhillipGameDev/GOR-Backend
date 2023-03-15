using System.Threading.Tasks;
using ExitGames.Concurrency.Fibers;
using GameOfRevenge.Common.Models;
using GameOfRevenge.GameHandlers;
using GameOfRevenge.Model;

namespace GameOfRevenge.Interface
{
    public interface IWorld
    {
        IFiber WorldFiber { get; }
        int WorldId { get; }
        string Name { get; }
        IPlayersManager PlayersManager { get; }
        Region[][] WorldRegions { get; }
        BoundingBox Area { get; }
        Vector TileDimensions { get; }

        Region FindFreeRegion();
        Task<(MmoActor actor, IInterestArea iA)> GetPlayerPositionAsync(int playerId, PlayerInfo playerInfo);
        double GetDistanceBw2Points(Region p1, Region p2);
        void Dispose();
    }
}

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
        IFiber WorldFiber { get; }

//        Region FindFreeRegion();
        Task<(MmoActor actor, IInterestArea iA)> GetPlayerPositionAsync(int playerId, PlayerInfo playerInfo);
        double GetDistanceBw2Points(Region p1, Region p2);
        void Dispose();
    }
}

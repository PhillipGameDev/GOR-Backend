using GameOfRevenge.Common.Models;
using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.GameHandlers;

namespace GameOfRevenge.Buildings.Handlers
{
    public class BlackSmith : PlayerBuildingManager, IPlayerBuildingManager
    {
        public BlackSmith(PlayerInstance player, StructureDetails structureData, IGameBuildingManager baseBuildingManager) : base(player, structureData, baseBuildingManager)
        {
        }
    }
}

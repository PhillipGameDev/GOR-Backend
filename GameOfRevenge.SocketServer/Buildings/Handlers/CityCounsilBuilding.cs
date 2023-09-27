using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.Common.Models;
using GameOfRevenge.GameHandlers;

namespace GameOfRevenge.Buildings.Handlers
{
    public class CityCounsilBuilding : PlayerBuildingManager, IPlayerBuildingManager
    {
        public CityCounsilBuilding(PlayerInstance player, StructureDetails structureData, IGameBuildingManager baseBuildingManager) : base(player, structureData, baseBuildingManager)
        {
        }
    }
}

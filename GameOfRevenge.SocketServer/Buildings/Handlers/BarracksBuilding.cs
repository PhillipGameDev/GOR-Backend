using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.GameHandlers;
using GameOfRevenge.Common.Models;

namespace GameOfRevenge.Buildings.Handlers
{
    public class BarracksBuilding : PlayerBuildingManager, IPlayerBuildingManager
    {
        public BarracksBuilding(MmoActor player, StructureDetails structureData, IGameBuildingManager baseBuildingManager) : base(player, structureData, baseBuildingManager)
        {
        }
    }
}

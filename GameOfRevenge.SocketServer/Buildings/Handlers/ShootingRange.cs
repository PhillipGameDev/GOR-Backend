using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.Common.Models;
using GameOfRevenge.GameHandlers;

namespace GameOfRevenge.Buildings.Handlers
{
    public class ShootingRange : PlayerBuildingManager,IPlayerBuildingManager
    {
        public ShootingRange(PlayerInstance player, StructureDetails structureData, IGameBuildingManager baseBuildingManager) : base(player, structureData, baseBuildingManager)
        {
        }

    }

    public class TrainingBuilding
    {

    }
}

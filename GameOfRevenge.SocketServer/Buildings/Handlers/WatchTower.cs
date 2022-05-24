using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.GameHandlers;

namespace GameOfRevenge.Buildings.Handlers
{
    public class WatchTower : PlayerBuildingManager, IPlayerBuildingManager
    {
        public WatchTower(IGameBuildingManager baseBuildingManager, MmoActor player, UserStructureData structureData) : base(structureData, player, baseBuildingManager)
        {
            BaseBuilderManager = baseBuildingManager;
        }
    }
}

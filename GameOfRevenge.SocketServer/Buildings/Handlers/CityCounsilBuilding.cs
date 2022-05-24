using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.GameHandlers;


namespace GameOfRevenge.Buildings.Handlers
{
    public class CityCounsilBuilding : PlayerBuildingManager, IPlayerBuildingManager
    {
        public CityCounsilBuilding(IGameBuildingManager baseBuildingManager, MmoActor player, UserStructureData structureData) : base(structureData, player, baseBuildingManager)
        {
            BaseBuilderManager = baseBuildingManager;
        }
    }
}

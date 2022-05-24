using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.GameHandlers;
using GameOfRevenge.ResourcesHandler;

namespace GameOfRevenge.Buildings.Handlers
{
    public class Farm : ResourceGenerator, IPlayerBuildingManager
    {
        public override IPlayerResources Resource => Player.PlayerDataManager.PlayerResources[ResourceType.Food];
        public override double ProductionInTime { get => BaseStructureData.Data.FoodProduction; }

        public Farm(IGameBuildingManager baseBuildingManager, MmoActor player, UserStructureData structureData) : base(structureData, player, baseBuildingManager)
        {

        }
    }
}

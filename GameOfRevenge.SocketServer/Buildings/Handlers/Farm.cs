using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models;
using GameOfRevenge.GameHandlers;
using GameOfRevenge.ResourcesHandler;

namespace GameOfRevenge.Buildings.Handlers
{
    public class Farm : ResourceGenerator, IPlayerBuildingManager
    {
        public override IPlayerResources Resource
        {
            get
            {
                if ((Player.InternalPlayerDataManager == null) ||
                    (Player.InternalPlayerDataManager.PlayerResources == null) ||
                    (!Player.InternalPlayerDataManager.PlayerResources.ContainsKey(ResourceType.Food)))
                {
                    return null;
                }

                return Player.InternalPlayerDataManager.PlayerResources[ResourceType.Food];
            }
        }
        public override double ProductionInTime
        {
            get
            {
                if ((BaseStructureData == null) || (BaseStructureData.Data == null)) return 0;

                return BaseStructureData.Data.FoodProduction;
            }
        }

        public Farm(MmoActor player, StructureDetails structureData, IGameBuildingManager baseBuildingManager) : base(player, structureData, baseBuildingManager)
        {
        }
    }
}

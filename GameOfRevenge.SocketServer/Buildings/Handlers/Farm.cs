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
                var resp = 0;
                var structureData = BaseStructureData;
                if ((structureData != null) && (structureData.Data != null))
                {
                    resp = structureData.Data.FoodProduction;
                }

                return resp;
            }
        }

        public Farm(MmoActor player, StructureDetails structureData, IGameBuildingManager baseBuildingManager) : base(player, structureData, baseBuildingManager)
        {
        }
    }
}

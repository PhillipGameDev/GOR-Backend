using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models;
using GameOfRevenge.GameHandlers;
using GameOfRevenge.ResourcesHandler;

namespace GameOfRevenge.Buildings.Handlers
{
    public class Mine : ResourceGenerator, IPlayerBuildingManager
    {
        public override IPlayerResources Resource
        {
            get
            {
                if ((Player.InternalPlayerDataManager == null) ||
                    (Player.InternalPlayerDataManager.PlayerResources == null) ||
                    (!Player.InternalPlayerDataManager.PlayerResources.ContainsKey(ResourceType.Ore)))
                {
                    return null;
                }

                return Player.InternalPlayerDataManager.PlayerResources[ResourceType.Ore];
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
                    resp = structureData.Data.OreProduction;
                }

                return resp;
            }
        }

        public Mine(MmoActor player, StructureDetails structureData, IGameBuildingManager baseBuildingManager) : base(player, structureData, baseBuildingManager)
        {
        }
    }
}

using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models;
using GameOfRevenge.GameHandlers;
using GameOfRevenge.ResourcesHandler;

namespace GameOfRevenge.Buildings.Handlers
{
    public class SawMill : ResourceGenerator, IPlayerBuildingManager
    {
        public override IPlayerResources Resource
        {
            get
            {
                if ((Player.InternalPlayerDataManager == null) ||
                    (Player.InternalPlayerDataManager.PlayerResources == null) ||
                    (!Player.InternalPlayerDataManager.PlayerResources.ContainsKey(ResourceType.Wood)))
                {
                    return null;
                }

                return Player.InternalPlayerDataManager.PlayerResources[ResourceType.Wood];
            }
        }
        public override double ProductionInTime
        {
            get
            {
                if ((BaseStructureData == null) || (BaseStructureData.Data == null)) return 0;

                return BaseStructureData.Data.WoodProduction;
            }
        }

        public SawMill(MmoActor player, StructureDetails structureData, IGameBuildingManager baseBuildingManager) : base(player, structureData, baseBuildingManager)
        {
        }
    }
}

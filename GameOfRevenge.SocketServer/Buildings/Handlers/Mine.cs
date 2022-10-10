using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.GameHandlers;
using GameOfRevenge.ResourcesHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfRevenge.Buildings.Handlers
{
    public class Mine : ResourceGenerator, IPlayerBuildingManager
    {
        public override IPlayerResources Resource => this.Player.InternalPlayerDataManager.PlayerResources[ResourceType.Ore];
        public override double ProductionInTime { get => this.BaseStructureData.Data.OreProduction; }
        public Mine(IGameBuildingManager baseBuildingManager, MmoActor player, UserStructureData structureData) : base(structureData, player, baseBuildingManager)
        {
            this.BaseBuilderManager = baseBuildingManager;
        }
    }
}

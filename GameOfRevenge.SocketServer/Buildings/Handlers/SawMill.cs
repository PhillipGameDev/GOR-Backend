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
    public class SawMill : ResourceGenerator, IPlayerBuildingManager
    {
        public override IPlayerResources Resource => Player.InternalPlayerDataManager.PlayerResources[ResourceType.Wood];
        public override double ProductionInTime { get => BaseStructureData.Data.WoodProduction; }

        public SawMill(IGameBuildingManager baseBuildingManager, MmoActor player, UserStructureData structureData) : base(structureData, player, baseBuildingManager)
        {
            this.BaseBuilderManager = baseBuildingManager;
        }
    }
}

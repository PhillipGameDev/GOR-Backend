using System;
using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.Common.Models;
using GameOfRevenge.GameHandlers;
using GameOfRevenge.ResourcesHandler;

namespace GameOfRevenge.Buildings.Handlers
{
    public abstract class ResourceGenerator : PlayerBuildingManager
    {
        public double GenerateTime = GlobalConst.ResourceGenerateTime;
        public double ProductionPerSecond
        {
            get
            {
                return (this.ProductionInTime / GenerateTime);
            }
        }
        public abstract double ProductionInTime { get; }  // that production is hourly
        public abstract IPlayerResources Resource { get; }
        public BoostUpData BoostUp { get; set; }
        public ResourceGenerator(MmoActor player, StructureDetails structureData, IGameBuildingManager buildingManager) : base(player, structureData, buildingManager)
        {
            BoostUp = (structureData.Boost != null)? structureData.Boost : new BoostUpData();
        }
    }
}

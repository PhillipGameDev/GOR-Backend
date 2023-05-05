using System;
using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.Common.Models;
using GameOfRevenge.GameHandlers;
using GameOfRevenge.Model;
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
        public BoostUpResource BoostUp { get; set; }
        public ResourceGenerator(MmoActor player, StructureDetails structureData, IGameBuildingManager buildingManager) : base(player, structureData, buildingManager)
        {
            BoostUp = new BoostUpResource();
        }

        public void BoostResourceGenerationTime()//int boostTime)
        {
/*            if (BoostUp.TimeLeft <= 0)
            {
                BoostUp.StartTime = DateTime.UtcNow;
                BoostUp.Duration = 0;
            }
            BoostUp.Duration += boostTime;
            BoostUp.Percentage = 2;*/

            BoostUp.StartTime = DateTime.UtcNow;
            BoostUp.Duration = 5 * 60;
            BoostUp.Percentage = 10;

            log.InfoFormat("Boost Resource boostup= {0} ", Newtonsoft.Json.JsonConvert.SerializeObject(this.BoostUp));
        }
    }
}

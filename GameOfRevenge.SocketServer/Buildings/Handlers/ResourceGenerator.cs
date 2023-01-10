using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.GameApplication;
using GameOfRevenge.GameHandlers;
using GameOfRevenge.Model;
using GameOfRevenge.ResourcesHandler;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public ResourceGenerator(UserStructureData structureData, MmoActor player, IGameBuildingManager buildingManager) : base(structureData, player, buildingManager)
        {
            BoostUp = new BoostUpResource();
        }

        public void BoostResourceGenerationTime(int boostTime)
        {
            if (BoostUp.TimeLeft <= 0)
            {
                BoostUp.StartTime = DateTime.UtcNow;
                BoostUp.Duration = 0;
            }
            BoostUp.Duration += boostTime;
            BoostUp.Multiplier = 2;

            log.InfoFormat("Boost Resource boostup= {1} ", JsonConvert.SerializeObject(this.BoostUp));
        }
    }
}

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
        public List<BoostUpResources> BoostUpList { get; set; }
        public ResourceGenerator(UserStructureData structureData, MmoActor player, IGameBuildingManager buildingManager) : base(structureData, player, buildingManager)
        {
            this.BoostUpList = new List<BoostUpResources>();
        }
        public override void BoostResourceGenerationTime(ResourceBoostUpRequest request)
        {
            this.BoostUpList.Add(new BoostUpResources()
            {
                ForTime = request.BoostTime,
                Multiplier = request.Multiplier,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddSeconds(request.BoostTime)
            });
            log.InfoFormat("Boost Resource Request {0} List {1} ",
                JsonConvert.SerializeObject(request),
                JsonConvert.SerializeObject(this.BoostUpList));
            this.Player.SendOperation(OperationCode.BoostResourceTime, ReturnCode.OK);
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ExitGames.Logging;
using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.GameHandlers;
using GameOfRevenge.Model;
using GameOfRevenge.Troops;
using GameOfRevenge.Common.Interface.Model;

namespace GameOfRevenge.Buildings.Handlers
{
    public abstract class PlayerBuildingManager : IPlayerBuildingManager
    {
        protected static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public IGameBuildingManager BaseBuilderManager { get; set; }
        public MmoActor Player { get; private set; }
        public StructureDetails PlayerStructureData { get; set; }

        public IReadOnlyStructureDataRequirement BaseStructureData
        {
            get
            {
                return BaseBuilderManager.CacheBuildingData.GetStructureLevelById(CurrentLevel);
            }
        }
        public int CurrentLevel => PlayerStructureData.CurrentLevel;
        public int Location => PlayerStructureData.Location;
        public bool IsConstructing => (PlayerStructureData.TimeLeft > 0);
        public StructureType StructureType => BaseBuilderManager.CacheBuildingData.Info.Code;

        public Dictionary<TroopType, ITroop> Troops { get; set; } = new Dictionary<TroopType, ITroop>();

        public PlayerBuildingManager(MmoActor player, StructureDetails structureData, IGameBuildingManager buildingManager)
        {
            BaseBuilderManager = buildingManager;
            Player = player;
            PlayerStructureData = structureData;
            log.Info("data =>>> " + Newtonsoft.Json.JsonConvert.SerializeObject(structureData));
            try
            {
                log.Info("constructing = " + this.IsConstructing);
                if (this.IsConstructing) AddBuildingCallBack();
            }
            catch (System.Exception ex)
            {
                log.Info("EXCEPTION! " + ex.Message);
                throw ex;
            }
            log.InfoFormat("INIT player Structure info {0}", JsonConvert.SerializeObject(this.PlayerStructureData));
        }

        public void SetStructureData(StructureDetails structureData)
        {
            PlayerStructureData = structureData;
        }

        public void AddBuildingCallBack()
        {
            log.Info("player = " + (Player != null));
            log.Info("fiber = " + (Player.Fiber != null));
            log.Info("data = " + (PlayerStructureData != null));
//            Player.Fiber.Schedule(SendBuildingCompleteToBuild, PlayerStructureData.TimeLeft * 1000);
        }

        public bool HasAvailableRequirement(IReadOnlyDataRequirement values)
        {
            var reqLvl = values.Value;
            log.InfoFormat("check structure level is available structureData {0} reuirmentLevel {1} ",
                JsonConvert.SerializeObject(PlayerStructureData), reqLvl);
            return CurrentLevel >= reqLvl;
//            return (PlayerStructureData.Level >= reqLvl) && ((CurrentLevel == reqLvl && !IsConstructing) || CurrentLevel != reqLvl);
//            return PlayerStructureData.(d => d.Level >= reqLvl) && ((CurrentLevel == reqLvl && !IsConstructing) || CurrentLevel != reqLvl);
        }

        public void AddBuildingUpgrading(StructureDetails data)
        {
            log.InfoFormat("Upgrade Building CurrentBuilding {0} Data {1} ", StructureType.ToString(), JsonConvert.SerializeObject(data));
            this.PlayerStructureData = data;
//            if (IsConstructing) AddBuildingCallBack();
        }

        //public void SendBuildTimerToClient()
        //{
        //    var dict = new Dictionary<byte, object>();
        //    var response = new PlayerBuildingBuildingStatuResponse(ref dict);
        //    response.LocationId = this.Location;
        //    response.BuildTime = (int)this.StructureDetails.TimeLeft;
        //    response.TotalTime = BaseBuilderManager.BuildTime(this.CurrentLevel);
        //    //response.TotalTime = 60;
        //    log.InfoFormat("Send Building status to Client location {0} buildType {1} Time {2} ",
        //       response.LocationId, this.PlayerStructureData.ValueId.ToString(), this.StructureDetails.TimeLeft);
        //    response.SetDictionary<PlayerBuildingBuildingStatuResponse>(response);
        //    this.Player.SendOperation((byte)OperationCode.PlayerBuildingStatus, ReturnCode.OK, dict);
        //}

        private void SendBuildingCompleteToBuild()
        {
            var timer = new TimerCompleteResponse
            {
                LocationId = Location,
                Level = CurrentLevel,
                StructureType = (int)StructureType
            };
            Player.SendEvent(EventCode.CompleteTimer, timer);
        }

        public void AddOrUpdateTroop(ITroop troop)
        {
            if (Troops.ContainsKey(troop.TroopType))
                Troops[troop.TroopType] = troop;
            else
                Troops.Add(troop.TroopType, troop);
        }

        public ITroop IsAnyTroopInTraining()
        {
            var troop = Troops.FirstOrDefault(d => d.Value.IsTraining);
            return (troop.Value != null) ? troop.Value : null;
        }

        #region VirtualMethods
/*        public virtual void BoostResourceGenerationTime(ResourceBoostUpRequest request)
        {
            var msg = "Invalid StructureType.";
            Player.SendOperation(OperationCode.BoostResourceTime, ReturnCode.InvalidOperation, debuMsg: msg);
        }*/
        public virtual void HandleWoundedTroops(WoundedTroopHealRequest request) { }
        public virtual void WoundedTroopTimerStatusRequest(WoundedTroopTimerStatusRequest request) { }
        public virtual void HandleUpgradeTechnology(UpgradeTechnologyRequest operation) { }

        public virtual void RepairGate(GateRequest operation) { }
        public virtual void GateHp(GateRequest operation) { }
        #endregion
    }
}

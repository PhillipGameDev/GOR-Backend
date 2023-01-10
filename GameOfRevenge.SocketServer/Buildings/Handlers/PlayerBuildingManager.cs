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
        public IReadOnlyStructureDataRequirement BaseStructureData
        {
            get
            {
                return BaseBuilderManager.CacheBuildingData.GetStructureLevelById(CurrentLevel);
            }
        }
        public MmoActor Player { get; private set; }


        public UserStructureData PlayerStructureData { get; set; }
        public StructureType StructureType => PlayerStructureData.ValueId;

        public StructureDetails StructureDetails => PlayerStructureData.Value[0];

        public int CurrentLevel => StructureDetails.Level;
        public int Location => StructureDetails.Location;

//        public int StructureId => PlayerStructureData.StructureId;

        public bool IsConstructing => (StructureDetails.TimeLeft > 0);

        public Dictionary<TroopType, ITroop> Troops { get; set; }

        public PlayerBuildingManager(UserStructureData structureData, MmoActor player, IGameBuildingManager buildingManager)
        {
            Troops = new Dictionary<TroopType, ITroop>();
            PlayerStructureData = structureData;
            BaseBuilderManager = buildingManager;
            Player = player;
            if (IsConstructing) AddBuildingCallBack();
            log.InfoFormat("INIT player Structure info {0}", JsonConvert.SerializeObject(this.PlayerStructureData));
        }

        public void SetStructureData(UserStructureData structureData)
        {
            this.PlayerStructureData = structureData;
        }

        public void AddBuildingCallBack()
        {
            Player.Fiber.Schedule(() => { SendBuildingCompleteToBuild(); }, (long)(1000 * StructureDetails.TimeLeft));
        }

        public bool HasAvailableRequirement(IReadOnlyDataRequirement values)
        {
            log.InfoFormat("check structure level is available structureData {0} reuirmentLevel {1} ",
                JsonConvert.SerializeObject(PlayerStructureData.Value), values.Value);
            return PlayerStructureData.Value.Any(d => d.Level >= values.Value) && ((CurrentLevel == values.Value && !IsConstructing) || CurrentLevel != values.Value);
        }

        public void AddBuildingUpgrading(UserStructureData data)
        {
            log.InfoFormat("Upgrade Building CurrentBuilding {0} Data {1} ", this.PlayerStructureData.DataType.ToString(), JsonConvert.SerializeObject(data));
            this.PlayerStructureData = data;
            if (IsConstructing) this.AddBuildingCallBack();
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
                LocationId = this.Location,
                Level = this.CurrentLevel,
                StructureType = (int)this.PlayerStructureData.ValueId
            };
            this.Player.SendEvent(EventCode.CompleteTimer, timer);
        }

        public void AddOrUpdateTroop(ITroop troop)
        {
            if (this.Troops.ContainsKey(troop.TroopType))
                this.Troops[troop.TroopType] = troop;
            else
                this.Troops.Add(troop.TroopType, troop);
        }

        public ITroop IsAnyTroopInTraining()
        {
            var troop = Troops.Where(d => d.Value.IsTraining).FirstOrDefault();
            if (troop.Value != null) return troop.Value;
            else return null;
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

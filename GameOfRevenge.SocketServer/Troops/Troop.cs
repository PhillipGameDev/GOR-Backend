using System;
using Newtonsoft.Json;
using ExitGames.Logging;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.Model;
using GameOfRevenge.GameHandlers;
using GameOfRevenge.GameApplication;

namespace GameOfRevenge.Troops
{
    public class Troop : ITroop
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        public readonly IPlayerBuildingManager Building;

        public TroopDetails TroopDetails { get; private set; }
        public UnavaliableTroopInfo TroopTrainer { get; private set; }
        public PlayerInstance Player { get => Building.Player; }
        public IGameTroop GameTroop { get; private set; }
        public TroopType TroopType => GameTroop.TroopType;
        public bool IsTraining { get => (TroopTrainer != null) && (TroopTrainer.TimeLeft > 0); }
        public int TroopLevel { get; private set; }

//        public DateTime StartTime { get; set; }
//        public DateTime EndTime { get => TroopTrainer.EndTime; }
//        public double TimeLeft { get => TroopTrainer.TimeLeft; }

        public Troop(IGameTroop gameTroop, IPlayerBuildingManager building) : this(gameTroop, building, 1) { }
        public Troop(IGameTroop gameTroop, IPlayerBuildingManager building, int level)
        {
            GameTroop = gameTroop;
            Building = building;
            TroopLevel = level;
        }

        public void Init(UserTroopData troopData)
        {
            SetTroopData(troopData);
//            AddBuildingCallBack();
        }

        private void SetTroopData(UserTroopData data)
        {
#if DEBUG
            log.Info($"Set Troop Data {JsonConvert.SerializeObject(data)}");
#endif

            if ((data != null) && (data.Value != null))
            {
                TroopDetails = data.Value.Find(x => (x.Level == TroopLevel));
                if ((TroopDetails != null) && (TroopDetails.InTraning != null))
                {
                    TroopTrainer = TroopDetails.InTraning.Find(x => (x.BuildingLocId == Building.Location));
                }
#if DEBUG
                try
                {
                    log.Info($"Set Troop Data 2 {JsonConvert.SerializeObject(data)}  TroopDetails {JsonConvert.SerializeObject(TroopDetails)} TroopTrainer {JsonConvert.SerializeObject(TroopTrainer)}");
                }
                catch (Exception ex)
                {
                    log.Error("Exception SetTroopData", ex);
                }
#endif
            }
        }

        public void TrainingStart(RecruitTroopRequest request)
        {
            try
            {
                var troop = GameService.BUsertroopManager.TrainTroops(Player.PlayerId, TroopType, request.TroopLevel, request.TroopCount, Building.Location).Result.Data;
                SetTroopData(troop);
//                AddBuildingCallBack();
                log.Info($"TrainingStart playerId {Player.PlayerId} troopType {TroopType} CurrentLevel {TroopLevel} troopCount {request.TroopCount} location {Building.Location}");
            }
            catch (Exception ex)
            {
                log.Error("Exception in TrainingStart", ex);
            }
        }

/*        public void AddBuildingCallBack()
        {
            if (IsTraining && (Player.Fiber != null))
            {
                Player.Fiber.Schedule(SendTrainingCompleteToTroop, (long)(1000 * TroopTrainer.TimeLeft));
            }
        }

        public void SendTrainingCompleteToTroop()
        {

        }*/

        public void SendTroopTrainingTimeToClient()
        {
            if (IsTraining)
            {
                //var response = new TroopTrainingTimeResponse()
                //{
                //    LocationId = Building.Location,
                //    StructureType = (int)Building.StructureType,
                //    TrainingTime = TimeLeft,
                //    // response.TotalTime = this.TroopTrainer.Count * GameTroop.GetTrainingTotalTime(this.TroopLevel);
                //    TotalTime = 60
                //};

                ////response.SetDictionary(response);
                //log.Info($"Request Of Troop Training time structureType {response.StructureType} locationId {response.LocationId} trainingTime {response.TrainingTime} totalTime { response.TotalTime}");
                //Player.SendOperation((byte)OperationCode.RecruitTroopStatus, ReturnCode.OK, dict);
            }
            else
                Player.SendOperation((byte)OperationCode.RecruitTroopStatus, ReturnCode.Failed);
        }

        public void TrainingTimeBoostUp()
        {
            if (IsTraining)
            {
                double reduceTime = TroopTrainer.TimeLeft / 2;
                log.Info($"Time Left for the training LeftTime {TroopTrainer.TimeLeft}");
                // if (reduceTime > 0)
                //  this.EndTime = this.EndTime.AddSeconds(-reduceTime);
                log.Info($"Time Left now when boost up apply {TroopTrainer.TimeLeft} ReduceTime {reduceTime}");
            }
            Player.SendOperation((byte)OperationCode.TroopTrainerTimeBoost, ReturnCode.OK);
        }
    }
}

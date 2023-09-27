using System.Linq;
using GameOfRevenge.Model;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models.Troop;
using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.GameHandlers;
using ExitGames.Logging;
using Newtonsoft.Json;

namespace GameOfRevenge.Troops
{
    public class GameTroops : IGameTroop
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public IReadOnlyTroopDataRequirementRel TroopData { get; set; }
        public TroopType TroopType => TroopData.Info.Code;

        public GameTroops(IReadOnlyTroopDataRequirementRel troopData)
        {
            TroopData = troopData;
        }

        public IReadOnlyTroopDataRequirements GetTroopLevel(int level)
        {
            return TroopData.Levels.FirstOrDefault(d => (d.Data.Level == level));
        }

        public double GetTrainingTotalTime(int level)
        {
            var troop = GetTroopLevel(level);
            if (troop != null) return troop.Data.TraningTime;
            else return GlobalConst.DefaultTroopTrainingTime;
        }

        public void TroopTraining(RecruitTroopRequest request, PlayerInstance actor)
        {
            var building = actor.InternalPlayerDataManager.GetPlayerBuilding(request.StructureType, request.LocationId);
            if (building == null)
            {
                actor.Peer.SendOperation(OperationCode.RecruitTroopRequest, ReturnCode.Failed, null, "Player building not found.");
                return;
            }

            var troopLevel = GetTroopLevel(request.TroopLevel);
            if (troopLevel == null)
            {
                actor.Peer.SendOperation(OperationCode.RecruitTroopRequest, ReturnCode.Failed, null, "Troop data not found of current level.");
                return;
            }

            var (succ, msg) = actor.InternalPlayerDataManager.CheckRequirementsAndUpdateValues(troopLevel.Requirements);
            if (!succ)
                actor.Peer.SendOperation(OperationCode.RecruitTroopRequest, ReturnCode.Failed, null, msg);
            else
            {
                var troop = AddTroopOnPlayerBuilding((TroopType)request.TroopType, building, actor);
                if (troop != null)
                {
                    troop.TrainingStart(request);
                    actor.Peer.SendOperation(OperationCode.RecruitTroopRequest, ReturnCode.OK);
                }
            }
        }

        public ITroop AddTroopOnPlayerBuilding(TroopType troopType, IPlayerBuildingManager building, PlayerInstance actor)
        {
            log.InfoFormat("AddTroopOnPlayerBuilding {0} bld= {1} ", troopType.ToString(), JsonConvert.SerializeObject(building));
            TroopFactory troopFactory = new ConcreteTroopFactory();

            ITroop troop;
            if (building.Troops.ContainsKey(troopType))
            {
                troop = building.Troops[troopType];
                if (troop.IsTraining)
                {
                    log.Info("troop already training");
                    actor.Peer.SendOperation(OperationCode.RecruitTroopRequest, ReturnCode.Failed, null, "Troop is already in the training.");
                    return null;
                }
            }
            else
            {
                troop = troopFactory.GetPlayerTroop(this, building);
            }

            building.AddOrUpdateTroop(troop);
            return troop;
        }
    }
}

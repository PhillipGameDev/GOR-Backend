using System.Linq;
using GameOfRevenge.Model;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models.Troop;
using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.GameHandlers;

namespace GameOfRevenge.Troops
{
    public class GameTroops : IGameTroop
    {
        public IReadOnlyTroopDataRequirementRel TroopData { get; set; }
        public TroopType TroopType => TroopData.Info.Code;

        public GameTroops(IReadOnlyTroopDataRequirementRel troopData)
        {
            TroopData = troopData;
        }

        public IReadOnlyTroopDataRequirements GetTroopLevel(int level)
        {
            return TroopData.Levels.Where(d => d.Data.Level == level).FirstOrDefault();
        }

        public double GetTrainingTotalTime(int level)
        {
            var troop = GetTroopLevel(level);
            if (troop != null) return troop.Data.TraningTime;
            else return GlobalConst.DefaultTroopTrainingTime;
        }

        public void TroopTraining(RecruitTroopRequest request, MmoActor actor)
        {
            var building = actor.InternalPlayerDataManager.GetPlayerBuilding(request.StructureType, request.LocationId);
            if (building == null) actor.Peer.SendOperation(OperationCode.RecruitTroopRequest, ReturnCode.Failed, null, "Player building not found.");
            else
            {
                var troopLevel = GetTroopLevel(request.TroopLevel);
                if (troopLevel == null) actor.Peer.SendOperation(OperationCode.RecruitTroopRequest, ReturnCode.Failed, null, "Troop data not found of current level.");
                else
                {
                    var (succ, msg) = actor.InternalPlayerDataManager.CheckRequirementsAndUpdateValues(troopLevel.Requirements);
                    if (!succ) actor.Peer.SendOperation(OperationCode.RecruitTroopRequest, ReturnCode.Failed, null, msg);
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
            }
        }

        public ITroop AddTroopOnPlayerBuilding(TroopType troopType, IPlayerBuildingManager building, MmoActor actor)
        {
            TroopFactory troopFactory = new ConcreteTroopFactory();

            ITroop troop;
            if (building.Troops.ContainsKey(troopType))
            {
                troop = building.Troops[troopType];
                if (troop.IsTraining)
                {
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

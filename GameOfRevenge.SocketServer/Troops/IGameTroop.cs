using GameOfRevenge.Common;
using GameOfRevenge.Common.Models.Troop;
using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.GameHandlers;
using GameOfRevenge.Model;

namespace GameOfRevenge.Troops
{
   public interface IGameTroop
    {
        IReadOnlyTroopDataRequirementRel TroopData { get; }
        TroopType TroopType { get; }

        double GetTrainingTotalTime(int level);
        void TroopTraining(RecruitTroopRequest request, PlayerInstance actor);
        ITroop AddTroopOnPlayerBuilding(TroopType troopType, IPlayerBuildingManager building, PlayerInstance actor);
    }
}

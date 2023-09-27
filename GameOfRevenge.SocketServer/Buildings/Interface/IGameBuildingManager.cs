using System.Collections.Generic;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.GameHandlers;
using GameOfRevenge.Model;
using GameOfRevenge.Troops;

namespace GameOfRevenge.Buildings.Interface
{
    public interface IGameBuildingManager
    {
        Dictionary<TroopType, IGameTroop> Troops { get; }
        IReadOnlyStructureDataRequirementRel CacheBuildingData { get; }

        int BuildTime(int level);
        void CreateStructureForPlayer(CreateStructureRequest request, PlayerInstance actor);
        bool UpgradeStructureForPlayer(UpgradeStructureRequest request, PlayerInstance actor);
        void RecruitTroops(RecruitTroopRequest request, PlayerInstance actor);
    }
}

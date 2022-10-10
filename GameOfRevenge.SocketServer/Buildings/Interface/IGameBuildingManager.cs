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
        void CreateStructureForPlayer(CreateStructureRequest request, MmoActor actor);
        bool UpgradeStructureForPlayer(UpgradeStructureRequest request, MmoActor actor);
        void RecruitTroops(RecruitTroopRequest request, MmoActor actor);
    }
}

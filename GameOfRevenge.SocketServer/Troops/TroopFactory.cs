using GameOfRevenge.Common.Models.Troop;
using GameOfRevenge.Buildings.Interface;

namespace GameOfRevenge.Troops
{
    public abstract class TroopFactory
    {
        public abstract IGameTroop GetAllGameTroops(IReadOnlyTroopDataRequirementRel troopData);
        public abstract ITroop GetPlayerTroop(IGameTroop troop, IPlayerBuildingManager building);
    }
}

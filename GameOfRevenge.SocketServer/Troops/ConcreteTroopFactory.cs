using System;
using GameOfRevenge.Common;
using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.Common.Models.Troop;

namespace GameOfRevenge.Troops
{
    public class ConcreteTroopFactory : TroopFactory
    {
        public override IGameTroop GetAllGameTroops(IReadOnlyTroopDataRequirementRel troopData)
        {
            return new GameTroops(troopData);
        }

        public override ITroop GetPlayerTroop(IGameTroop troop,IPlayerBuildingManager building)
        {
            switch (troop.TroopType)
            {
                case TroopType.Swordsman:
                    return new Swordsman(troop,building);
                case TroopType.Archer:
                    return new Archer(troop,building);
                case TroopType.Knight:
                    return new Knight(troop,building);
                case TroopType.Slingshot:
                    return new Slingshot(troop,building);
                default:
                    throw new ApplicationException($"Troop is no created {troop.TroopData.Info.Code}");
            }
        }
    }
}

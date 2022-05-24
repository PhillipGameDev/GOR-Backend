using GameOfRevenge.Buildings.Interface;

namespace GameOfRevenge.Troops
{
    public class Archer : Troop
    {
        public Archer(IGameTroop troop, IPlayerBuildingManager Building) : base(troop, Building)
        {

        }
    }
}

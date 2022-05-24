using GameOfRevenge.Buildings.Interface;

namespace GameOfRevenge.Troops
{
    public class Swordsmen : Troop
    {
        public Swordsmen(IGameTroop troop, IPlayerBuildingManager Building) : base(troop, Building)
        {

        }
    }
}

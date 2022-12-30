using GameOfRevenge.Buildings.Interface;

namespace GameOfRevenge.Troops
{
    public class Swordsman : Troop
    {
        public Swordsman(IGameTroop troop, IPlayerBuildingManager Building) : base(troop, Building)
        {

        }
    }
}

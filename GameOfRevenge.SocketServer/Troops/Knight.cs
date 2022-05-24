using GameOfRevenge.Buildings.Interface;

namespace GameOfRevenge.Troops
{
    public class Knight : Troop
    {
        public Knight(IGameTroop troop, IPlayerBuildingManager Building) : base(troop, Building)
        {

        }
    }
}

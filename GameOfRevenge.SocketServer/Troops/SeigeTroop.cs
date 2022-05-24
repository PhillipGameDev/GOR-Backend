using GameOfRevenge.Buildings.Interface;

namespace GameOfRevenge.Troops
{
    public class SeigeTroop : Troop
    {
        public SeigeTroop(IGameTroop troop, IPlayerBuildingManager Building) : base(troop, Building)
        {

        }
    }
}

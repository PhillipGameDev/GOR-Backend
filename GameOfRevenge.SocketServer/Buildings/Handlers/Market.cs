using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.Common.Models;
using GameOfRevenge.GameHandlers;

namespace GameOfRevenge.Buildings.Handlers
{
    public class Market : PlayerBuildingManager,IPlayerBuildingManager
    {
        public Market(PlayerInstance player, StructureDetails structureData, IGameBuildingManager baseBuildingManager) : base(player, structureData, baseBuildingManager)
        {
        }
    }
}

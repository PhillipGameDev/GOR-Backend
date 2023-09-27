using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.Common.Models;
using GameOfRevenge.GameHandlers;

namespace GameOfRevenge.Buildings.Handlers
{
    public class InfantryCamp : PlayerBuildingManager,IPlayerBuildingManager
    {
        public InfantryCamp(PlayerInstance player, StructureDetails structureData, IGameBuildingManager baseBuildingManager) : base(player, structureData, baseBuildingManager)
        {
        }
    }
}

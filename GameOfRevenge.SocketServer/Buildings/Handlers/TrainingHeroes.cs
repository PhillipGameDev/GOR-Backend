using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.GameHandlers;

namespace GameOfRevenge.Buildings.Handlers
{
    public class TrainingHeroes : PlayerBuildingManager
    {
        public TrainingHeroes(IGameBuildingManager baseBuildingManager, MmoActor player, UserStructureData structureData) : base(structureData, player, baseBuildingManager)
        {

        }
    }
}

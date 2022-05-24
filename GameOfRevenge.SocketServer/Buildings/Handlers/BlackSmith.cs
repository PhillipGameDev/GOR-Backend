using System;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.GameHandlers;

namespace GameOfRevenge.Buildings.Handlers
{
    public class BlackSmith : PlayerBuildingManager, IPlayerBuildingManager
    {
        public BlackSmith(IGameBuildingManager baseBuildingManager, MmoActor player, UserStructureData structureData) : base(structureData, player, baseBuildingManager)
        {
            BaseBuilderManager = baseBuildingManager;
        }
    }
}

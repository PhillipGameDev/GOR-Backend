using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.GameHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfRevenge.Buildings.Handlers
{
    public class ShootingRange : PlayerBuildingManager,IPlayerBuildingManager
    {
        public ShootingRange(IGameBuildingManager baseBuildingManager, MmoActor player, UserStructureData structureData) : base(structureData, player, baseBuildingManager)
        {
            //this.BaseBuilderManager = baseBuildingManager;
        }

    }

    public class TrainingBuilding
    {

    }
}

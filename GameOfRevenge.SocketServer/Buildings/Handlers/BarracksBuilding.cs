using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.GameHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameOfRevenge.Common.Models.PlayerData;

namespace GameOfRevenge.Buildings.Handlers
{
    public class BarracksBuilding : PlayerBuildingManager, IPlayerBuildingManager
    {
        public BarracksBuilding(IGameBuildingManager baseBuildingManager, MmoActor player, UserStructureData structureData) : base(structureData, player, baseBuildingManager)
        {
        }
    }
}

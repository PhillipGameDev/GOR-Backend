using System.Collections.Generic;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.ResourcesHandler;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Interface.Model;

namespace GameOfRevenge.Interface
{
    public interface IPlayerSocketDataManager
    {
        Dictionary<StructureType, List<IPlayerBuildingManager>> PlayerBuildings { get; }
        Dictionary<ResourceType, IPlayerResources> PlayerResources { get; }
        IPlayerAttackHandler AttackHandler { get; }
        UserKingDetails King { get; }

        void AddStructure(int locationId, UserStructureData structure, IGameBuildingManager gameBuilding);
        (bool succ, string msg) CheckRequirmentsAndUpdateValues(IReadOnlyList<IReadOnlyDataRequirement> requirments);
        IPlayerBuildingManager GetPlayerBuilding(StructureType structType, int locationId);
        IPlayerBuildingManager GetPlayerBuilding(int structType, int locationId);
        IPlayerBuildingManager GetPlayerBuildingByLocationId(int locationId);
        void Dispose();
    }
}

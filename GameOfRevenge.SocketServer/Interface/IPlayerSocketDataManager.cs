using System.Collections.Generic;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.ResourcesHandler;
using GameOfRevenge.Common.Models.Structure;

namespace GameOfRevenge.Interface
{
    public interface IPlayerSocketDataManager
    {
        Dictionary<StructureType, List<IPlayerBuildingManager>> PlayerBuildings { get; }
        Dictionary<ResourceType, IPlayerResources> PlayerResources { get; }
        IPlayerAttackHandler AttackHandler { get; }
//        UserKingDetails King { get; }
//        List<UserRecordBuilderDetails> Builders { get; }

        void UpdateData(PlayerCompleteData data);

        void AddStructureOnPlayer(UserStructureData data);
        void AddStructure(int locationId, StructureType structureType, StructureDetails structure, IGameBuildingManager gameBuilding);
        (bool succ, string msg) CheckRequirementsAndUpdateValues(IReadOnlyList<IReadOnlyDataRequirement> requirments);
        IPlayerBuildingManager GetPlayerBuilding(StructureType structType, int location);
        IPlayerBuildingManager GetPlayerBuilding(int structType, int location);
        IPlayerBuildingManager GetPlayerBuildingByLocationId(int location);
        void Dispose();
    }
}

using System.Collections.Generic;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Interface.Model;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.GameHandlers;
using GameOfRevenge.Model;
using GameOfRevenge.Troops;

namespace GameOfRevenge.Buildings.Interface
{
    public interface IPlayerBuildingManager 
    {
        MmoActor Player { get; }
        UserStructureData PlayerStructureData { get; }

        int Location { get; }
//        int StructureId { get; }
        int CurrentLevel { get; }
        bool IsConstructing { get; }
        StructureType StructureType { get; }
        Dictionary<TroopType, ITroop> Troops {get;}

        void SetStructureData(UserStructureData structureData);
        void AddOrUpdateTroop(ITroop troop);
        void AddBuildingUpgrading(UserStructureData data);
        bool HasAvailableRequirement(IReadOnlyDataRequirement values);
       // void SendBuildTimerToClient();
        void BoostResourceGenerationTime(ResourceBoostUpRequest request);
        ITroop IsAnyTroopInTraining();
        void HandleWoundedTroops(WoundedTroopHealRequest request);
        void WoundedTroopTimerStatusRequest(WoundedTroopTimerStatusRequest request);
        void HandleUpgradeTechnology(UpgradeTechnologyRequest operation);
        void RepairGate(GateRequest operation);
        void GateHp(GateRequest operation);
    }
}

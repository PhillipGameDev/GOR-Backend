using System.Collections.Generic;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Interface.Model;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.GameHandlers;
using GameOfRevenge.Model;
using GameOfRevenge.Troops;
using Newtonsoft.Json;

namespace GameOfRevenge.Buildings.Interface
{
    public interface IPlayerBuildingManager 
    {
        [JsonIgnore]
        PlayerInstance Player { get; }
        //TODO: we should remove userstructuredata, this implementation store all the user
        //structure data of this type. We should just store the data for this building
        //the core base of this class was build to support multiple buildings, but we
        //should remove it to improve the process
        StructureDetails PlayerStructureData { get; }

        int Location { get; }
//        int StructureId { get; }
        int CurrentLevel { get; }
        bool IsConstructing { get; }
        StructureType StructureType { get; }
        Dictionary<TroopType, ITroop> Troops {get;}

        void SetStructureData(StructureDetails structureData);
        void AddOrUpdateTroop(ITroop troop);
        void AddBuildingUpgrading(StructureDetails data);
        bool HasAvailableRequirement(IReadOnlyDataRequirement values);
       // void SendBuildTimerToClient();

//        void BoostResourceGenerationTime(ResourceBoostUpRequest request);

        ITroop IsAnyTroopInTraining();
        void HandleWoundedTroops(WoundedTroopHealRequest request);
        void HandleInstantWoundedTroops(WoundedTroopHealRequest request);
        void WoundedTroopTimerStatusRequest(WoundedTroopTimerStatusRequest request);
        void HandleUpgradeTechnology(UpgradeTechnologyRequest operation);
        void RepairGate(GateRequest operation);
        void GateHp(GateRequest operation);
    }
}

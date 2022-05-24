using System;
using System.Collections.Generic;
using System.Linq;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.Model;
using GameOfRevenge.GameApplication;
using GameOfRevenge.GameHandlers;
using GameOfRevenge.Common;
using GameOfRevenge.Business.CacheData;

namespace GameOfRevenge.Buildings.Handlers
{
    public class Gate : PlayerBuildingManager, IPlayerBuildingManager
    {
        public Gate(IGameBuildingManager baseBuildingManager, MmoActor player, UserStructureData structureData) : base(structureData, player, baseBuildingManager)
        {
            BaseBuilderManager = baseBuildingManager;
        }

        public override void GateHp(GateRequest operation)
        {
            try
            {
                var playerData = PlayerData();

                if (!playerData.IsSuccess) throw new DataNotExistExecption(playerData.Message);

                var gateData = playerData.Data?.Structures?.Where(x => x.StructureType == StructureType)?.FirstOrDefault()?.Buildings;
                if (gateData == null || gateData.Count == 0) throw new DataNotExistExecption("Structure does not exists");

                var currentGateData = gateData.FirstOrDefault();

                var dict = new Dictionary<byte, object>();
                var newValues = new GateResponse(ref dict)
                {
                    BuildingLocationId = operation.BuildingLocationId,
                    Hitpoint = currentGateData.HitPoints
                };

                Player.SendOperation(OperationCode.GateHp, ReturnCode.OK, dict, playerData.Message);
                log.Debug("****************************************GateHp Success**********************************************");
            }
            catch (DataNotExistExecption ex)
            {
                Player.SendOperation(OperationCode.GateHp, ReturnCode.Failed, null, ex.Message);
                log.Debug("****************************************GateHp Invalid**********************************************");
            }
            catch (Exception ex)
            {
                log.Error("****************************************GateHp ERROR**********************************************", ex);
            }
        }

        private Response<PlayerCompleteData> PlayerData()
        {
            var task = GameService.BPlayerStructureManager.GetPlayerData(Player.PlayerId);
            task.Wait();
            var result = task.Result;
            return result;
        }

        public override void RepairGate(GateRequest operation)
        {
            try
            {
                var task = GameService.BPlayerStructureManager.RepairGate(Player.PlayerId);
                task.Wait();
                if (!task.Result.IsSuccess) throw new DataNotExistExecption(task.Result.Message);

                var playerData = PlayerData();
                if (!playerData.IsSuccess) throw new DataNotExistExecption(playerData.Message);

                var gateData = playerData.Data?.Structures?.Where(x => x.StructureType == StructureType)?.FirstOrDefault()?.Buildings;
                if (gateData == null || gateData.Count == 0) throw new DataNotExistExecption("Structure does not exists");

                var currentGateData = gateData.FirstOrDefault();
                var structureData = CacheStructureDataManager.GetFullStructureData(StructureType)?.Levels.Where(x => x.Data.Level == currentGateData.Level).FirstOrDefault();

                var dict = new Dictionary<byte, object>();
                var newValues = new GateResponse(ref dict)
                {
                    BuildingLocationId = operation.BuildingLocationId,
                    Hitpoint = currentGateData.HitPoints
                };

                Player.SendOperation(OperationCode.GateHp, ReturnCode.OK, dict, playerData.Message);
                log.Debug("****************************************RepairGate Success**********************************************");
            }
            catch(DataNotExistExecption ex)
            {
                Player.SendOperation(OperationCode.GateHp, ReturnCode.Failed, null, ex.Message);
                log.Debug("****************************************RepairGate Invalid**********************************************");
            }
            catch (Exception ex)
            {
                log.Error("****************************************RepairGate ERROR**********************************************", ex);
            }
        }
    }
}

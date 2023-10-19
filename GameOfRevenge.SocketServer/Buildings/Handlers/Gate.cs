using System;
using System.Collections.Generic;
using System.Linq;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.Model;
using GameOfRevenge.GameApplication;
using GameOfRevenge.GameHandlers;
using GameOfRevenge.Common;
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Business;

namespace GameOfRevenge.Buildings.Handlers
{
    public class Gate : PlayerBuildingManager, IPlayerBuildingManager
    {
        public Gate(PlayerInstance player, StructureDetails structureData, IGameBuildingManager baseBuildingManager) : base(player, structureData, baseBuildingManager)
        {
        }

        public override void GateHp(GateRequest operation)
        {
            try
            {
                var playerData = PlayerData();

                if (!playerData.IsSuccess) throw new DataNotExistExecption(playerData.Message);

                List<StructureDetails> gateData = null;
                if (playerData.Data != null)
                {
                    gateData = playerData.Data.Structures.Find(x => (x.StructureType == StructureType))?.Buildings;
                }
                if (gateData == null || gateData.Count == 0) throw new DataNotExistExecption("Structure does not exists");

                var dict = new Dictionary<byte, object>();
                var newValues = new GateResponse(ref dict)
                {
                    BuildingLocationId = operation.BuildingLocationId,
                    Hitpoints = gateData[0].HitPoints
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
            var task = BaseUserDataManager.GetFullPlayerData(Player.PlayerId);
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

                List<StructureDetails> gateData = null;
                if (playerData.Data != null)
                {
                    gateData = playerData.Data.Structures.Find(x => (x.StructureType == StructureType))?.Buildings;
                }
                if (gateData == null || gateData.Count == 0) throw new DataNotExistExecption("Structure does not exists");

                //TODO: not implemented yet?
                var structureData = CacheStructureDataManager.GetFullStructureData(StructureType).Levels.FirstOrDefault(x => (x.Data.Level == gateData[0].Level));

                var dict = new Dictionary<byte, object>();
                var newValues = new GateResponse(ref dict)
                {
                    BuildingLocationId = operation.BuildingLocationId,
                    Hitpoints = gateData[0].HitPoints
                };

                Player.SendOperation(OperationCode.RepairGate, ReturnCode.OK, dict, playerData.Message);
                log.Debug("****************************************RepairGate Success**********************************************");
            }
            catch(DataNotExistExecption ex)
            {
                Player.SendOperation(OperationCode.RepairGate, ReturnCode.Failed, null, ex.Message);
                log.Debug("****************************************RepairGate Invalid**********************************************");
            }
            catch (Exception ex)
            {
                log.Error("****************************************RepairGate ERROR**********************************************", ex);
            }
        }
    }
}

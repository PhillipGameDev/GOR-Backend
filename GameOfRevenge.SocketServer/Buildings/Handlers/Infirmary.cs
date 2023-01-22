using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Common.Models.Troop;
using GameOfRevenge.GameApplication;
using GameOfRevenge.GameHandlers;
using GameOfRevenge.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameOfRevenge.Buildings.Handlers
{
    public class Infirmary : PlayerBuildingManager, IPlayerBuildingManager
    {
        public List<UnavaliableTroopInfo> RecoverList = new List<UnavaliableTroopInfo>();

        public Infirmary(IGameBuildingManager baseBuildingManager, MmoActor player, UserStructureData structureData) : base(structureData, player, baseBuildingManager)
        {
            BaseBuilderManager = baseBuildingManager;
        }

        public override void HandleWoundedTroops(WoundedTroopHealRequest request)
        {
            bool suc = true; string message = "OK";
            try
            {
                Dictionary<TroopType, List<WoundeAndDeadTroopsUpdate>> dict = new Dictionary<TroopType, List<WoundeAndDeadTroopsUpdate>>();
                for (int j = 0; j < request.TroopLevel.Length; j++)
                {
                    List<WoundeAndDeadTroopsUpdate> troopList = null;
                    if (dict.ContainsKey((TroopType)request.TroopType[j]))
                    {
                        troopList = dict[(TroopType)request.TroopType[j]];
                    }
                    else
                    {
                        troopList = new List<WoundeAndDeadTroopsUpdate>();
                        dict.Add((TroopType)request.TroopType[j], troopList);
                    }
                    WoundeAndDeadTroopsUpdate obj = troopList.Find(d => (d.Level == request.TroopLevel[j]));
                    if (obj == null)
                    {
                        obj = new WoundeAndDeadTroopsUpdate();
                        troopList.Add(obj);
                    }
                    obj.BuildingLocation = request.BuildingLocationId;
                    obj.Level = request.TroopLevel[j];
                    obj.WoundedCount += request.WoundedCount[j];
                }
                try
                {
                    for (int j = 0; j < dict.Count(); j++)
                    {
                        var recoverResponse = GameService.BUsertroopManager.RecoverWounded(this.Player.PlayerId, dict.ElementAt(j).Key, dict.ElementAt(j).Value);
                        if (recoverResponse.Result.Case > 99 && recoverResponse.Result.Case < 200)
                        {
                            var result = recoverResponse.Result.Data;
                            foreach (var item in result.Value)
                                foreach (var items in item.InRecovery)
                                    this.RecoverList.Add(items);
                        }
                        else
                        {
                            suc = false; message = recoverResponse.Result.Message;
                            log.InfoFormat("Error in RecoverWounded {0} {1} ", recoverResponse.Result.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    suc = false; message = ex.Message;
                    log.InfoFormat("Exception in HandleWoundedTroops Block2 {0} {1} ", ex.Message, ex.StackTrace);
                }
            }
            catch (Exception ex)
            {
                suc = false; message = ex.Message;
                log.InfoFormat("Exception in HandleWoundedTroops Block1 {0} {1} ", ex.Message, ex.StackTrace);
            }
            if (suc)
                this.Player.SendOperation(OperationCode.WoundedHealReqeust, ReturnCode.OK);
            else
                this.Player.SendOperation(OperationCode.WoundedHealReqeust, ReturnCode.Failed, debuMsg: message);
        }

        public override void WoundedTroopTimerStatusRequest(WoundedTroopTimerStatusRequest request)
        {
            Dictionary<byte, object> dictNetwork = new Dictionary<byte, object>();
            try
            {
                var response = new WoundedTroopTimerStatusResponse(ref dictNetwork)
                {
                    BuildingLocationId = request.BuildingLocationId
                };

                foreach (var item in RecoverList)
                    response.TotalHealTime += item.TimeLeft;

                response.SetDictionary<WoundedTroopTimerStatusResponse>(response);
                Player.SendOperation(OperationCode.WoundedHealTimerRequest, ReturnCode.OK, dictNetwork);
            }
            catch(Exception ex)
            {
                log.InfoFormat("Exception in TroopHealStatusRequest {0} {1} ", ex.Message, ex.StackTrace);
                Player.SendOperation(OperationCode.WoundedHealTimerRequest, ReturnCode.Failed,debuMsg: ex.Message);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.Business;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Troop;
using GameOfRevenge.GameApplication;
using GameOfRevenge.GameHandlers;
using GameOfRevenge.Model;

namespace GameOfRevenge.Buildings.Handlers
{
    public class Infirmary : PlayerBuildingManager, IPlayerBuildingManager
    {
        public List<UnavaliableTroopInfo> RecoverList = new List<UnavaliableTroopInfo>();

        public Infirmary(PlayerInstance player, StructureDetails structureData, IGameBuildingManager baseBuildingManager) : base(player, structureData, baseBuildingManager)
        {
        }

        public override void HandleWoundedTroops(WoundedTroopHealRequest request)
        {
            log.Debug("****************************************HandleWoundedTroops START**********************************************");
            bool success = true;
            string message = "OK";
            try
            {
                var dict = new Dictionary<TroopType, List<WoundeAndDeadTroopsUpdate>>();
                for (int j = 0; j < request.TroopLevel.Length; j++)
                {
                    List<WoundeAndDeadTroopsUpdate> troopList = null;
                    var troopType = (TroopType)request.TroopType[j];
                    var troopLevel = request.TroopLevel[j];
                    if (!dict.ContainsKey(troopType))
                    {
                        dict.Add(troopType, new List<WoundeAndDeadTroopsUpdate>());
                    }
                    troopList = dict[troopType];
                    WoundeAndDeadTroopsUpdate obj = troopList.Find(d => (d.Level == troopLevel));
                    if (obj == null)
                    {
                        obj = new WoundeAndDeadTroopsUpdate();
                        troopList.Add(obj);
                    }
                    obj.BuildingLocation = request.BuildingLocationId;
                    obj.Level = troopLevel;
                    obj.WoundedCount += request.WoundedCount[j];
                }
                string test = "";
                try
                {
                    foreach (var element in dict)
                    {
                        test = "1";
                        var recoverResponse = GameService.BUsertroopManager.RecoverWounded(Player.PlayerId, element.Key, element.Value);
                        test = "2";
                        if ((recoverResponse.Result.Case >= 100) && recoverResponse.Result.Case < 200)
                        {
                            test = "3";
                            var result = recoverResponse.Result.Data;
                            test = "4";
                            foreach (var item in result.Value)
                            {
                                test = "5";
                                if (item.InRecovery == null) continue;

                                foreach (var items in item.InRecovery)
                                {
                                    test = "6";
                                    RecoverList.Add(items);
                                }
                            }
                            continue;
                        }

                        log.InfoFormat("Error in RecoverWounded {0} {1} ", recoverResponse.Result.Message);
                        throw new Exception(recoverResponse.Result.Message);
                    }
                }
                catch (Exception ex)
                {
                    success = false;
                    message = ex.Message;
                    log.InfoFormat("Exception in HandleWoundedTroops Block2 {0} {1} ", test+" " +ex.Message, ex.StackTrace);
                }
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                log.InfoFormat("Exception in HandleWoundedTroops Block1 {0} {1} ", ex.Message, ex.StackTrace);
            }
            Player.SendOperation(OperationCode.WoundedHealReqeust, success? ReturnCode.OK : ReturnCode.Failed, debuMsg: message);

            log.Debug("****************************************HandleWoundedTroops END**********************************************");
        }

        public override void WoundedTroopTimerStatusRequest(WoundedTroopTimerStatusRequest request)
        {
            var dictNetwork = new Dictionary<byte, object>();
            try
            {
                var response = new WoundedTroopTimerStatusResponse(ref dictNetwork)
                {
                    BuildingLocationId = request.BuildingLocationId
                };

//                response.StartTime = res.StartTime.ToUniversalTime().ToString("s") + "Z";

                foreach (var item in RecoverList)
                {
                    response.TotalHealTime += item.TimeLeft;
                }

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

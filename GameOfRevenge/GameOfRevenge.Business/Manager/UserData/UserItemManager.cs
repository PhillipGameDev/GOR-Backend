using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models;
using ExitGames.Logging;
using Newtonsoft.Json;
using GameOfRevenge.Common.Models.Boost;
using GameOfRevenge.Common.Models.PlayerData;
using System;
using GameOfRevenge.Common.Models.Quest;
using GameOfRevenge.Common.Services;

namespace GameOfRevenge.Business.Manager.UserData
{
    public class UserItemManager : BaseManager, IUserItemManager
    {

        public static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly UserHeroManager userHeroManager = new UserHeroManager();
        private readonly UserStructureManager userStructureManager = new UserStructureManager();
        private readonly UserTroopManager userTroopManager = new UserTroopManager();
        private readonly UserResourceManager resmanager = new UserResourceManager();
        private readonly UserActiveBoostManager boostManager = new UserActiveBoostManager();
        private readonly UserAcademyManager academyManager = new UserAcademyManager();
        protected static readonly IPlayerDataManager manager = new PlayerDataManager();

        public async Task<Response<List<PlayerItemDataTable>>> GetUserAllItems(int playerId)
        {
            try
            {
                if (playerId <= 0) throw new InvalidModelExecption("Invalid player id");

                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId }
                };

                var result = new List<PlayerItemDataTable>();

                var rewardsDataResp = await Db.ExecuteSPMultipleRow<PlayerItemDataTable>("GetPlayerAllItemData", spParams);
                if (rewardsDataResp.IsSuccess && rewardsDataResp.HasData) result.AddRange(rewardsDataResp.Data);

                return new Response<List<PlayerItemDataTable>>()
                {
                    Case = 100,
                    Data = result,
                    Message = "Data fetched successfully"
                };
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<PlayerItemDataTable>>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<PlayerItemDataTable>>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<PlayerDataTableUpdated>> ConsumeItem(int playerId, long playerDataId, int itemCount, string context = null)
        {
            var resp = await manager.GetPlayerDataById(playerDataId);
            if (!resp.IsSuccess || !resp.HasData) return new Response<PlayerDataTableUpdated>(200, "User reward not found");

            var playerData = resp.Data;
            var itemId = playerData.ValueId;
            var itemData = CacheItemManager.AllItems.FirstOrDefault(y => (y.Id == itemId));

            if (itemData == null) return new Response<PlayerDataTableUpdated>(201, "Reward data not found");

            if (int.TryParse(playerData.Value, out int count))
            {
                if (count >= itemCount)
                {
                    var value = itemData.Value * itemCount;
                    count -= itemCount;
                    try
                    {
                        long kingDetailsId = 0;
                        UserKingDetails kingDetails = null;
                        if ((itemData.DataType == DataType.Custom) && (itemData.ValueId < (int)CustomRewardType.VIPPoints))
                        {
                            var kingresp = await manager.GetPlayerData(playerId, DataType.Custom, (int)CustomValueType.KingDetails);
                            if (kingresp.IsSuccess && kingresp.HasData)
                            {
                                kingDetails = JsonConvert.DeserializeObject<UserKingDetails>(kingresp.Data.Value);
                                kingDetailsId = kingresp.Data.Id;
                            }
                            if (kingDetails == null) throw new InvalidModelExecption("King data corrupted");
                        }

                        bool updateKing = false;
                        switch (itemData.DataType)
                        {
                            case DataType.Custom:
                                switch ((CustomRewardType)itemData.ValueId)
                                {
                                    case CustomRewardType.KingExperiencePoints:
                                        kingDetails.Experience += value;
                                        updateKing = true;
                                        break;
                                    case CustomRewardType.KingStaminaPoints:
                                        kingDetails.MaxStamina += value;
                                        updateKing = true;
                                        break;
                                    case CustomRewardType.VIPPoints:
                                        var vipresp = await manager.GetPlayerData(playerId, DataType.Custom, (int)CustomValueType.VIPPoints);
                                        if (!vipresp.IsSuccess) throw new InvalidModelExecption(vipresp.Message);

                                        var vipdata = vipresp.Data;
                                        if (vipdata == null) throw new InvalidModelExecption("VIP data missing");

                                        var vipdetails = JsonConvert.DeserializeObject<UserVIPDetails>(vipdata.Value);
                                        vipdetails.Points += value;
                                        var json = JsonConvert.SerializeObject(vipdetails);
                                        var saveResp = await manager.UpdatePlayerDataID(playerId, vipdata.Id, json);
                                        if (!saveResp.IsSuccess) throw new InvalidModelExecption(saveResp.Message);
                                        break;
                                    case CustomRewardType.HeroPoints: //hero points
                                        var heroResp = await userHeroManager.AddHeroPoints(playerId, context, value);
                                        if (!heroResp.IsSuccess) throw new InvalidModelExecption(heroResp.Message);
                                        break;
                                    case CustomRewardType.VIPActivate:
                                        var vipActivateResp = await boostManager.ActivateVIPBoosts(playerId, value);
                                        if (!vipActivateResp.IsSuccess) throw new InvalidModelExecption(vipActivateResp.Message);
                                        break;
                                }

                                break;
                            case DataType.Resource:
                                var sumResp = await resmanager.SumResource(playerId, itemData.ValueId, value);
                                if (!sumResp.IsSuccess) throw new InvalidModelExecption(sumResp.Message);
                                break;
                            case DataType.CityBoost:
                                await boostManager.ActivateBoost(playerId, (CityBoostType)itemData.ValueId, value, 0);
                                break;
                            case DataType.Technology:
                                int location = 0;
                                TroopType troopType = TroopType.Other;
                                List<TroopDetails> listTroops = null;
                                Response<PlayerCompleteData> fullPlayerData;
                                switch ((NewBoostTech)itemData.ValueId)
                                {
                                    //                                    case NewBoostTech.TroopTrainingSpeedMultiplier:/*14*/
                                    case NewBoostTech.TroopTrainingTimeBonus:/*18*/
                                        int.TryParse(context, out location);
                                        if (location <= 0) throw new InvalidModelExecption("Invalid Troop location");

                                        fullPlayerData = await BaseUserDataManager.GetFullPlayerData(playerId);
                                        if (!fullPlayerData.IsSuccess) throw new InvalidModelExecption(fullPlayerData.Message);

                                        List<UnavaliableTroopInfo> listInTraining = null;
                                        UnavaliableTroopInfo troopTraining = null;
                                        fullPlayerData.Data.Troops.Find(troop =>
                                        {
                                            TroopDetails troopDetails = null;
                                            if (troop.TroopData != null)
                                            {
                                                troopType = troop.TroopType;
                                                listTroops = troop.TroopData;
                                                troopDetails = listTroops?.Find((data) =>
                                                {
                                                    listInTraining = data.InTraning;
                                                    troopTraining = listInTraining?.Find((info) => (info.BuildingLocId == location));
                                                    return troopTraining != null;
                                                });
                                            }

                                            return troopDetails != null;
                                        });
                                        if (troopTraining == null) throw new InvalidModelExecption("Training troop not found");

                                        troopTraining.Duration -= value;
                                        if (troopTraining.Duration < 0)
                                        {
                                            listInTraining.Remove(troopTraining);
                                        }
                                        await userTroopManager.UpdateTroops(playerId, troopType, listTroops);
                                        break;

                                    case NewBoostTech.TroopRecoverySpeedMultiplier:/*15*/
                                        var boostResp2 = await boostManager.ActivateBoost(playerId, CityBoostType.LifeSaver, value, 0);
                                        if (!boostResp2.IsSuccess) throw new InvalidModelExecption(boostResp2.Message);
                                        break;

                                    case NewBoostTech.TroopRecoveryTimeBonus:/*19*/
                                        int.TryParse(context, out location);
                                        if (location <= 0) throw new InvalidModelExecption("Invalid Troop location");

                                        fullPlayerData = await BaseUserDataManager.GetFullPlayerData(playerId);
                                        if (!fullPlayerData.IsSuccess) throw new InvalidModelExecption(fullPlayerData.Message);

                                        List<UnavaliableTroopInfo> listInRecovery = null;
                                        UnavaliableTroopInfo troopRecovery = null;
                                        fullPlayerData.Data.Troops.Find(troop =>
                                        {
                                            TroopDetails troopDetails = null;
                                            if (troop.TroopData != null)
                                            {
                                                troopType = troop.TroopType;
                                                listTroops = troop.TroopData;
                                                troopDetails = listTroops?.Find((data) =>
                                                {
                                                    listInRecovery = data.InRecovery;
                                                    troopRecovery = listInRecovery?.Find((info) => (info.BuildingLocId == location));
                                                    return troopRecovery != null;
                                                });
                                            }

                                            return troopDetails != null;
                                        });
                                        if (troopRecovery == null) throw new InvalidModelExecption("Injured troop not found");

                                        troopRecovery.Duration -= value;
                                        if (troopRecovery.Duration < 0)
                                        {
                                            listInRecovery.Remove(troopRecovery);
                                        }
                                        await userTroopManager.UpdateTroops(playerId, troopType, listTroops);
                                        break;

                                    case NewBoostTech.TroopMarchingReductionMultiplier://27
                                        long.TryParse(context, out long marchingId);
                                        if (marchingId == 0) throw new InvalidModelExecption("Invalid marching army");

                                        var marching = await manager.GetPlayerDataById(marchingId); //GetAllPlayerData(playerId, DataType.Marching);
                                        if (!marching.IsSuccess) throw new InvalidModelExecption(marching.Message);

                                        MarchingArmy marchingArmy = null;
                                        if (marching.HasData && !string.IsNullOrEmpty(marching.Data.Value))
                                        {
                                            try
                                            {
                                                marchingArmy = JsonConvert.DeserializeObject<MarchingArmy>(marching.Data.Value);
                                            }
                                            catch { }
                                        }
                                        if (marchingArmy == null) throw new InvalidModelExecption("Invalid marching data");
                                        var returning = marchingArmy.IsRecalling || marchingArmy.IsTimeForReturn;
                                        var timeLeft = returning ? marchingArmy.TimeLeft : marchingArmy.TimeLeftForTask;
                                        if (timeLeft < 5) throw new InvalidModelExecption("Consume reward is not required");

                                        var percentage = (value > 100) ? 100 : value;
                                        var reduction = (int)(timeLeft * (percentage / 100f));
                                        if (returning)
                                        {
                                            marchingArmy.ReturnReduction += reduction;
                                        }
                                        else
                                        {
                                            marchingArmy.AdvanceReduction += reduction;
                                        }
                                        var marchingJson = JsonConvert.SerializeObject(marchingArmy.Base());
                                        await manager.UpdatePlayerDataID(playerId, marchingId, marchingJson);
                                        break;
                                    case NewBoostTech.ResearchTimeBonus:/*21*/
                                        int.TryParse(context, out int techId);
                                        if (techId <= 0) throw new InvalidModelExecption("Invalid technology item");

                                        await academyManager.SpeedUpUpgradeItem(playerId, techId, value);
                                        break;

                                    //                                    case NewBoostTech.BuildingSpeedMultiplier:/*7*/
                                    case NewBoostTech.BuildingTimeBonus:/*20*/
                                        int.TryParse(context, out location);
                                        if (location <= 0) throw new InvalidModelExecption("Invalid Building location");

                                        var speedupResp = await userStructureManager.SpeedupBuilding(playerId, location, value);
                                        if (!speedupResp.IsSuccess) throw new InvalidModelExecption(speedupResp.Message);
                                        break;
                                    default:
                                        throw new InvalidModelExecption("Can't consume the reward on this place");
                                }
                                break;
                        }

                        if (updateKing)
                        {
                            var kingjson = JsonConvert.SerializeObject(kingDetails);
                            var kingResp = await manager.UpdatePlayerDataID(playerId, kingDetailsId, kingjson);
                            if (!kingResp.IsSuccess) throw new InvalidModelExecption(kingResp.Message);
                        }
                    }
                    catch (InvalidModelExecption ex)
                    {
                        return new Response<PlayerDataTableUpdated>(205, ex.Message);
                    }
                    catch (Exception ex)
                    {
                        return new Response<PlayerDataTableUpdated>(205, "Error consuming reward");
                    }

                    return await manager.UpdatePlayerDataID(playerId, playerData.Id, count.ToString());
                }
                else
                {
                    return new Response<PlayerDataTableUpdated>(204, "Reward already consumed");
                }
            }
            else
            {
                return new Response<PlayerDataTableUpdated>(203, "User reward value corrupted");
            }
        }
    }
}

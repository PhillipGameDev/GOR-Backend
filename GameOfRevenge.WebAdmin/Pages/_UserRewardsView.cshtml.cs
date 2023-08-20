using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Business.Manager.UserData;
using GameOfRevenge.WebAdmin.Models;
using GameOfRevenge.Common.Interface.UserData;

namespace GameOfRevenge.WebAdmin.Pages
{
    public class _UserRewardsViewModel : PageModel
    {
        public List<PlayerDataReward> Data { get; set; }

        public static string GetTitle(PlayerDataReward rewardData)
        {
            var reward = new DataReward()
            {
                DataType = rewardData.DataType,
                ValueId = rewardData.ValueId,
                Value = rewardData.Value
            };

            return reward.GetProperties().Item1;
        }

/*        public static (string, string, string) GetProperties(DataReward reward)
        {
//            var percentage = false;
            string value = null;
            if (reward.DataType == DataType.Technology)
            {
/*                switch ((NewBoostTech)reward.ValueId)
                {
                    case NewBoostTech.BuildingSpeedMultiplier:
                    case NewBoostTech.ResearchSpeedMultiplier:
                    case NewBoostTech.TroopRecoverySpeedMultiplier:
                    case NewBoostTech.TroopMarchingSpeedMultiplier:
                    case NewBoostTech.TroopTrainingSpeedMultiplier:
                    case NewBoostTech.KingStaminaRecoverySpeedMultiplier:
                        break;
                    default:* /
                        value = Helper.ChangeSecondsToTimeFormat(reward.Value, false);
//                        break;
//                }
            }
            else
//            if (value == null)
            {
                value = (reward.Value >= 1000)? Helper.ToKMB(reward.Value) : string.Format("{0:N0}", reward.Value);
            }
//            if (percentage) value = ((reward.Value < 0)? "-" : "+") + value + "%";

            string str = null;
            string title = null;
            string desc = null;
//            Texture texture = null;
            string filename = null;
            switch (reward.DataType)
            {
                case DataType.Custom:
                    switch (reward.ValueId)
                    {
                        case 1:
                            str = Localization.GetText("{0} Experience Points for your King", Localization.REWARDS);
                            title = string.Format(str, value);
                            str = Localization.GetText("Use this item to add {0:N0} Experience Points to your King", Localization.REWARDS);
                            desc = string.Format(str, reward.Value);
                            filename = "Rewards/king_exp_icon";
                            break;
                        case 2:
                            str = Localization.GetText("{0} Stamina Points for your King", Localization.REWARDS);
                            title = string.Format(str, value);
                            str = Localization.GetText("Use this item to add {0:N0} Stamina Points to your King", Localization.REWARDS);
                            desc = string.Format(str, reward.Value);
                            filename = "Rewards/stamina_icon";
                            break;
                        case 3:
                            str = Localization.GetText("{0} VIP Points", Localization.REWARDS);
                            title = string.Format(str, value);
                            str = Localization.GetText("Use this item to receive {0:N0} VIP Points", Localization.REWARDS);
                            desc = string.Format(str, reward.Value);
                            filename = "Rewards/vip_points_icon";
                            break;
                        case 4:
                            str = Localization.GetText("{0} Hero Points", Localization.REWARDS);
                            title = string.Format(str, value);
                            str = Localization.GetText("Use this item to receive {0:N0} Hero Points", Localization.REWARDS);
                            desc = string.Format(str, reward.Value);
                            filename = "Rewards/hero_points_icon";
                            break;
                    }
                    break;
/*                case DataType.Hero://DEPRECATED
                    switch (reward.ValueId)
                    {
                        case 1:
                            str = Localization.GetText("{0} Hero Points", Localization.REWARDS);
                            title = string.Format(str, value);
                            str = Localization.GetText("Use this item to receive {0:N0} Hero Points", Localization.REWARDS);
                            desc = string.Format(str, reward.Value);
                            filename = "Rewards/hero_points_icon";
                            break;
                    }
                    break;* /
                case DataType.Resource:
                    var type = (ResourceType)reward.ValueId;
                    var resName = Localization.GetText(type.ToString(), Localization.ENUMS);
                    str = Localization.GetText("{0} {1}", Localization.REWARDS);
                    title = string.Format(str, value, resName);
                    str = Localization.GetText("Use it to receive {0:N0} {1}", Localization.REWARDS);
                    desc = string.Format(str, reward.Value, resName);

                    switch (type)
                    {
                        case ResourceType.Food: filename = "Rewards/food_icon"; break;
                        case ResourceType.Wood: filename = "Rewards/wood_icon"; break;
                        case ResourceType.Ore: filename = "Rewards/ore_icon"; break;
                        case ResourceType.Gems: filename = "Rewards/gems_icon"; break;
                    }
                    break;
                case DataType.Technology:
                    string str2 = null;
                    var techType = (NewBoostTech)reward.ValueId;
                    switch (techType)
                    {
                        case NewBoostTech.TroopTrainingSpeedMultiplier:/*14* /
                        case NewBoostTech.TroopTrainingTimeBonus:/*18* /
                            str = Localization.GetText("{0} Training Speedup", Localization.REWARDS);
                            title = string.Format(str, Helper.ChangeSecondsToFormatTimeWords(reward.Value, true));
                            str2 = Localization.GetText("Reduces troop training time by {0}", Localization.REWARDS);
                            desc = string.Format(str2, Helper.ChangeSecondsToFormatTimeWords(reward.Value));
                            filename = "Rewards/troop_icon";
                            break;

                        case NewBoostTech.TroopRecoverySpeedMultiplier:/*15* /
                        case NewBoostTech.TroopRecoveryTimeBonus:/*19* /
                            if (techType == NewBoostTech.TroopRecoverySpeedMultiplier)
                            {
                                str = Localization.GetText("{0} Recovery Boost", Localization.REWARDS);
                                str2 = Localization.GetText("Boost troop recovery speed for {0}", Localization.REWARDS);
//                                texture = CityBoostManager.Instance.GetIcon(CityBoostType.LifeSaver);
                            }
                            else
                            {
                                str = Localization.GetText("{0} Recovery Speedup", Localization.REWARDS);
                                str2 = Localization.GetText("Reduces the recovery time of a troop by {0}", Localization.REWARDS);
                                filename = "Rewards/troop_healing_icon";
                            }
                            title = string.Format(str, Helper.ChangeSecondsToFormatTimeWords(reward.Value, true));
                            desc = string.Format(str2, Helper.ChangeSecondsToFormatTimeWords(reward.Value));
                            break;
                        case NewBoostTech.TroopMarchingReductionMultiplier://27
                            str = Localization.GetText("{0} Marching Time Reduction", Localization.REWARDS);
                            title = string.Format(str, reward.Value + "%");
                            str2 = Localization.GetText("Reduces one-way marching time of the army by {0}", Localization.REWARDS);
                            desc = string.Format(str2, reward.Value + "%");
                            filename = "Rewards/troop_icon";
                            break;

                        case NewBoostTech.BuildingSpeedMultiplier:/*7* /
                        case NewBoostTech.BuildingTimeBonus:/*20* /
                            str = Localization.GetText("{0} Construction Speedup", Localization.REWARDS);
                            title = string.Format(str, Helper.ChangeSecondsToFormatTimeWords(reward.Value, true));
                            str2 = Localization.GetText("Reduces the construction time by {0}", Localization.REWARDS);
                            desc = string.Format(str2, Helper.ChangeSecondsToFormatTimeWords(reward.Value));
                            filename = "Rewards/construction2_icon";
                            break;

                        case NewBoostTech.ResearchSpeedMultiplier:/*10* /
                        case NewBoostTech.ResearchTimeBonus:/*21* /
                            if (techType == NewBoostTech.ResearchSpeedMultiplier)
                            {
                                str = Localization.GetText("{0} Research Boost", Localization.REWARDS);
                                str2 = Localization.GetText("Boosts technology research speed for {0}", Localization.REWARDS);
//                                texture = CityBoostManager.Instance.GetIcon(CityBoostType.TechBoost);
                            }
                            else
                            {
                                str = Localization.GetText("{0} Research Speedup", Localization.REWARDS);
                                str2 = Localization.GetText("Reduces the research time by {0}", Localization.REWARDS);
                                filename = "Rewards/academy_research_icon";
                            }
                            title = string.Format(str, Helper.ChangeSecondsToFormatTimeWords(reward.Value, true));
                            desc = string.Format(str2, Helper.ChangeSecondsToFormatTimeWords(reward.Value));
                            break;
                    }
                    break;
            }
//            if (filename != null) texture = Resources.Load<Texture>(filename);

            return (title, desc, value);
        }*/

        public _UserRewardsViewModel(List<PlayerDataReward> rewards)
        {
            Data = rewards;
        }

        public static async Task<IActionResult> OnGetRewardsViewAsync(IAdminDataManager manager, int playerId)
        {
//            Console.WriteLine("get rewards ply="+playerId);

            var list = await manager.GetAllPlayerRewards(playerId);

            return UsersModel.NewPartial("_UserRewardsView", new _UserRewardsViewModel(list));
        }

        public static async Task<IActionResult> OnGetEditRewardViewAsync(IAdminDataManager manager, int playerId, long playerDataId, string description)
        {
//            Console.WriteLine("get reward ply="+playerId+" id="+ playerDataId+ " description="+description);

            InputRewardModel model = null;
            var allRewards = await manager.GetAllPlayerRewards(playerId);
            if (allRewards != null)
            {
                var data = allRewards.Find(x => (x.PlayerDataId == playerDataId));
                if (data != null)
                {
                    model = new InputRewardModel()
                    {
                        PlayerId = playerId,
                        PlayerDataId = playerDataId,
                        Description = description,
                        Value = data.Count
                    };
                }
            }

            return UsersModel.NewPartial("Forms/_EditRewardView", model);
        }

        public static async Task<IActionResult> OnGetAddRewardsViewAsync(IAdminDataManager manager, int playerId, bool applyToAll)
        {
//            Console.WriteLine("get add rewards ply=" + playerId+"  apply to all="+applyToAll);

            InputRewardsModel model = null;
            var allRewards = await manager.GetAvailableRewards();
            if (allRewards != null)
            {
                var uniqueDataTypes = allRewards.GroupBy(dataReward => dataReward.DataType)
                                    .Select(group => group.Key).OrderBy(x => x).ToList();
                var allOptions = allRewards.ConvertAll(x => (x.RewardId, x.DataType, x.ValueId, x.Value, x.GetProperties().Item1));

                var groupedOptions = allOptions.GroupBy(x => x.Item5).Select(group => group.First()).ToList();

                var sortedOptions = groupedOptions.OrderBy(x => x.DataType).ThenBy(x => x.ValueId).ThenBy(x => x.Value).ToList();
                model = new InputRewardsModel()
                {
                    PlayerId = playerId,
                    ApplyToAll = applyToAll,
                    DataTypes = uniqueDataTypes,
                    Options = sortedOptions.ConvertAll(x => (x.RewardId, x.DataType, x.Item5))
                };
            }

            return UsersModel.NewPartial("Forms/_AddRewardsView", model);
        }

        public static async Task<IActionResult> OnPostSaveRewardChangeAsync(IAdminDataManager manager, InputRewardModel inputReward)
        {
            int playerId = inputReward.PlayerId;
            long playerDataId = inputReward.PlayerDataId;
            int rewardValue = inputReward.RewardValue;
//            Console.WriteLine("post save reward ply = " + playerId+" playerDataId="+playerDataId+" rewardValue="+rewardValue);

            var pdm = new PlayerDataManager();
            var resp = await pdm.UpdatePlayerDataID(playerId, playerDataId, rewardValue.ToString());
            if (!resp.IsSuccess) throw new Exception(resp.Message);

            return new JsonResult(new { Success = true });
        }

        public static async Task<IActionResult> OnPostAddRewardAsync(IAdminDataManager manager, InputRewardsModel inputRewards)
        {
            int playerId = inputRewards.PlayerId;
            bool applyToAll = inputRewards.ApplyToAll;
            var rewards = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(inputRewards.RewardValues);
            foreach (var reward in rewards)
            {
                if (!int.TryParse(reward.Key, out int rewardId) || !int.TryParse(reward.Value, out int count))
                {
                    throw new Exception("Error in form values");
                }

//                Console.WriteLine("post add reward ply = " + playerId + " applyToAll=" + applyToAll + " reward id="+ rewardId + " rewardValue=" + count);
                var pdm = new PlayerDataManager();
                if (applyToAll)
                {
                    var playersResp = await manager.GetPlayers();
                    if (!playersResp.IsSuccess || !playersResp.HasData) throw new Exception(playersResp.Message);

                    foreach (var ply in playersResp.Data)
                    {
                        if (ply.Invaded > 0) continue;

                        var resp = await pdm.IncrementPlayerData(ply.PlayerId, DataType.Reward, rewardId, count);
                        if (!resp.IsSuccess)
                        {
                            Console.WriteLine("Player_" + ply.PlayerId + " error incrementing data: >" + resp.Message + "<");
                        }
                    }
                }
                else
                {
                    var resp = await pdm.IncrementPlayerData(playerId, DataType.Reward, rewardId, count);
                    if (!resp.IsSuccess) throw new Exception(resp.Message);
                }
            }

            return new JsonResult(new { Success = true });
        }
    }
}
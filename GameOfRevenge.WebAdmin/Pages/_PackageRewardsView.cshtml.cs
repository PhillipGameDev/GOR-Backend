using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Boost;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.WebAdmin.Models;

namespace GameOfRevenge.WebAdmin.Pages
{
    public class _PackageRewardsViewModel : PageModel
    {
        public ProductPackage Data { get; set; }


        public static string GetTitle(IReadOnlyDataReward rewardData)
        {
            var reward = new DataReward()
            {
                DataType = rewardData.DataType,
                ValueId = rewardData.ValueId,
                Value = rewardData.Value
            };

            return reward.GetProperties().Item1;
        }

        public _PackageRewardsViewModel(ProductPackage product)
        {
            Data = product;
        }

        public static async Task<IActionResult> OnGetPackageRewardsViewAsync(IAdminDataManager manager, int packageId)
        {
//            Console.WriteLine("get pkg rewards pkg="+ packageId);

            ProductPackage package = null;
            var packages = await manager.GetAllProductPackages();
            if (packages != null)
            {
                package = packages.Find(x => (x.PackageId == packageId));
            }

            return UsersModel.NewPartial("_PackageRewardsView", new _PackageRewardsViewModel(package));
        }

        public static IActionResult OnGetEditPackageRewardView(IAdminDataManager manager, int packageId, int rewardId, int count, string description)
        {
//            Console.WriteLine("get edit pkg reward pkg="+ packageId + " reward="+ rewardId + " description="+description);

/*            InputPackageRewardModel model = null;
            var allRewards = await manager.GetAvailableRewards();
            if (allRewards != null)
            {
                var data = allRewards.Find(x => (x.RewardId == rewardId));
                if (data != null)
                {
                    model = new InputPackageRewardModel()
                    {
                        PackageId = packageId,
                        RewardId = rewardId,
                        Description = description,
                        Value = data.Count
                    };
                }
            }*/
            var model = new InputPackageRewardModel()
            {
                PackageId = packageId,
                RewardId = rewardId,
                RewardValue = count,
                Description = description
            };

            return UsersModel.NewPartial("Forms/_EditPackageRewardView", model);
        }

        public static async Task<IActionResult> OnGetAddPackageRewardsViewAsync(IAdminDataManager manager, int packageId, int questId)
        {
//            Console.WriteLine("get add pkg rewards pkg=" + packageId+" quest id="+questId);

            InputPackageRewardsModel model = null;
            var allRewards = await manager.GetAvailableRewards();
            if (allRewards != null)
            {
                allRewards = allRewards.Where(item => (item.QuestId == questId)).ToList();

                var uniqueDataTypes = new List<DataType>() {
                    DataType.Resource, DataType.Custom, DataType.Technology
                };
//                var uniqueDataTypes = Enum.GetValues(typeof(DataType)).Cast<DataType>().ToList();
//                var uniqueDataTypes = allRewards.GroupBy(dataReward => dataReward.DataType)
//                                    .Select(group => group.Key).OrderBy(x => x).ToList();

                var allOptions = allRewards.ConvertAll(x => (x.RewardId, x.DataType, x.ValueId, x.Value, x.GetProperties().Item1));

                var groupedOptions = allOptions.GroupBy(x => x.Item5).Select(group => group.First()).ToList();

                var sortedOptions = groupedOptions.OrderBy(x => x.DataType).ThenBy(x => x.ValueId).ThenBy(x => x.Value).ToList();
                sortedOptions.Add((0, DataType.Resource, 0, 0, "New Resource Option"));
                sortedOptions.Add((0, DataType.Custom, 0, 0, "New Custom Option"));
                sortedOptions.Add((0, DataType.Technology, 0, 0, "New Technology Option"));

                var subOptions = new List<(DataType, int, string, string)>()
                {
                    Tuple(DataType.Custom, CustomRewardType.KingExperiencePoints),
                    Tuple(DataType.Custom, CustomRewardType.KingStaminaPoints),
                    Tuple(DataType.Custom, CustomRewardType.VIPPoints),
                    Tuple(DataType.Custom, CustomRewardType.HeroPoints),
                    Tuple(DataType.Resource, ResourceType.Food),
                    Tuple(DataType.Resource, ResourceType.Wood),
                    Tuple(DataType.Resource, ResourceType.Ore),
                    Tuple(DataType.Resource, ResourceType.Gems),
                    Tuple(DataType.Technology, NewBoostTech.TroopTrainingSpeedMultiplier, "(Seconds)"),
                    Tuple(DataType.Technology, NewBoostTech.TroopTrainingTimeBonus, "(Seconds)"),
                    Tuple(DataType.Technology, NewBoostTech.TroopRecoverySpeedMultiplier, "(Seconds)"),
                    Tuple(DataType.Technology, NewBoostTech.TroopRecoveryTimeBonus, "(Seconds)"),
                    Tuple(DataType.Technology, NewBoostTech.TroopMarchingReductionMultiplier, "(Percentage)"),
                    Tuple(DataType.Technology, NewBoostTech.BuildingSpeedMultiplier, "(Seconds)"),
                    Tuple(DataType.Technology, NewBoostTech.BuildingTimeBonus, "(Seconds)"),
                    Tuple(DataType.Technology, NewBoostTech.ResearchSpeedMultiplier, "(Seconds)"),
                    Tuple(DataType.Technology, NewBoostTech.ResearchTimeBonus, "(Seconds)")
                };

                model = new InputPackageRewardsModel()
                {
                    PackageId = packageId,
                    QuestId = questId,
                    DataTypes = uniqueDataTypes,
                    Options = sortedOptions.ConvertAll(x => (x.RewardId, x.DataType, x.Item5)),
                    SubOptions = subOptions
                };
            }

            return UsersModel.NewPartial("Forms/_AddPackageRewardsView", model);
        }

        private static (DataType, int, string, string) Tuple(DataType dataType, object typeValue, string valueLabel = null)
        {
            return (dataType, Convert.ToInt32(typeValue), typeValue.ToString(), valueLabel);
        }

        public static async Task<IActionResult> OnPostSavePackageRewardChangeAsync(IAdminDataManager manager, InputPackageRewardModel inputReward)
        {
            int packageId = inputReward.PackageId;
//            int questId = inputReward.QuestId;
            int rewardId = inputReward.RewardId;
            int rewardValue = inputReward.RewardValue;
//            Console.WriteLine("post save pkg reward pkg = " + packageId + " rewardId="+rewardId+" rewardValue="+rewardValue);

            var resp = await manager.UpdatePackageReward(packageId, rewardId, rewardValue);
            if (!resp.IsSuccess) throw new Exception(resp.Message);

            return new JsonResult(new { Success = true });
        }

        public static async Task<IActionResult> OnPostAddPackageRewardAsync(IAdminDataManager manager, InputPackageRewardsModel inputRewards)
        {
            int packageId = inputRewards.PackageId;
            int questId = inputRewards.QuestId;
            var rewards = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string[]>>(inputRewards.RewardValues);
            foreach (var reward in rewards)
            {
                string err = null;
                try
                {
                    var rewardId = int.Parse(reward.Key);
                    var count = int.Parse(reward.Value[3]);
                    if (rewardId == 0)
                    {
                        var dataType = (DataType)Enum.Parse(typeof(DataType), reward.Value[0]);
                        var valueId = int.Parse(reward.Value[1]);
                        var value = int.Parse(reward.Value[2]);
//                        Console.WriteLine("post add pkg reward pkg = " + packageId + " questId=" + questId + " reward id=" + rewardId +
//                                        " dataType=" + dataType + " valueId="+valueId+" value="+value+" count=" + count);
                        var respAdd = await manager.AddQuestReward(questId, dataType, valueId, value, count);
                        if (!respAdd.IsSuccess)
                        {
                            if ((respAdd.Case == 200) && (respAdd.HasData))
                            {
                                rewardId = respAdd.Data.Value;
                            }
                            else
                            {
                                err = respAdd.Message;
                            }
                        }
                    }

                    if (rewardId > 0)
                    {
//                        Console.WriteLine("post add pkg reward pkg = " + packageId + " questId=" + questId + " reward id=" + rewardId + " count=" + count);
                        var resp = await manager.IncrementPackageReward(packageId, rewardId, count);
                        if (!resp.IsSuccess) err = resp.Message;
                    }
                }
                catch
                {
                    err = "Error in form values";
                }

                if (err != null) throw new Exception(err);
            }

            return new JsonResult(new { Success = true });
        }
    }
}
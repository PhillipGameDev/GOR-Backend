using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ExitGames.Logging;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Services;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Boost;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Common.Models.Quest;
using GameOfRevenge.Common.Models.Quest.Template;
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Business.Manager.Base;

namespace GameOfRevenge.Business.Manager.UserData
{
    public class UserQuestManager : BaseManager, IUserQuestManager
    {
        public static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly UserHeroManager userHeroManager = new UserHeroManager();
        private readonly UserStructureManager userStructureManager = new UserStructureManager();
        private readonly UserTroopManager userTroopManager = new UserTroopManager();
        private readonly UserResourceManager resmanager = new UserResourceManager();
        private readonly UserActiveBoostManager boostManager = new UserActiveBoostManager();
        protected static readonly IPlayerDataManager manager = new PlayerDataManager();

        public async Task<Response<List<PlayerRewardDataTable>>> GetUserAllRewards(int playerId)
        {
            try
            {
                if (playerId <= 0) throw new InvalidModelExecption("Invalid player id");

                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId }
                };

                return await Db.ExecuteSPMultipleRow<PlayerRewardDataTable>("GetPlayerAllRewardData", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<PlayerRewardDataTable>>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<PlayerRewardDataTable>>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<List<PlayerQuestDataTable>>> GetAllQuestProgress(int playerId)
        {
            return await Db.ExecuteSPMultipleRow<PlayerQuestDataTable>("GetPlayerAllQuestData", new Dictionary<string, object>()
            {
                { "PlayerId", playerId }
            });
        }

        private async Task<Response<List<PlayerChapterData>>> GetAllChapterRedeemed(int playerId)
        {
            return await Db.ExecuteSPMultipleRow<PlayerChapterData>("GetPlayerAllChapterData", new Dictionary<string, object>()
            {
                { "PlayerId", playerId }
            });
        }

/*        public async Task<Response<List<UserRecordNewBoost>>> GetAllPlayerCustomData(int playerId)
        {
            try
            {
                if (playerId < 1) throw new InvalidModelExecption("Invalid player id");

                var list = new List<UserRecordNewBoost>();
                var response = await manager.GetAllPlayerData(playerId, DataType.Custom);
                if (response.IsSuccess && response.HasData)
                {
                    foreach (var data in response.Data)
                    {
                        UserNewBoost boost = null;
                        try
                        {
                            boost = JsonConvert.DeserializeObject<User>(data.Value);
                        }
                        catch { }

                        if (boost?.TimeLeft > 0)
                        {
                            list.Add(new UserRecordNewBoost(data.Id, boost));
                        }
                    }
                }

                return new Response<List<UserRecordNewBoost>>(list);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<UserRecordNewBoost>>()
                {
                    Case = 200,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (Exception)
            {
                return new Response<List<UserRecordNewBoost>>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError()
                };
            }
        }*/

        public async Task<Response> RedeemQuestReward(int playerId, int questId)
        {
            var questProgress = await GetAllQuestProgress(playerId);
            if (!questProgress.IsSuccess || !questProgress.HasData)
            {
                return new Response(questProgress.Case, questProgress.Message);
            }

            var questData = questProgress.Data.Find(x => (x.QuestId == questId));
            if (questData == null) return new Response(200, "Quest not found");

            if (!questData.Completed) return new Response(200, "Quest not completed");
            if (questData.Redeemed) return new Response(201, "Quest reward already redemeed");

            var questRewards = CacheQuestDataManager.GetQuestData(questData.QuestId);
            var collectResp = await CollectRewards(playerId, questRewards.Rewards);
            if (collectResp != ReturnCode.OK) return new Response(202, "Failed to collect rewards");

            QuestResourceData quest = null;
            if (questRewards.Quest.QuestGroup == QuestGroupType.DAILY_QUEST)
            {
                if (questRewards.Quest.QuestType == QuestType.ResourceCollection)
                {
                    var resp = await UpdateQuestData(playerId, questId, false);
                    if (resp.IsSuccess && resp.HasData)
                    {
                        try
                        {
                            quest = JsonConvert.DeserializeObject<QuestResourceData>(resp.Data.ProgressData);
                        }
                        catch { }
                    }
                }
            }

            if ((quest == null) || (quest.Iteration < 1))//TODO: iteration validation required?
            {
                var redemeedResp = await Db.ExecuteSPNoData("RedeemQuestReward", new Dictionary<string, object>()
                {
                    { "PlayerQuestUserId", questData.QuestUserDataId }
                });
                return redemeedResp;
            }

            return new Response(100, "Updated quest reward");
        }

        public async Task<Response> RedeemChapterReward(int playerId, int chapterId)
        {
            var response = await GetUserAllChapterAndQuestProgress(playerId);
            if (!response.IsSuccess || !response.HasData) return new Response(200, response.Message);

            var (userChapterQuests, _) = response.Data;
            var chapterData = userChapterQuests.Find(x => (x.ChapterId == chapterId));
            if (chapterData == null) return new Response(201, "Chapter not found");

            if (!chapterData.AllQuestsCompleted) return new Response(202, "Chapter not completed");
            if (chapterData.Redeemed) return new Response(203, "Quest reward already redemeed");

            //var chapterProgress = await GetAllChapterProgress(playerId);
            //if (!chapterProgress.IsSuccess || !chapterProgress.HasData) return new Response(chapterProgress.Case, chapterProgress.Message);
            //var chapterData = chapterProgress.Data.FirstOrDefault(x => x.ChapterId == chapterId);
            //if (chapterData == null) return new Response(200, "Chapter not completed");
            //if (chapterData.Redemeed) return new Response(201, "Quest reward already redemeed");

            //var questProgress = await GetAllQuestProgress(playerId);
            //if (!questProgress.IsSuccess || !questProgress.HasData) return new Response(questProgress.Case, questProgress.Message);
            //foreach (var quest in questProgress.Data) if (!quest.Completed) return new Response(200, "Chapter not completed");

            var questRewards = CacheQuestDataManager.GetFullChapterData(chapterId);
            var resp = await CollectRewards(playerId, questRewards.Chapter.Rewards);
            if (resp != ReturnCode.OK) return new Response(204, "Failed to collect rewards");

            var redemeedResp = await Db.ExecuteSPNoData("RedeemChapterReward", new Dictionary<string, object>()
            {
                { "PlayerChapterUserId", chapterData.ChapterUserDataId }
            });

            return redemeedResp;
        }

/*        public class UserChapterAllQuestProgress
        {
            public List<int> ChapterRedeemed;
            public List<PlayerQuestData> QuestProgress;
        }

        public async Task<Response<UserChapterAllQuestProgress>> GetAllQuestProgressData(int playerId)
        {
            var response2 = await GetAllChapterProgress(playerId);//chapter redeem
            var userChapterRedemeed = new List<int>();
            foreach (var chapRedeemed in response2.Data)
            {
                if (chapRedeemed.Redemeed) userChapterRedemeed.Add(chapRedeemed.ChapterId);
            }
            var userQuestData = await GetAllQuestProgress(playerId);

            var userQuests = new List<PlayerQuestData>();
            var chapterQuests = CacheData.CacheQuestDataManager.ChapterQuests;
            foreach (var chapter in chapterQuests)
            {
                foreach (var questReward in chapter.Quests)
                {
                    var questData = userQuestData.Data.Find(x => x.QuestId == questReward.Quest.QuestId);
                    if (questData == null) continue;

                    userQuests.Add(questData);
                }
            }

            foreach (var questData in userQuestData.Data)
            {
                if (!userQuests.Contains(questData)) userQuests.Add(questData);
            }

            var chapterQuestRels = new UserChapterAllQuestProgress()
            {
                ChapterRedeemed = userChapterRedemeed,
                QuestProgress = userQuests
            };

            return new Response<UserChapterAllQuestProgress>(chapterQuestRels, userQuestData.Case, userQuestData.Message);
        }*/

        public async Task<Response<UserChapterAllQuestProgress>> GetUserAllQuestProgress(int playerId, bool fullTree = false)
        {
            var response = await GetUserAllChapterAndQuestProgress(playerId, fullTree);
            if (!response.IsSuccess || !response.HasData)
            {
                return new Response<UserChapterAllQuestProgress>(200, response.Message);
            }

            var (userChapterQuests, allUserQuestProgress) = response.Data;

            var allQuestRewards = CacheQuestDataManager.AllQuestRewards;
            var allChapterQuests = CacheQuestDataManager.ChapterQuests;
            var allDailyQuests = CacheQuestDataManager.DailyQuests;

            var dailyQuests = new List<PlayerQuestDataTable>();
            foreach (var questReward in allQuestRewards)
            {
                var questData = questReward.Quest;
                if (questData == null) continue;

                var chapterQuest = allChapterQuests.FirstOrDefault(x =>
                {
                    return (x.Quests.FirstOrDefault(y => (y.Quest.QuestId == questData.QuestId)) != null);
                });
                if (chapterQuest != null) continue;

                var dailyQuest = allDailyQuests.FirstOrDefault(x => (x.Quest.QuestId == questData.QuestId));
                if (dailyQuest != null)
                {
                    var userQuest = allUserQuestProgress.Find(x => x.QuestId == questData.QuestId);
                    if (fullTree || ((userQuest != null) && (userQuest.Completed || (userQuest.ProgressData != null))))
                    {
                        var questProgress = new PlayerQuestDataTable() { QuestId = questData.QuestId };
                        if (userQuest != null)
                        {
                            questProgress.QuestUserDataId = userQuest.QuestUserDataId;
                            questProgress.Completed = userQuest.Completed;
                            questProgress.ProgressData = userQuest.ProgressData;
                            questProgress.Redeemed = userQuest.Redeemed;
                            questProgress.QuestId = userQuest.QuestId;
                        }
                        dailyQuests.Add(questProgress);
                    }
                }
            }

            var chapterQuestRels = new UserChapterAllQuestProgress()
            {
                ChapterQuests = userChapterQuests,
                DailyQuests = dailyQuests
            };

            return new Response<UserChapterAllQuestProgress>(chapterQuestRels, response.Case, response.Message);
        }

        public async Task<Response<(List<UserChapterQuestData>, List<PlayerQuestDataTable>)>> GetUserAllChapterAndQuestProgress(int playerId, bool fullTree = false)
        {
            var userChapterRedeemed = await GetAllChapterRedeemed(playerId);
            if (!userChapterRedeemed.IsSuccess || !userChapterRedeemed.HasData)
            {
                return new Response<(List<UserChapterQuestData>, List<PlayerQuestDataTable>)>(200, userChapterRedeemed.Message);
            }
            var userQuestData = await GetAllQuestProgress(playerId);
            if (!userQuestData.IsSuccess || !userQuestData.HasData)
            {
                return new Response<(List<UserChapterQuestData>, List<PlayerQuestDataTable>)>(201, userQuestData.Message);
            }

            var chapterQuestRels = new List<UserChapterQuestData>();
            foreach (var chapterQuests in CacheQuestDataManager.ChapterQuests)
            {
                var chapter = chapterQuests.Chapter;
                var questRewards = chapterQuests.Quests;

                var userChapter = new UserChapterQuestData()
                {
                    ChapterId = chapter.ChapterId,
                    Name = chapter.Name,
                    Description = chapter.Description,
                    Quests = new List<PlayerQuestDataTable>(),
                    TotalQuests = questRewards.Count
                };
                var chapterRedeemed = userChapterRedeemed.Data.Find(x => (x.ChapterId == chapter.ChapterId));
                if (chapterRedeemed != null)
                {
                    userChapter.ChapterUserDataId = chapterRedeemed.ChapterUserDataId;
                    userChapter.Redeemed = chapterRedeemed.Redemeed;
                }

                var completedFound = false;
                var completedIdx = 0;
                var lastIdx = -1;
                int len = questRewards.Count;
                for (var num = 0; num < len; num++)
                {
                    var questData = questRewards[num].Quest;
                    var questProgress = userQuestData.Data.Find(x => (x.QuestId == questData.QuestId));
                    if ((questProgress != null) && (lastIdx == -1))
                    {
                        if (questProgress.Completed)
                        {
                            completedFound = true;
                            completedIdx = num;
                        } else if (completedFound)
                        {
                            lastIdx = num;
                        }
                    }
                }
                if (lastIdx == -1) lastIdx = completedIdx;

                var moveNext = true;
                for (var num = 0; num < len; num++)
                {
                    var questData = questRewards[num].Quest;
                    var userQuest = new PlayerQuestDataTable()
                    {
                        QuestId = questData.QuestId
                    };
                    var questProgress = userQuestData.Data.Find(x => (x.QuestId == questData.QuestId));
                    if (questProgress != null)
                    {
                        userQuest.QuestUserDataId = questProgress.QuestUserDataId;
                        userQuest.Completed = questProgress.Completed;
                        userQuest.Redeemed = questProgress.Redeemed;
                        userQuest.ProgressData = questProgress.ProgressData;
                    }

                    if (moveNext) moveNext = userQuest.Completed;
//                    if (!fullTree && !completed && (questProgress == null)) break;

                    userChapter.Quests.Add(userQuest);
//                    if (!fullTree && !completed) break;
                    if (!fullTree && (num == lastIdx)) break;
                }

                chapterQuestRels.Add(userChapter);
                if (!fullTree && !moveNext) break;
            }

            return new Response<(List<UserChapterQuestData>, List<PlayerQuestDataTable>)>((chapterQuestRels, userQuestData.Data), userQuestData.Case, userQuestData.Message);
        }

        public async Task<Response<PlayerQuestDataTable>> GetQuestProgress(int playerId, int questId)
        {
            return await Db.ExecuteSPSingleRow<PlayerQuestDataTable>("GetPlayerQuestData", new Dictionary<string, object>()
            {
                { "PlayerId", playerId },
                { "QuestId", questId }
            });
        }

        public async Task<Response<PlayerQuestDataTable>> UpdateQuestData(int playerId, PlayerQuestDataTable currentQuest)
        {
            return await UpdateQuestData(playerId, currentQuest.QuestId, currentQuest.Completed, currentQuest.ProgressData);
        }

        public async Task<Response<PlayerQuestDataTable>> UpdateQuestData(int playerId, int questId, bool isCompleted, string progress = null)
        {
            var dic = new Dictionary<string, object>()
            {
                { "PlayerId", playerId },
                { "QuestId", questId },
                { "IsCompleted", isCompleted }
            };
            if (progress != null) dic.Add("Data", progress);

            return await Db.ExecuteSPSingleRow<PlayerQuestDataTable>("AddOrUpdatePlayerQuestData", dic);
        }

/*        public async Task<Response<PlayerQuestDataTable>> UpdateQuestData<T>(int playerId, int questId, bool isCompleted, T progress) where T : IBaseQuestTemplateData
        {
            var dataString = string.Empty;
            if (progress != null) dataString = JsonConvert.SerializeObject(progress);

            return await UpdateQuestData(playerId, questId, isCompleted, dataString);
        }*/

        public async Task<Response<PlayerQuestDataTable>> UpdateQuestData(int playerId, int questId, bool isCompleted, object progress)
        {
            var dataString = string.Empty;

            if (progress != null)
            {
                var objType = progress.GetType();

                if (objType == typeof(string)) dataString = progress as string;
                else if (objType == typeof(int) || objType == typeof(long) || objType == typeof(float) || objType == typeof(double)) dataString = progress.ToString();
                else dataString = JsonConvert.SerializeObject(progress);
            }

            return await UpdateQuestData(playerId, questId, isCompleted, dataString);
        }

        public static async Task<ReturnCode> CollectRewards(int playerId, IReadOnlyList<IReadOnlyDataReward> rewards)
        {
            if ((rewards == null) || (rewards.Count == 0)) return ReturnCode.OK;

            var resp = await manager.GetAllPlayerData(playerId, DataType.Reward);
            if (!resp.IsSuccess || !resp.HasData) return ReturnCode.InvalidOperation;

            var noErrors = true;
            var userRewards = resp.Data;
            foreach (var reward in rewards)
            {
                var data = userRewards.Find(x => (x.ValueId == reward.RewardId));
                if (data != null)
                {
                    if (int.TryParse(data.Value, out int userCount))
                    {
                        userCount += reward.Count;
                        data.Value = userCount.ToString();
                        var saveResp = await manager.UpdatePlayerDataID(playerId, data.Id, data.Value);
                        if (!saveResp.IsSuccess) noErrors = false;
                    }
                    else
                    {
                        noErrors = false;
                    }
                }
                else
                {
                    var value = reward.Count.ToString();
                    var saveResp = await manager.AddOrUpdatePlayerData(playerId, DataType.Reward, reward.RewardId, value);
                    if (saveResp.IsSuccess && saveResp.HasData)
                    {
                        userRewards.Add(saveResp.Data.ToPlayerDataTable);
                    }
                    else
                    {
                        noErrors = false;
                    }
                }
            }
            //TODO: implement a way to rollback changes if an error occurs?

            return noErrors? ReturnCode.OK : ReturnCode.Failed;
        }

        public async Task<Response<PlayerDataTableUpdated>> ConsumeReward(int playerId, long playerDataId, string context = null)
        {
            var resp = await manager.GetPlayerDataById(playerDataId);
            if (!resp.IsSuccess || !resp.HasData) return new Response<PlayerDataTableUpdated>(200, "User reward not found");

            var playerData = resp.Data;
            var rewardId = playerData.ValueId;
            var rewardData = CacheQuestDataManager.AllQuestRewards
                            .SelectMany(x => x.Rewards).FirstOrDefault(y => (y.RewardId == rewardId));

            if (rewardData == null) return new Response<PlayerDataTableUpdated>(201, "Reward data not found");
            if (rewardData.Count < 1) return new Response<PlayerDataTableUpdated>(202, "Reward data invalid");

            if (int.TryParse(playerData.Value, out int count))
            {
                if (count > 0)
                {
                    count--;
                    try
                    {
                        long kingDetailsId = 0;
                        UserKingDetails kingDetails = null;
                        if ((rewardData.DataType == DataType.Custom) && (rewardData.ValueId < (int)CustomRewardType.VIPPoints))
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
                        switch (rewardData.DataType)
                        {
                            case DataType.Custom:
                                switch ((CustomRewardType)rewardData.ValueId)
                                {
                                    case CustomRewardType.KingExperiencePoints:
                                        kingDetails.Experience += rewardData.Value;
                                        updateKing = true;
                                        break;
                                    case CustomRewardType.KingStaminaPoints:
                                        kingDetails.MaxStamina += rewardData.Value;
                                        updateKing = true;
                                        break;
                                    case CustomRewardType.VIPPoints:
                                        var vipresp = await manager.GetPlayerData(playerId, DataType.Custom, (int)CustomValueType.VIPPoints);
                                        if (!vipresp.IsSuccess) throw new InvalidModelExecption(vipresp.Message);

                                        var vipdata = vipresp.Data;
                                        if (vipdata == null) throw new InvalidModelExecption("VIP data missing");

                                        var vipdetails = JsonConvert.DeserializeObject<UserVIPDetails>(vipdata.Value);
                                        vipdetails.Points += rewardData.Value;
                                        var json = JsonConvert.SerializeObject(vipdetails);
                                        var saveResp = await manager.UpdatePlayerDataID(playerId, vipdata.Id, json);
                                        if (!saveResp.IsSuccess) throw new InvalidModelExecption(saveResp.Message);
                                        break;
                                    case CustomRewardType.HeroPoints: //hero points
                                        var heroResp = await userHeroManager.AddHeroPoints(playerId, context, rewardData.Value);
                                        if (!heroResp.IsSuccess) throw new InvalidModelExecption(heroResp.Message);
                                        break;
                                    case CustomRewardType.VIPActivate:
                                        var vipActivateResp = await boostManager.ActivateVIPBoosts(playerId, rewardData.Value);
                                        if (!vipActivateResp.IsSuccess) throw new InvalidModelExecption(vipActivateResp.Message);
                                        break;
                                }
                                
                                break;
                            case DataType.Resource:
                                var sumResp = await resmanager.SumResource(playerId, rewardData.ValueId, rewardData.Value);
                                if (!sumResp.IsSuccess) throw new InvalidModelExecption(sumResp.Message);
                                break;
                            case DataType.Technology:
                                int location = 0;
                                TroopType troopType = TroopType.Other;
                                List<TroopDetails> listTroops = null;
                                Response<PlayerCompleteData> fullPlayerData;
                                switch ((NewBoostTech)rewardData.ValueId)
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

                                        troopTraining.Duration -= rewardData.Value;
                                        if (troopTraining.Duration < 0)
                                        {
                                            listInTraining.Remove(troopTraining);
                                        }
                                        await userTroopManager.UpdateTroops(playerId, troopType, listTroops);
                                        break;

                                    case NewBoostTech.TroopRecoverySpeedMultiplier:/*15*/
                                        var boostResp2 = await boostManager.ActivateBoost(playerId, CityBoostType.LifeSaver, rewardData.Value, 0);
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

                                        troopRecovery.Duration -= rewardData.Value;
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
                                        var timeLeft = returning? marchingArmy.TimeLeft : marchingArmy.TimeLeftForTask;
                                        if (timeLeft < 5) throw new InvalidModelExecption("Consume reward is not required");

                                        var percentage = (rewardData.Value > 100) ? 100 : rewardData.Value;
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

                                    case NewBoostTech.ResearchSpeedMultiplier:/*10*/
                                        var boostResp3 = await boostManager.ActivateBoost(playerId, CityBoostType.TechBoost, rewardData.Value, 0);
                                        if (!boostResp3.IsSuccess) throw new InvalidModelExecption(boostResp3.Message);
                                        break;
                                    case NewBoostTech.ResearchTimeBonus:/*21*/
                                        int.TryParse(context, out location);
                                        if (location <= 0) throw new InvalidModelExecption("Invalid building location");

                                        fullPlayerData = await BaseUserDataManager.GetFullPlayerData(playerId);
                                        if (!fullPlayerData.IsSuccess) throw new InvalidModelExecption(fullPlayerData.Message);

                                        var userTech = fullPlayerData.Data.Technologies.Find(x => (x.TimeLeft > 0));
                                        if (userTech == null) throw new InvalidModelExecption("There are no active researches");

                                        userTech.Duration -= rewardData.Value;
                                        if (userTech.Duration < 0) userTech.Duration = 0;

                                        var json = JsonConvert.SerializeObject(userTech);
                                        var tech = CacheTechnologyDataManager.GetFullTechnologyData(userTech.TechnologyType);
                                        await manager.AddOrUpdatePlayerData(playerId, DataType.Technology, tech.Info.Id, json);
                                        break;

//                                    case NewBoostTech.BuildingSpeedMultiplier:/*7*/
                                    case NewBoostTech.BuildingTimeBonus:/*20*/
                                        int.TryParse(context, out location);
                                        if (location <= 0) throw new InvalidModelExecption("Invalid Building location");

                                        var speedupResp = await userStructureManager.SpeedupBuilding(playerId, location, rewardData.Value);
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


        public async Task CheckQuestProgressForCollectResourceAsync(PlayerUserQuestData playerData, ResourceType resourceType, int count)
        {
            if ((count < 1) || (resourceType == ResourceType.Other) || (playerData == null)) return;

            log.Info("CHECK QUESTS RESOURCE START type="+resourceType.ToString()+" count="+count);
            var questsInProgress = GetQuestsInProgress(playerData.QuestData, QuestType.ResourceCollection);
            foreach (var quest in questsInProgress)
            {
                var initialData = JsonConvert.DeserializeObject<QuestResourceData>(quest.InitialData);
                if ((initialData == null) || (initialData.ResourceType != resourceType)) continue;

                QuestResourceData progressData;
                if (!string.IsNullOrEmpty(quest.ProgressData))
                {
                    progressData = JsonConvert.DeserializeObject<QuestResourceData>(quest.ProgressData);
                }
                else
                {
                    progressData = new QuestResourceData(initialData);
                    progressData.Count = 0;
                }
                progressData.Count += count;
                if (progressData.Count >= initialData.Count)
                {
                    progressData.Count = initialData.Count;
//                    quest.Completed = true;
                    await CheckQuestProgressResourceCollectionAsync(playerData, quest, initialData, progressData);
                }
                else
                {
                    var progress = JsonConvert.SerializeObject(progressData);
                    if (quest.UserData != null)
                    {
                        quest.UserData.ProgressData = progress;
//                        CheckQuestProgress(data , quest);
                        await UpdateQuestDataAsync(playerData, quest.UserData);
                    }
                    else
                    {
                        await SaveQuestDataAsync(playerData, quest.QuestId, progress);
                    }
                }
            }
            log.Info("CHECK QUESTS RESOURCE END");
        }

        public async Task CheckQuestProgressForTrainTroops(PlayerUserQuestData playerData, TroopType troopType, int level, int count)
        {
            if ((count < 1) || (troopType == TroopType.Other) || (playerData == null)) return;

            log.Info("CHECK QUESTS TRAIN START type="+troopType.ToString()+" lvl="+level+" count="+count);
            var questsInProgress = GetQuestsInProgress(playerData.QuestData, QuestType.TrainTroops);
            foreach (var quest in questsInProgress)
            {
                var initialData = JsonConvert.DeserializeObject<QuestTroopData>(quest.InitialData);
                if (initialData == null) continue;
                if ((initialData.TroopType != TroopType.Other) && (initialData.TroopType != troopType)) continue;

                if ((initialData.Level == 0) || (level == initialData.Level))
                {
                    QuestTroopData progressData;
                    if (!string.IsNullOrEmpty(quest.ProgressData))
                    {
                        progressData = JsonConvert.DeserializeObject<QuestTroopData>(quest.ProgressData);
                    }
                    else
                    {
                        progressData = new QuestTroopData(initialData);
                        progressData.Count = 0;
                    }
                    progressData.Count += count;
                    if (progressData.Count >= initialData.Count)
                    {
                        progressData.Count = initialData.Count;
//                        quest.Completed = true;
                        await CheckQuestProgressTrainTroopsAsync(playerData, quest, initialData, progressData);
                    }
                    else
                    {
                        var progress = JsonConvert.SerializeObject(progressData);
                        if (quest.UserData != null)
                        {
                            quest.UserData.ProgressData = progress;
//                            CheckQuestProgress(data , quest);
                            await UpdateQuestDataAsync(playerData, quest.UserData);
                        }
                        else
                        {
                            await SaveQuestDataAsync(playerData, quest.QuestId, progress);
                        }
                    }
                }
            }
            log.Info("CHECK QUESTS TRAIN END");
        }

        public async Task CheckQuestProgressForGroupTechnologyAsync(PlayerUserQuestData playerData, GroupTechnologyType groupTechnologyType)
        {
            if (playerData == null) return;

            log.Info("CHECK QUESTS TECH START type="+groupTechnologyType.ToString());
            var questsInProgress = GetQuestsInProgress(playerData.QuestData, QuestType.ResearchTechnology);
            foreach (var quest in questsInProgress)
            {
                var initialData = JsonConvert.DeserializeObject<QuestGroupTechnologyData>(quest.InitialData);
                if ((initialData == null) || (initialData.GroupTechnologyType != groupTechnologyType)) return;

//                if ((initialData.Level == 0) || (level == initialData.Level))
                {
                    QuestGroupTechnologyData progressData;
                    if (!string.IsNullOrEmpty(quest.ProgressData))
                    {
                        progressData = JsonConvert.DeserializeObject<QuestGroupTechnologyData>(quest.ProgressData);
                    }
                    else
                    {
                        progressData = new QuestGroupTechnologyData(initialData);
                        progressData.Count = 0;
                    }
                    progressData.Count++;
                    if (progressData.Count >= initialData.Count)
                    {
                        progressData.Count = initialData.Count;
//                        quest.Completed = true;
                        await CheckQuestProgressGroupTechnologyAsync(playerData, quest, initialData, progressData);
                    }
                    else
                    {
                        var progress = JsonConvert.SerializeObject(progressData);
                        if (quest.UserData != null)
                        {
                            quest.UserData.ProgressData = progress;
//                            CheckQuestProgress(data , quest);
                            await UpdateQuestDataAsync(playerData, quest.UserData);
                        }
                        else
                        {
                            await SaveQuestDataAsync(playerData, quest.QuestId, progress);
                        }
                    }
                }
            }
            log.Info("CHECK QUESTS TECH END");
        }

        public List<UserQuestProgressData> GetQuestsInProgress(UserChapterAllQuestProgress userQuests, QuestType questType)
        {
            var list = new List<UserQuestProgressData>();

            //check current milestone quest
            var chapQuests = CacheQuestDataManager.ChapterQuests;
            foreach (var chapterData in chapQuests)
            {
                UserChapterQuestData userChap = userQuests.ChapterQuests.Find(x => (x.ChapterId == chapterData.Chapter.ChapterId));
                if (userChap == null)
                {
                    userChap = new UserChapterQuestData()
                    {
                        ChapterId = chapterData.Chapter.ChapterId
                    };
//                    userChap.Quests;// = new List<PlayerQuestDataTable>();
                }
                userChap.TotalQuests = chapterData.Quests.Count;
                if (userChap.AllQuestsCompleted) continue;

                log.Info("user chapter inclomplete = " + userChap.ChapterId+"  quests="+ chapterData.Quests.Count);
                foreach (var quest in chapterData.Quests)
                {
                    PlayerQuestDataTable userQuest = null;
                    if (userChap.Quests != null)
                    {
                        userQuest = userChap.Quests.Find(x => (x.QuestId == quest.Quest.QuestId));
                    }
                    if ((userQuest != null) && userQuest.Completed) continue;

                    log.Info("in progress = " + quest.Quest.QuestId + "  " + quest.Quest.DataString);
                    list.Add(new UserQuestProgressData(quest.Quest, userQuest));
                    break;
                }
                break;
            }


            //check daily quest
            foreach (var questData in CacheQuestDataManager.DailyQuests)
            {
                if (questData.Quest.QuestType != questType) continue;

                var userQuest = userQuests.DailyQuests.Find(x => (x.QuestId == questData.Quest.QuestId));
                if ((userQuest != null) && userQuest.Completed) continue;

                list.Add(new UserQuestProgressData(questData.Quest, userQuest));
            }

            return list;
        }


        private async Task CheckQuestProgress(PlayerUserQuestData data, List<PlayerQuestDataTable> quests)
        {
            var showLog = true;// data.UserData.IsAdmin;

            var allQuests = CacheQuestDataManager.AllQuestRewards;
            int idx = 0;
            foreach (var userQuest in quests)
            {
                idx++;
                if (showLog) System.Console.WriteLine(idx + "  " + userQuest.QuestId + "   c:" + userQuest.Completed + "   " + userQuest.ProgressData);
                if (!userQuest.Completed)
                {
                    var questData = allQuests.FirstOrDefault(x => (x.Quest.QuestId == userQuest.QuestId));
                    if (questData != null)
                    {
                        await CheckQuestProgressAsync(data, new UserQuestProgressData(questData.Quest, userQuest));
                    }
                }
            }
        }

        public async Task CheckPlayerQuestDataAsync(PlayerUserQuestData data)
        {
            var showLog = true;// data.UserData.IsAdmin;

            //check daily quest
            await CheckQuestProgress(data, data.QuestData.DailyQuests);



            //check current milestone quest
            if (showLog) log.Info("--chapter quests for " + data.PlayerId);
            var currChapterQuest = data.QuestData.ChapterQuests.Find(x => !x.AllQuestsCompleted);
            if (currChapterQuest != null)
            {
                var currQuest = currChapterQuest.Quests.Find(x => !x.Completed);
                if (currQuest != null)
                {
                    var questData = CacheQuestDataManager.AllQuestRewards.FirstOrDefault(x => (x.Quest.QuestId == currQuest.QuestId));
                    if (questData != null)
                    {
                        await CheckQuestProgressAsync(data, new UserQuestProgressData(questData.Quest, currQuest));
                    }
                }
            }
            if (showLog) log.Info("----");
        }



        public async Task CheckQuestProgressAsync(PlayerUserQuestData data, UserQuestProgressData currentQuest)
        {
            var showLog = true;// data.UserData.IsAdmin;

            switch (currentQuest.QuestType)
            {
                case QuestType.BuildingUpgrade: await CheckQuestProgressBuildingUpgradeAsync(data, currentQuest); break;
                case QuestType.XBuildingCount: await CheckQuestProgressXBuildingCountAsync(data, currentQuest); break;
                case QuestType.XTroopCount: await CheckQuestProgressXTroopCountAsync(data, currentQuest); break;
                case QuestType.Alliance: await CheckQuestProgressAllianceAsync(data, currentQuest); break;
//                case QuestType.ResourceCollection: CheckQuestProgressResourceCollection(data, currentQuest); break;
//                case QuestType.TrainTroops: CheckQuestProgressTrainTroops(data, currentQuest); break;
            }
            if (showLog) System.Console.WriteLine("end chk");
        }

        private async Task CheckQuestProgressAllianceAsync(PlayerUserQuestData data, UserQuestProgressData quest)
        {
            var showLog = true;// data.UserData.IsAdmin;

            if (showLog) System.Console.WriteLine("check alliance join");
            try
            {
                var questData = CacheQuestDataManager.AllQuestRewards.FirstOrDefault(x => (x.Quest.QuestId == quest.QuestId));
                var initialData = JsonConvert.DeserializeObject<QuestAllianceData>(questData.Quest.DataString);

                bool completed = false;
/*                if (!string.IsNullOrEmpty(quest.ProgressData))
                {
                    var progressData = JsonConvert.DeserializeObject<QuestAllianceData>(quest.ProgressData);
                    completed = (progressData != null);// && (progressData.Level >= initialData.Level);
                }*/

                if (!completed)
                {
                    if (initialData.AllianceTaskType == AllianceTaskType.JoinOrCreate)
                    {
                        completed = data.UserData.ClanId != 0;
                    }
                }

                if (completed && !quest.Completed)
                {
                    quest.UserData.ProgressData = questData.Quest.DataString;
                    quest.UserData.Completed = true;

                    await UpdateQuestDataAsync(data, quest.UserData);
                }
            }
            catch { }
        }

        private async Task CheckQuestProgressBuildingUpgradeAsync(PlayerUserQuestData data, UserQuestProgressData quest)
        {
            var showLog = true;// data.UserData.IsAdmin;

            if (showLog) System.Console.WriteLine("check building upgrade");
            try
            {
                var questData = CacheQuestDataManager.AllQuestRewards.FirstOrDefault(x => (x.Quest.QuestId == quest.QuestId));
                var initialData = JsonConvert.DeserializeObject<QuestBuildingData>(questData.Quest.DataString);

                bool completed = false;
                if (!string.IsNullOrEmpty(quest.ProgressData))
                {
                    var progressData = JsonConvert.DeserializeObject<QuestBuildingData>(quest.ProgressData);
                    completed = (progressData != null) && (progressData.Level >= initialData.Level);
                }

                if (!completed)
                {
                    var fieldBuildings = 50;//location used for field buildings outside castle
                    var bld = data.UserData.Structures.Find(x => (x.StructureType == initialData.StructureType))?
                                    .Buildings.Find(x => (x.Location < fieldBuildings) && (x.CurrentLevel >= initialData.Level));
                    completed = (bld != null);
                }

                if (completed && !quest.Completed)
                {
                    quest.UserData.ProgressData = questData.Quest.DataString;
                    quest.UserData.Completed = true;

                    await UpdateQuestDataAsync(data, quest.UserData);
                }
            }
            catch { }
        }

        private async Task CheckQuestProgressXBuildingCountAsync(PlayerUserQuestData data, UserQuestProgressData quest)
        {
            var showLog = true;// data.UserData.IsAdmin;

            if (showLog) System.Console.WriteLine("check building count");
            try
            {
                var initialData = JsonConvert.DeserializeObject<QuestBuildingData>(quest.InitialData);
                QuestBuildingData progressData = null;
                bool completed = false;
                if (!string.IsNullOrEmpty(quest.ProgressData))
                {
                    progressData = JsonConvert.DeserializeObject<QuestBuildingData>(quest.ProgressData);
                    completed = (progressData != null) && (progressData.Count >= initialData.Count);
                }

                if (!completed)
                {
                    var blds = data.UserData.Structures.Find(x => (x.StructureType == initialData.StructureType));
                    if (blds != null)
                    {
                        var fieldBuildings = 50;//location used for field buildings outside castle
                        var count = blds.Buildings.Sum(x => (x.Location < fieldBuildings) && (x.CurrentLevel >= 1)? 1 : 0);

                        if (count >= initialData.Count)
                        {
                            completed = true;
                        }
                        else if ((progressData == null) || (count > progressData.Count))
                        {
                            progressData = initialData;
                            progressData.Count = count;
                            quest.UserData.ProgressData = JsonConvert.SerializeObject(progressData);

                            await UpdateQuestDataAsync(data, quest.UserData);
                        }
                    }
                }
                if (completed && !quest.Completed)
                {
                    progressData = initialData;
                    progressData.Count = 0;
                    quest.UserData.ProgressData = JsonConvert.SerializeObject(progressData);
                    quest.UserData.Completed = true;

                    await UpdateQuestDataAsync(data, quest.UserData);
                }
            }
            catch { }
        }

        public async Task CheckQuestProgressResourceCollectionAsync(PlayerUserQuestData data, UserQuestProgressData quest, QuestResourceData initialData = null, QuestResourceData progressData = null)
        {
            var showLog = true;// data.UserData.IsAdmin;

            if (showLog) System.Console.WriteLine("check resource collection");
//            if (string.IsNullOrEmpty(quest.ProgressData)) return;

            try
            {
                if (progressData == null) progressData = JsonConvert.DeserializeObject<QuestResourceData>(quest.ProgressData);
                if (progressData != null)
                {
                    if (initialData == null) initialData = JsonConvert.DeserializeObject<QuestResourceData>(quest.InitialData);
                    if (progressData.Count >= initialData.Count)
                    {
                        if (progressData.Iteration > 0) progressData.Iteration--;
                        progressData.Count = 0;//initialData3.Count;

                        var progress = JsonConvert.SerializeObject(progressData);
                        if (quest.UserData != null)
                        {
                            quest.UserData.ProgressData = progress;
                            quest.UserData.Completed = true;

                            await UpdateQuestDataAsync(data, quest.UserData);
                        }
                        else
                        {
                            await SaveQuestDataAsync(data, quest.QuestId, progress);
                        }
                    }
                }
            }
            catch { }
        }

        private async Task CheckQuestProgressXTroopCountAsync(PlayerUserQuestData data, UserQuestProgressData quest)
        {
            var showLog = true;// data.UserData.IsAdmin;

            if (showLog) System.Console.WriteLine("check troop count");
            try
            {
                var initialData = JsonConvert.DeserializeObject<QuestTroopData>(quest.InitialData);
                QuestTroopData progressData = null;
                bool completed = false;
                if (!string.IsNullOrEmpty(quest.ProgressData))
                {
                    progressData = JsonConvert.DeserializeObject<QuestTroopData>(quest.ProgressData);
                    completed = (progressData != null) && (progressData.Count >= initialData.Count);
                }

                if (!completed)
                {
                    var troops = data.UserData.Troops.Find(x => (x.TroopType == initialData.TroopType));
                    if (troops != null)
                    {
                        var count = troops.TroopData.Sum(x => x.FinalCount);

                        if (count >= initialData.Count)
                        {
                            completed = true;
                        }
                        else if ((progressData == null) || (count > progressData.Count))
                        {
                            progressData = initialData;
                            progressData.Count = count;
                            quest.UserData.ProgressData = JsonConvert.SerializeObject(progressData);

                            await UpdateQuestDataAsync(data, quest.UserData);
                        }
                    }
                }
                if (completed && !quest.Completed)
                {
                    if (progressData.Iteration > 0) progressData.Iteration--;
                    progressData.Count = 0;
                    quest.UserData.ProgressData = JsonConvert.SerializeObject(progressData);
                    quest.UserData.Completed = true;

                    await UpdateQuestDataAsync(data, quest.UserData);
                }
            }
            catch { }
        }

        private async Task CheckQuestProgressTrainTroopsAsync(PlayerUserQuestData data, UserQuestProgressData quest, QuestTroopData initialData = null, QuestTroopData progressData = null)
        {
            var showLog = true;// data.UserData.IsAdmin;

            if (showLog) System.Console.WriteLine("check train troops");
//            if (string.IsNullOrEmpty(quest.ProgressData)) return;

            try
            {
                if (progressData == null) progressData = JsonConvert.DeserializeObject<QuestTroopData>(quest.ProgressData);
                if (progressData != null)
                {
                    if (initialData == null) initialData = JsonConvert.DeserializeObject<QuestTroopData>(quest.InitialData);
                    if ((progressData.Count >= initialData.Count) &&
                        ((initialData.Level == 0) || (progressData.Level == initialData.Level)))
                    {
                        //                            currentQuest.ProgressData = currentQuest.InitialData;
                        if (progressData.Iteration > 0) progressData.Iteration--;
                        progressData.Count = 0;

                        var progress = JsonConvert.SerializeObject(progressData);
                        if (quest.UserData != null)
                        {
                            quest.UserData.ProgressData = progress;
                            quest.UserData.Completed = true;

                            await UpdateQuestDataAsync(data, quest.UserData);
                        }
                        else
                        {
                            await SaveQuestDataAsync(data, quest.QuestId, progress);
                        }
                    }
                }
            }
            catch { }
        }

        private async Task CheckQuestProgressGroupTechnologyAsync(PlayerUserQuestData data, UserQuestProgressData quest, QuestGroupTechnologyData initialData = null, QuestGroupTechnologyData progressData = null)
        {
            var showLog = true;// data.UserData.IsAdmin;

            if (showLog) System.Console.WriteLine("check group technology");
//            if (string.IsNullOrEmpty(quest.ProgressData)) return;

            try
            {
                if (progressData == null) progressData = JsonConvert.DeserializeObject<QuestGroupTechnologyData>(quest.ProgressData);
                if (progressData != null)
                {
                    if (initialData == null) initialData = JsonConvert.DeserializeObject<QuestGroupTechnologyData>(quest.InitialData);
                    if (progressData.Count >= initialData.Count)
                    {
                        //                            currentQuest.ProgressData = currentQuest.InitialData;
//                        if (progressData.Iteration > 0) progressData.Iteration--;
                        progressData.Count = 0;

                        var progress = JsonConvert.SerializeObject(progressData);
                        if (quest.UserData != null)
                        {
                            quest.UserData.ProgressData = progress;
                            quest.UserData.Completed = true;

                            await UpdateQuestDataAsync(data, quest.UserData);
                        }
                        else
                        {
                            await SaveQuestDataAsync(data, quest.QuestId, progress);
                        }
                    }
                }
            }
            catch { }
        }

        private async Task UpdateQuestDataAsync(PlayerUserQuestData data, PlayerQuestDataTable quest)
        {
            var resp = await UpdateQuestData(data.PlayerId, quest);
            if (resp.IsSuccess && resp.HasData) data.QuestEventAction?.Invoke(quest);
        }

        private async Task SaveQuestDataAsync(PlayerUserQuestData data, int questId, string progressData)
        {
            var resp = await UpdateQuestData(data.PlayerId, questId, false, progressData);
            if (resp.IsSuccess && resp.HasData) data.QuestEventAction?.Invoke(resp.Data);
        }
    }
}

using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Quest;
using GameOfRevenge.Common.Models.Quest.Template;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameOfRevenge.Business.Manager.UserData
{
    public class UserQuestManager : BaseManager, IUserQuestManager
    {
        private readonly UserResourceManager resmanager = new UserResourceManager();
        protected static readonly IPlayerDataManager manager = new PlayerDataManager();

        public async Task<Response> ResetAllDailyQuests()
        {
            return await Db.ExecuteSPNoData("ResetAllDailyQuests", new Dictionary<string, object>());
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

            var questRewards = CacheData.CacheQuestDataManager.GetQuestData(questData.QuestId);
            await GiveRewards(playerId, questRewards.Rewards); //TODO:implement response error

            QuestResourceData quest = null;
            if (questRewards.Quest.MilestoneId == 0)
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

            if ((quest == null) || (quest.Iteration < 1))
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
            var chapterProgress = await GetUserAllChapterQuestProgress(playerId);
            if (!chapterProgress.IsSuccess || !chapterProgress.HasData)
            {
                return new Response(200, "Chapter not completed");
            }

            var chapterData = chapterProgress.Data.Find(x => (x.ChapterId == chapterId));
            if (chapterData == null) return new Response(200, "Chapter not found");

            if (!chapterData.Completed()) return new Response(200, "Chapter not completed");
            if (chapterData.Redeemed) return new Response(201, "Quest reward already redemeed");

            //var chapterProgress = await GetAllChapterProgress(playerId);
            //if (!chapterProgress.IsSuccess || !chapterProgress.HasData) return new Response(chapterProgress.Case, chapterProgress.Message);
            //var chapterData = chapterProgress.Data.FirstOrDefault(x => x.ChapterId == chapterId);
            //if (chapterData == null) return new Response(200, "Chapter not completed");
            //if (chapterData.Redemeed) return new Response(201, "Quest reward already redemeed");

            //var questProgress = await GetAllQuestProgress(playerId);
            //if (!questProgress.IsSuccess || !questProgress.HasData) return new Response(questProgress.Case, questProgress.Message);
            //foreach (var quest in questProgress.Data) if (!quest.Completed) return new Response(200, "Chapter not completed");

            var questRewards = CacheData.CacheQuestDataManager.GetFullChapterData(chapterId);
            await GiveRewards(playerId, questRewards.Rewards); //TODO:implement response error

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
            var allQuestRewards = CacheQuestDataManager.AllQuestRewards;
            var allChapterQuests = CacheQuestDataManager.ChapterQuests;
            var allSideQuests = CacheQuestDataManager.SideQuests;
            var allDailyQuests = CacheQuestDataManager.DailyQuests;

            var allUserQuestProgress = await GetAllQuestProgress(playerId);
            var userChapterQuests = await GetUserAllChapterQuestProgress(playerId, fullTree);

            var sideQuests = new List<UserQuestProgressData>();
            var dailyQuests = new List<UserQuestProgressData>();
            foreach (var questReward in allQuestRewards)
            {
                var questData = questReward.Quest;
                var chapterQuest = allChapterQuests.FirstOrDefault(x =>
                {
                    return (x.Quests.FirstOrDefault(y => (y.Quest.QuestId == questData.QuestId)) != null);
                });
                if (chapterQuest != null) continue;

                var sideQuest = allSideQuests.FirstOrDefault(x => (x.Quest.QuestId == questData.QuestId));
                if (sideQuest != null)
                {
                    UserQuestProgressData questProgress = null;
                    var userQuest = allUserQuestProgress.Data.Find(x => x.QuestId == questData.QuestId);
                    if (fullTree || (userQuest != null))
                    {
                        questProgress = new UserQuestProgressData
                        {
                            QuestId = questData.QuestId,
                            QuestType = questData.QuestType,
                            MilestoneId = questData.MilestoneId,
                            InitialData = questData.DataString
                        };
                        if (userQuest != null)
                        {
                            questProgress.QuestUserDataId = userQuest.QuestUserDataId;
                            questProgress.Completed = userQuest.Completed;
                            questProgress.ProgressData = userQuest.ProgressData;
                            questProgress.Redeemed = userQuest.Redeemed;
                            questProgress.QuestId = userQuest.QuestId;
                        }
                        sideQuests.Add(questProgress);
                    }
                }
                var dailyQuest = allDailyQuests.FirstOrDefault(x => (x.Quest.QuestId == questData.QuestId));
                if (dailyQuest != null)
                {
                    UserQuestProgressData questProgress = null;
                    var userQuest = allUserQuestProgress.Data.Find(x => x.QuestId == questData.QuestId);
                    if (fullTree || (userQuest != null))
                    {
                        questProgress = new UserQuestProgressData
                        {
                            QuestId = questData.QuestId,
                            QuestType = questData.QuestType,
                            MilestoneId = questData.MilestoneId,
                            InitialData = questData.DataString
                        };
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
                ChapterQuests = userChapterQuests.Data,
                SideQuests = sideQuests,
                DailyQuests = dailyQuests
            };

            return new Response<UserChapterAllQuestProgress>(chapterQuestRels, userChapterQuests.Case, userChapterQuests.Message);
        }

        public async Task<Response<List<UserChapterQuestData>>> GetUserAllChapterQuestProgress(int playerId, bool fullTree = false)
        {
            var userChapterRedeemed = await GetAllChapterRedeemed(playerId);
            var userQuestData = await GetAllQuestProgress(playerId);

            var chapterQuestRels = new List<UserChapterQuestData>();
            foreach (var chapterQuests in CacheQuestDataManager.ChapterQuests)
            {
                var chapter = chapterQuests.Chapter;
                var questRewards = chapterQuests.Quests;

                var userChapter = new UserChapterQuestData()
                {
                    ChapterId = chapter.ChapterId,
                    Code = chapter.Code,//obsolete
                    Name = chapter.Name,//obsolete
                    Description = chapter.Description,//obsolete
                    Order = chapter.Order,//obsolete
                    Quests = new List<UserQuestProgressData>()
                };
                var chapterRedeemed = userChapterRedeemed.Data.Find(x => (x.ChapterId == chapter.ChapterId));
                if (chapterRedeemed != null)
                {
                    userChapter.ChapterUserDataId = chapterRedeemed.ChapterUserDataId;
                    userChapter.Redeemed = chapterRedeemed.Redemeed;
                }

                foreach (var quest in questRewards)
                {
                    var questData = quest.Quest;
                    var userQuest = new UserQuestProgressData
                    {
                        MilestoneId = questData.MilestoneId,
                        QuestId = questData.QuestId,
                        QuestType = questData.QuestType,
                        InitialData = questData.DataString
//                        ProgressData = "{}",
//                        Completed = false
                    };
                    var questProgress = userQuestData.Data.Find(x => (x.QuestId == questData.QuestId));
                    if (questProgress != null)
                    {
                        userQuest.QuestUserDataId = questProgress.QuestUserDataId;
                        userQuest.Completed = questProgress.Completed;
                        userQuest.ProgressData = questProgress.ProgressData;
                        userQuest.Redeemed = questProgress.Redeemed;
                    }

                    userChapter.Quests.Add(userQuest);
                }

                chapterQuestRels.Add(userChapter);
                if (!fullTree && !userChapter.Completed()) break;
            }

            return new Response<List<UserChapterQuestData>>(chapterQuestRels, userQuestData.Case, userQuestData.Message);
        }

        public async Task<Response<Common.Models.Quest.PlayerQuestDataTable>> GetQuestProgress(int playerId, int questId)
        {
            return await Db.ExecuteSPSingleRow<Common.Models.Quest.PlayerQuestDataTable>("GetPlayerQuestData", new Dictionary<string, object>()
            {
                { "PlayerId", playerId },
                { "QuestId", questId }
            });
        }

        public async Task<Response<Common.Models.Quest.PlayerQuestDataTable>> UpdateQuestData(int playerId, int questId, bool isCompleted, string progress = null)
        {
            var dic = new Dictionary<string, object>()
            {
                { "PlayerId", playerId },
                { "QuestId", questId },
                { "IsCompleted", isCompleted }
            };
            if (progress != null) dic.Add("Data", progress);

            return await Db.ExecuteSPSingleRow<Common.Models.Quest.PlayerQuestDataTable>("AddOrUpdatePlayerQuestData", dic);
        }

        public async Task<Response<Common.Models.Quest.PlayerQuestDataTable>> UpdateQuestData<T>(int playerId, int questId, bool isCompleted, T progress) where T : IBaseQuestTemplateData
        {
            var dataString = string.Empty;
            if (progress != null) dataString = JsonConvert.SerializeObject(progress);

            return await UpdateQuestData(playerId, questId, isCompleted, dataString);
        }

        public async Task<Response<Common.Models.Quest.PlayerQuestDataTable>> UpdateQuestData(int playerId, int questId, bool isCompleted, object progress)
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

        public async Task GiveRewards(int playerId, IReadOnlyList<IReadOnlyDataReward> rewards)
        {
            var kingRewards = rewards.Where(x => (x.DataType == Common.DataType.Reward)).ToList();
            if (kingRewards.Count > 0)
            {
                UserKingDetails kingDetails = null;
                var resp = await manager.GetPlayerData(playerId, DataType.Custom, 1);
                if (resp.IsSuccess && resp.HasData)
                {
                    try
                    {
                        kingDetails = JsonConvert.DeserializeObject<UserKingDetails>(resp.Data.Value);
                    }
                    catch { }
                }
                if (kingDetails != null)
                {
                    foreach (var reward in kingRewards)
                    {
                        switch (reward.ValueId)
                        {
                            case 1: kingDetails.Experience += reward.Value; break; //king exp
                            case 2: kingDetails.MaxStamina += reward.Value; break; //king stamina
                        }
                    }
                    var json = JsonConvert.SerializeObject(kingDetails);
                    await manager.UpdatePlayerDataID(playerId, resp.Data.Id, json);
                }
                else
                {
                    //error
                }
            }

            var resourceRewards = rewards.Where(x => (x.DataType == Common.DataType.Resource)).ToList();
            if (resourceRewards.Count > 0)
            {
                await resmanager.SumResourceByReward(playerId, resourceRewards);
            }
        }
    }
}

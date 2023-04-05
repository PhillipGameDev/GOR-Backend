using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Models.Quest;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Services;
using System;
using System.Linq;
using GameOfRevenge.Common.Models;

namespace GameOfRevenge.Business.Manager.GameDef
{
    public class QuestManager : BaseManager
    {
        public const int CHAPTER_QUEST = 0;
        public const int SIDE_QUEST = 99;
        public const int DAILY_QUEST = -1;
        public const int PRODUCT_PACK = 200;

        public async Task<Response<List<QuestTable>>> GetAllQuests() => await Db.ExecuteSPMultipleRow<QuestTable>("GetAllQuests");
        public async Task<Response<List<ChapterQuestTable>>> GetAllChapterQuests() => await Db.ExecuteSPMultipleRow<ChapterQuestTable>("GetAllChapterQuests");
        public async Task<Response<List<ChapterTable>>> GetAllChapters() => await Db.ExecuteSPMultipleRow<ChapterTable>("GetAllChapters");
//        public async Task<Response<List<DataReward>>> GetAllChapterRewards() => await Db.ExecuteSPMultipleRow<DataReward>("GetAllChapterRewards");
        public async Task<Response<List<DataReward>>> GetAllQuestRewards() => await Db.ExecuteSPMultipleRow<DataReward>("GetAllQuestRewards");

        public async Task<Response<List<ChapterQuestRelData>>> GetAllChapterQuestRelData()
        {
            try
            {
                var allChapters = await GetAllChapters();
                if (!allChapters.IsSuccess) return new Response<List<ChapterQuestRelData>>(allChapters.Case, allChapters.Message);

                var allChapterQuests = await GetAllChapterQuests();
                if (!allChapterQuests.IsSuccess) return new Response<List<ChapterQuestRelData>>(allChapterQuests.Case, allChapterQuests.Message);

//                var allChapterRewards = await GetAllChapterRewards();
//                if (!allChapterRewards.IsSuccess) return new Response<List<ChapterQuestRelData>>(allChapterRewards.Case, allChapterRewards.Message);

                var allQuests = await GetAllQuests();
                if (!allQuests.IsSuccess) return new Response<List<ChapterQuestRelData>>(allQuests.Case, allQuests.Message);

                var allQuestRewards = await GetAllQuestRewards();
                if (!allQuestRewards.IsSuccess) return new Response<List<ChapterQuestRelData>>(allQuestRewards.Case, allQuestRewards.Message);

                var response = new Response<List<ChapterQuestRelData>>(new List<ChapterQuestRelData>(), 100, allChapters.Message);

                foreach (var chapter in allChapters.Data)
                {
                    var chapterId = chapter.ChapterId;
                    var quests = new List<QuestRewardRelData>();
                    var chapterQuests = allChapterQuests.Data.Where(x => (x.ChapterId == chapterId)).ToList();
                    QuestRewardRelData chapterRewardRelData = null;// new QuestRewardRelData();
//                    chapterRewardRelData.Rewards = new List<DataReward>();//TODO: remove this on future builds, we should be able to send null data
                    foreach (var quest in chapterQuests)
                    {
                        var questId = quest.QuestId;
                        var questData = allQuests.Data.Find(x => (x.QuestId == questId));
                        if (questData == null) continue;

                        var questRewards = new QuestRewardRelData()
                        {
                            Quest = questData,
                            Rewards = allQuestRewards.Data.Where(x => (x.QuestId == questId)).ToList()
                        };

                        if ((questData.QuestType == QuestType.Custom) && (questData.MilestoneId == CHAPTER_QUEST))
                        {
                            chapterRewardRelData = questRewards;//chapter quest rewards
                        }
                        else
                        {
                            quests.Add(questRewards);
                        }
                    }
                    
                    response.Data.Add(new ChapterQuestRelData()
                    {
                        Chapter = chapter,
                        Quests = quests,
                        Rewards = ((chapterRewardRelData != null) && (chapterRewardRelData.Rewards != null)) ? chapterRewardRelData.Rewards : new List<DataReward>(),
                        QuestRewards = chapterRewardRelData
                    });
                }

                return response;
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<ChapterQuestRelData>>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<ChapterQuestRelData>>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<List<QuestRewardRelData>>> GetAllSideQuestRelData()
        {
            try
            {
                var allQuests = await GetAllQuests();
                if (!allQuests.IsSuccess) return new Response<List<QuestRewardRelData>>(allQuests.Case, allQuests.Message);

                var allQuestRewards = await GetAllQuestRewards();
                if (!allQuestRewards.IsSuccess) return new Response<List<QuestRewardRelData>>(allQuestRewards.Case, allQuestRewards.Message);

                var response = new Response<List<QuestRewardRelData>>(new List<QuestRewardRelData>(), 100, "All Side Quests");

                var sideQuests = allQuests.Data.Where(x => x.MilestoneId == SIDE_QUEST).ToList();
                foreach (var questData in sideQuests)
                {
                    var questId = questData.QuestId;
//                    var questData = allQuests.Data.Find(x => x.QuestId == questId);
                    if (questData == null) continue;

                    response.Data.Add(new QuestRewardRelData()
                    {
                        Quest = questData,
                        Rewards = allQuestRewards.Data.Where(x => x.QuestId == questId).ToList()
                    });
                }

                return response;
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<QuestRewardRelData>>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<QuestRewardRelData>>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<List<QuestRewardRelData>>> GetAllDailyQuestRelData()
        {
            try
            {
                var allQuests = await GetAllQuests();
                if (!allQuests.IsSuccess) return new Response<List<QuestRewardRelData>>(allQuests.Case, allQuests.Message);

                var allQuestRewards = await GetAllQuestRewards();
                if (!allQuestRewards.IsSuccess) return new Response<List<QuestRewardRelData>>(allQuestRewards.Case, allQuestRewards.Message);

                var response = new Response<List<QuestRewardRelData>>(new List<QuestRewardRelData>(), 100, "All Daily Quests");

                var dailyQuests = allQuests.Data.Where(x => x.MilestoneId == DAILY_QUEST).ToList();
                foreach (var questData in dailyQuests)
                {
                    var questId = questData.QuestId;
//                    var questData = allQuests.Data.Find(x => x.QuestId == questId);
                    if (questData == null) continue;

                    response.Data.Add(new QuestRewardRelData()
                    {
                        Quest = questData,
                        Rewards = allQuestRewards.Data.Where(x => x.QuestId == questId).ToList()
                    });
                }

                return response;
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<QuestRewardRelData>>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<QuestRewardRelData>>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<List<QuestRewardRelData>>> GetAllProductPackRelData()
        {
            try
            {
                var allQuests = await GetAllQuests();
                if (!allQuests.IsSuccess) return new Response<List<QuestRewardRelData>>(allQuests.Case, allQuests.Message);

                var allQuestRewards = await GetAllQuestRewards();
                if (!allQuestRewards.IsSuccess) return new Response<List<QuestRewardRelData>>(allQuestRewards.Case, allQuestRewards.Message);

                var productRewards = new List<QuestRewardRelData>();
                var productPacks = allQuests.Data.Where(x => (x.MilestoneId == PRODUCT_PACK)).ToList();
                foreach (var data in productPacks)
                {
                    if (data == null) continue;

                    var questId = data.QuestId;
                    productRewards.Add(new QuestRewardRelData()
                    {
                        Quest = data,
                        Rewards = allQuestRewards.Data.Where(x => (x.QuestId == questId)).ToList()
                    });
                }

                return new Response<List<QuestRewardRelData>>(productRewards, 100, "All Product Packs");
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<QuestRewardRelData>>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<QuestRewardRelData>>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }
    }
}

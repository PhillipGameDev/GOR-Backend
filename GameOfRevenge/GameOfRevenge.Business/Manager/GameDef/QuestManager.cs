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
        public async Task<Response<List<QuestTable>>> GetAllQuests() => await Db.ExecuteSPMultipleRow<QuestTable>("GetAllQuests");
        public async Task<Response<List<ChapterTable>>> GetAllChapters() => await Db.ExecuteSPMultipleRow<ChapterTable>("GetAllChapters");
        public async Task<Response<List<DataRequirement>>> GetAllChapterRewards() => await Db.ExecuteSPMultipleRow<DataRequirement>("GetAllChapterRewards");
        public async Task<Response<List<DataRequirement>>> GetAllQuestRewards() => await Db.ExecuteSPMultipleRow<DataRequirement>("GetAllQuestRewards");

        public async Task<Response<List<ChapterQuestRelData>>> GetAllChapterQuestRelData()
        {
            try
            {
                var allChapter = await GetAllChapters();
                if (!allChapter.IsSuccess) return new Response<List<ChapterQuestRelData>>(allChapter.Case, allChapter.Message);

                var allChapterRewards = await GetAllChapterRewards();
                if (!allChapterRewards.IsSuccess) return new Response<List<ChapterQuestRelData>>(allChapter.Case, allChapter.Message);

                var allQuest = await GetAllQuests();
                if (!allQuest.IsSuccess) return new Response<List<ChapterQuestRelData>>(allQuest.Case, allQuest.Message);

                var allQuestRewards = await GetAllQuestRewards();
                if (!allQuestRewards.IsSuccess) return new Response<List<ChapterQuestRelData>>(allQuest.Case, allQuest.Message);

                var response = new Response<List<ChapterQuestRelData>>(new List<ChapterQuestRelData>(), 100, allQuest.Message);

                foreach (var chapter in allChapter.Data)
                {
                    var chapData = new ChapterQuestRelData()
                    {
                        Chapter = chapter,
                        Quests = new List<QuestRewardRelData>(),
                        Rewards = allChapterRewards.Data.Where(x=>x.DataId == chapter.ChapterId).ToList()
                    };

                    foreach (var quest in allQuest.Data)
                    {
                        if (quest.ChapterId == chapter.ChapterId)
                        {
                            var questRewardRel = new QuestRewardRelData()
                            {
                                Quest = quest,
                                Rewards = allQuestRewards.Data.Where(x => x.DataId == quest.QuestId).ToList()
                            };

                            chapData.Quests.Add(questRewardRel);
                        }
                    }

                    response.Data.Add(chapData);
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
    }
}

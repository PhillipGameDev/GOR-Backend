using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Quest;
using GameOfRevenge.Common.Models.Quest.Template;
using GameOfRevenge.Common.Net;
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

        private async Task<Response<List<PlayerQuestData>>> GetAllQuestProgress(int playerId)
        {
            return await Db.ExecuteSPMultipleRow<PlayerQuestData>("GetPlayerAllQuestData", new Dictionary<string, object>()
            {
                { "PlayerId", playerId }
            });
        }

        private async Task<Response<List<PlayerChapterData>>> GetAllChapterProgress(int playerId)
        {
            return await Db.ExecuteSPMultipleRow<PlayerChapterData>("GetPlayerAllChapterData", new Dictionary<string, object>()
            {
                { "PlayerId", playerId }
            });
        }

        public async Task<Response> RedeemQuestReward(int playerId, int questId)
        {
            var questProgress = await GetAllQuestProgress(playerId);
            if (!questProgress.IsSuccess || !questProgress.HasData) return new Response(questProgress.Case, questProgress.Message);
            var questData = questProgress.Data.FirstOrDefault(x => x.QuestId == questId);
            if (questData == null || !questData.Completed) return new Response(200, "Quest not completed");
            if (questData.Redemeed) return new Response(201, "Quest reward already redemeed");

            var questRewards = CacheData.CacheQuestDataManager.GetQuestData(questData.QuestId);
            await GiveRewards(playerId, questRewards.Rewards);

            var redemeedResp = await Db.ExecuteSPNoData("RedeemQuestReward", new Dictionary<string, object>()
            {
                { "PlayerQuestUserId", questData.QuestUserDataId }
            });

            return redemeedResp;
        }

        public async Task<Response> RedeemChapterReward(int playerId, int chapterId)
        {
            var chapterProgress = await GetAllQuestChapterDataWithName(playerId);
            if (!chapterProgress.IsSuccess || !chapterProgress.HasData) return new Response(200, "Chapter not completed");
          
            var chapterData = chapterProgress.Data.FirstOrDefault(x=>x.ChapterId == chapterId);
            if (chapterData == null || !chapterData.Completed) return new Response(200, "Chapter not completed");
            if (chapterData.IsRedeemed) return new Response(201, "Quest reward already redemeed");

            //var chapterProgress = await GetAllChapterProgress(playerId);
            //if (!chapterProgress.IsSuccess || !chapterProgress.HasData) return new Response(chapterProgress.Case, chapterProgress.Message);
            //var chapterData = chapterProgress.Data.FirstOrDefault(x => x.ChapterId == chapterId);
            //if (chapterData == null) return new Response(200, "Chapter not completed");
            //if (chapterData.Redemeed) return new Response(201, "Quest reward already redemeed");

            //var questProgress = await GetAllQuestProgress(playerId);
            //if (!questProgress.IsSuccess || !questProgress.HasData) return new Response(questProgress.Case, questProgress.Message);
            //foreach (var quest in questProgress.Data) if (!quest.Completed) return new Response(200, "Chapter not completed");

            var questRewards = CacheData.CacheQuestDataManager.GetFullChapterData(chapterId);
            await GiveRewards(playerId, questRewards.Rewards);

            var redemeedResp = await Db.ExecuteSPNoData("RedeemChapterReward", new Dictionary<string, object>()
            {
                { "PlayerChapterUserId", chapterData.ChapterUserDataId }
            });

            return redemeedResp;
        }

        public async Task<Response<List<UserChapterQuestData>>> GetAllQuestChapterDataWithName(int playerId)
        {
            var userQuestData = await GetAllQuestProgress(playerId);
            var userChapterData = await GetAllChapterProgress(playerId);

            var chatpterQuestRels = CacheData.CacheQuestDataManager.UserChapterData;

            foreach (var chapter in chatpterQuestRels)
            {
                var chapterProgress = userChapterData.Data.FirstOrDefault(x => x.ChapterId == chapter.ChapterId);
                if (chapterProgress != null)
                {
                    chapter.ChapterUserDataId = chapterProgress.ChapterUserDataId;
                    chapter.IsRedeemed = chapterProgress.Redemeed;
                }

                foreach (var quest in chapter.Quests)
                {
                    foreach (var uquest in userQuestData.Data)
                    {
                        if (uquest.QuestId == quest.QuestId)
                        {
                            quest.QuestUserDataId = uquest.QuestUserDataId;
                            quest.Completed = uquest.Completed;
                            quest.ProgressData = uquest.ProgressData;
                            quest.IsRedeemed = uquest.Redemeed;
                        }
                    }
                }
            }

            return new Response<List<UserChapterQuestData>>(chatpterQuestRels, userQuestData.Case, userQuestData.Message);
        }

        public async Task<Response<PlayerQuestData>> GetQuestProgress(int playerId, int questId)
        {
            return await Db.ExecuteSPSingleRow<PlayerQuestData>("GetPlayerQuestData", new Dictionary<string, object>()
            {
                { "PlayerId", playerId },
                { "QuestId", questId }
            });
        }

        public async Task<Response<PlayerQuestData>> UpdateQuestData(int playerId, int questId, bool isCompleted, string progress)
        {
            return await Db.ExecuteSPSingleRow<PlayerQuestData>("AddOrUpdatePlayerQuestData", new Dictionary<string, object>()
            {
                { "PlayerId", playerId },
                { "QuestId", questId },
                { "IsCompleted", isCompleted },
                { "Data", progress }
            });
        }

        public async Task<Response<PlayerQuestData>> UpdateQuestData<T>(int playerId, int questId, bool isCompleted, T progress) where T : IBaseQuestTemplateData
        {
            var dataString = string.Empty;
            if (progress != null) dataString = JsonConvert.SerializeObject(progress);

            return await UpdateQuestData(playerId, questId, isCompleted, dataString);
        }

        public async Task<Response<PlayerQuestData>> UpdateQuestData(int playerId, int questId, bool isCompleted, object progress)
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

        public async Task GiveRewards(int playerId, IReadOnlyList<IReadOnlyDataRequirement> rewards)
        {
            await resmanager.RefundResourceByRequirement(playerId, rewards.Where(x => x.DataType == Common.DataType.Resource).ToList(), 1);
        }
    }
}

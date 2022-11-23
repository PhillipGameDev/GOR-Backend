﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameOfRevenge.Business.Manager;
using GameOfRevenge.Business.Manager.GameDef;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models.Quest;

namespace GameOfRevenge.Business.CacheData
{
    public static class CacheQuestDataManager
    {
        public const string QuestNotExist = "Quest item does not exist";

        private static bool isLoaded = false;
        private static List<ChapterQuestRelData> chapterQuests = null;
        private static List<QuestRewardRelData> sideQuests = null;
        private static List<QuestRewardRelData> dailyQuests = null;
        private static List<QuestRewardRelData> allQuestRewards = null;

        public static bool IsLoaded { get => isLoaded && (allQuestRewards != null); }
        public static IReadOnlyList<IReadOnlyChapterQuestRelData> ChapterQuests { get { CheckLoadCacheMemory(); return chapterQuests.ToList(); } }
        public static IReadOnlyList<IReadOnlyQuestRewardRelData> SideQuests { get { CheckLoadCacheMemory(); return sideQuests.ToList(); } }
        public static IReadOnlyList<IReadOnlyQuestRewardRelData> DailyQuests { get { CheckLoadCacheMemory(); return dailyQuests.ToList(); } }
        public static IReadOnlyList<IReadOnlyQuestRewardRelData> AllQuestRewards { get { CheckLoadCacheMemory(); return allQuestRewards.ToList(); } }

        public static IReadOnlyChapterQuestRelData GetFullChapterData(int id)
        {
            var item = ChapterQuests.FirstOrDefault(x => (x.Chapter.ChapterId == id));
            if (item == null) throw new CacheDataNotExistExecption(QuestNotExist);
            else return item;
        }

        public static IReadOnlyChapterQuestRelData GetFullChapterData(string code)
        {
            var item = ChapterQuests.FirstOrDefault(x => (x.Chapter.Code == code));
            if (item == null) throw new CacheDataNotExistExecption(QuestNotExist);
            else return item;
        }

        public static IReadOnlyQuestRewardRelData GetQuestData(int questId)
        {
            var questData = AllQuestRewards.FirstOrDefault(x => x.Quest.QuestId == questId);
            if (questData != null) return questData;

            throw new CacheDataNotExistExecption(QuestNotExist);
        }


        #region Cache Check, Load and Clear
        public static async Task LoadCacheMemoryAsync()
        {
            ClearCache();

            var questManager = new QuestManager();
            var response = await questManager.GetAllChapterQuestRelData();

            if (response.IsSuccess)
            {
                chapterQuests = response.Data;

                var response2 = await questManager.GetAllSideQuestRelData();
                if (response2.IsSuccess)
                {
                    sideQuests = response2.Data;

                    var response3 = await questManager.GetAllDailyQuestRelData();
                    if (response3.IsSuccess)
                    {
                        dailyQuests = response3.Data;

                        var all = new List<QuestRewardRelData>();
                        foreach (var chapterQuest in chapterQuests)
                        {
                            all.AddRange(chapterQuest.Quests);
                        }
                        all.AddRange(sideQuests);
                        all.AddRange(dailyQuests);
                        allQuestRewards = all;

                        isLoaded = true;
                    }
                }
            }

            if (!isLoaded)
            {
                ClearCache();
                throw new CacheDataNotExistExecption(response.Message);
            }
        }
        public static void LoadCacheMemory()
        {
            var tsk = LoadCacheMemoryAsync();
            tsk.Wait();
        }
        public static void CheckLoadCacheMemory()
        {
            if (isLoaded) return;
            else LoadCacheMemory();
        }
        public static async Task CheckLoadCacheMemoryAsync()
        {
            if (isLoaded) return;
            else await LoadCacheMemoryAsync();
        }
        public static void ClearCache()
        {
            isLoaded = false;

            if (chapterQuests != null)
            {
                chapterQuests.Clear();
                chapterQuests = null;
            }
            if (sideQuests != null)
            {
                sideQuests.Clear();
                sideQuests = null;
            }
            if (dailyQuests != null)
            {
                dailyQuests.Clear();
                dailyQuests = null;
            }
        }
        #endregion
    }
}

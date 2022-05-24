using System;
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
        private static List<ChapterQuestRelData> QuestInfo = null;

        public static List<UserChapterQuestData> UserChapterData
        {
            get
            {
                var lst = new List<UserChapterQuestData>();
                foreach (var questChapter in QuestInfos)
                {
                    var item = new UserChapterQuestData()
                    {
                        ChapterId = questChapter.Chapter.ChapterId,
                        Code = questChapter.Chapter.Code,
                        Description = questChapter.Chapter.Description,
                        Name = questChapter.Chapter.Name,
                        Order = questChapter.Chapter.Order,
                        Quests = new List<UserQuestProgressData>()
                    };

                    foreach (var quest in questChapter.Quests)
                    {
                        item.Quests.Add(new UserQuestProgressData()
                        {
                            InitialData = quest.Quest.DataString,
                            MilestoneId = quest.Quest.MilestoneId,
                            QuestType = quest.Quest.QuestType,
                            QuestId = quest.Quest.QuestId,
                            Completed = false,
                            ProgressData = "{}"
                        });
                    }

                    lst.Add(item);
                }

                return lst;
            }
        }
        public static bool IsLoaded { get => isLoaded && QuestInfo != null; }
        public static IReadOnlyList<IReadOnlyChapterQuestRelData> QuestInfos { get { CheckLoadCacheMemory(); return QuestInfo.ToList(); } }

        public static IReadOnlyChapterQuestRelData GetFullChapterData(int id)
        {
            var item = QuestInfos.FirstOrDefault(x => x.Chapter.ChapterId == id);
            if (item == null) throw new CacheDataNotExistExecption(QuestNotExist);
            else return item;
        }

        public static IReadOnlyChapterQuestRelData GetFullChapterData(string code)
        {
            var item = QuestInfos.FirstOrDefault(x => x.Chapter.Code == code);
            if (item == null) throw new CacheDataNotExistExecption(QuestNotExist);
            else return item;
        }

        public static IReadOnlyQuestRewardRelData GetQuestData(int questId)
        {
            foreach (var chapter in QuestInfos)
            {
                foreach (var quest in chapter.Quests)
                {
                    if (quest.Quest.QuestId == questId)
                    {
                        return quest;
                    }
                }
            }

            throw new CacheDataNotExistExecption(QuestNotExist);
        }


        #region Cache Check, Load and Clear
        public static async Task LoadCacheMemoryAsync()
        {
            ClearCache();

            var resManager = new QuestManager();
            var response = await resManager.GetAllChapterQuestRelData();

            if (response.IsSuccess)
            {
                QuestInfo = response.Data;
                isLoaded = true;
            }
            else
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

            if (QuestInfo != null)
            {
                QuestInfo.Clear();
                QuestInfo = null;
            }
        }
        #endregion
    }
}

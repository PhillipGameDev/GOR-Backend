using GameOfRevenge.Common.Models.Quest.Template;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GameOfRevenge.Common.Models.Quest
{
    public class UserChapterAllQuestProgress
    {
        public List<UserChapterQuestData> ChapterQuests;
        public List<UserQuestProgressData> SideQuests;
        public List<UserQuestProgressData> DailyQuests;
    }

    public class UserChapterQuestData : ChapterTable
    {
        public int ChapterUserDataId { get; set; }
        public bool Redeemed { get; set; }

        public bool Completed()
        {
            var completed = true;
            if (Quests != null)
            {
                foreach (var quest in Quests)
                {
                    if (quest.Completed) continue;

                    completed = false;
                    break;
                }
            }

            return completed;
        }

        public List<UserQuestProgressData> Quests { get; set; }
    }

    public class UserQuestProgressData : PlayerQuestDataTable
    {
        public int MilestoneId { get; set; }
        public QuestType QuestType { get; set; }
        public string InitialData { get; set; }

        public string GetName()
        {
            return PlayerQuestDataTable.GetName(QuestType, Completed, InitialData, ProgressData);
        }
    }
}

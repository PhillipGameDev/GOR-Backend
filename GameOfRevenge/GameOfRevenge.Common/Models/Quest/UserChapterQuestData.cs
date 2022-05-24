using GameOfRevenge.Common.Models.Quest.Template;
using System;
using System.Collections.Generic;

namespace GameOfRevenge.Common.Models.Quest
{
    public class UserChapterQuestData : ChapterTable
    {
        public int ChapterUserDataId { get; set; }
        public bool IsRedeemed { get; set; }
        public bool Completed
        {
            get
            {
                if (Quests == null || Quests.Count == 0) return true;
                else
                {
                    foreach (var quest in Quests) if (!quest.Completed) return false;
                    return true;
                }
            }
        }

        public List<UserQuestProgressData> Quests { get; set; }
    }

    public class UserQuestProgressData
    {
        public int QuestUserDataId { get; set; }
        public int QuestId { get; set; }
        public int MilestoneId { get; set; }
        public string Name { get => GetName(); }
        public QuestType QuestType { get; set; }
        public bool Completed { get; set; }
        public string InitialData { get; set; }
        public string ProgressData { get; set; }
        public bool IsRedeemed { get; set; }


        private string GetName()
        {
            IBaseQuestTemplateData template;

            switch (QuestType)
            {
                case QuestType.BuildingUpgrade:
                    template = new QuestBuildingData();
                    break;
                case QuestType.XBuildingCount:
                    template = new QuestHaveXBuildingData();
                    break;
                case QuestType.ResourceCollection:
                    template = new QuestCollectXResourceData();
                    break;
                case QuestType.TrainTroops:
                    template = new QuestTrainXTroopData();
                    break;
                case QuestType.XTroopCount:
                    template = new QuestHaveXTroopData();
                    break;
                default: return string.Empty;
            }

            if (template == null) return string.Empty;

            template.SetData(InitialData);
            return template.Name;
        }
    }
}



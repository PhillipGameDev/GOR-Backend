using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GameOfRevenge.Common.Models.Quest
{
    public class UserChapterAllQuestProgress
    {
        public List<UserChapterQuestData> ChapterQuests;
        public List<PlayerQuestDataTable> SideQuests;
        public List<PlayerQuestDataTable> DailyQuests;
    }

    [DataContract]
    public class UserChapterQuestData : ChapterTable
    {
        [DataMember]
        public int ChapterUserDataId { get; set; }
        [DataMember(Order = 10)]
        public bool Redeemed { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public new string Name { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public new string Description { get; set; }

        private string name;
        private string desciption;
        [OnSerializing]
        void OnSerializing(StreamingContext context)
        {
            name = Name;
            Name = null;
            desciption = Description;
            Description = null;
        }
        [OnSerialized]
        void OnSerialized(StreamingContext context)
        {
            Name = name;
            Description = desciption;
        }

        [DataMember(Order = 11)]
        public List<PlayerQuestDataTable> Quests { get; set; }

        public int TotalQuests { get; set; }

        public bool AllQuestsCompleted => (Quests != null) && (Quests.Count >= TotalQuests) && !Quests.Exists(x => !x.Completed);
        public bool AllQuestsRedeemed => (Quests != null) && (Quests.Count >= TotalQuests) && !Quests.Exists(x => !x.Redeemed);
    }
}

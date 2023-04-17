using System;
using System.Data;
using System.Runtime.Serialization;

namespace GameOfRevenge.Common.Models.Quest
{
    public interface IReadOnlyChapterRewardData : IReadOnlyQuestRewardRelData
    {
        int ChapterId { get; }
        string Name { get; }
        string Description { get; }
    }

    [DataContract]
    public class ChapterRewardData : QuestRewardRelData, IReadOnlyChapterRewardData
    {
        [DataMember]
        public int ChapterId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
/*#if !UNITY_2019_1_OR_NEWER
        [DataMember(Name = "Quest")]
        public QuestTableQuestId QuestId { get; set; }
#endif*/
/*        [OnSerializing]
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
        }*/
    }

/*    [DataContract]
    public class QuestTableQuestId
    {
        [DataMember]
        public int QuestId { get; set; }

        public QuestTableQuestId(int questId)
        {
            QuestId = questId;
        }
    }*/
}

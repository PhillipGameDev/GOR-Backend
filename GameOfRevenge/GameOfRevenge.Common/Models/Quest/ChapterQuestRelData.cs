using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace GameOfRevenge.Common.Models.Quest
{
    public interface IReadOnlyChapterQuestRelData
    {
        IReadOnlyChapterRewardData Chapter { get; }
        IReadOnlyList<IReadOnlyQuestRewardRelData> Quests { get; }
//        IReadOnlyList<IReadOnlyDataReward> Rewards { get; }
    }

    public interface IReadOnlyQuestRewardRelData
    {
        IReadOnlyQuestTable Quest { get; }
        IReadOnlyList<IReadOnlyDataReward> Rewards { get; }
    }

    [DataContract]
    public class QuestRewardRelData : IReadOnlyQuestRewardRelData
    {
        [DataMember(EmitDefaultValue = false)]
        public QuestTable Quest { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<DataReward> Rewards { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyQuestTable IReadOnlyQuestRewardRelData.Quest { get => Quest; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyList<IReadOnlyDataReward> IReadOnlyQuestRewardRelData.Rewards { get => Rewards; }
    }

    public class ChapterQuestRelData : IReadOnlyChapterQuestRelData
    {
        public ChapterRewardData Chapter { get; set; }
        public List<QuestRewardRelData> Quests { get; set; }
//        public List<DataReward> Rewards { get; set; }
//        public QuestRewardRelData QuestRewards { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyChapterRewardData IReadOnlyChapterQuestRelData.Chapter { get => Chapter; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyList<IReadOnlyQuestRewardRelData> IReadOnlyChapterQuestRelData.Quests { get => Quests; }

//        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
//        IReadOnlyList<IReadOnlyDataReward> IReadOnlyChapterQuestRelData.Rewards { get => Rewards; }
    }
}

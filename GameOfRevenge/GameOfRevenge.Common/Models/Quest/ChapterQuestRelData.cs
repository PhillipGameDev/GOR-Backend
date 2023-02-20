using System.Collections.Generic;
using System.Diagnostics;

namespace GameOfRevenge.Common.Models.Quest
{
    public interface IReadOnlyChapterQuestRelData
    {
        IReadOnlyChapterTable Chapter { get; }
        IReadOnlyList<IReadOnlyQuestRewardRelData> Quests { get; }
        IReadOnlyList<IReadOnlyDataReward> Rewards { get; }
    }

    public interface IReadOnlyQuestRewardRelData
    {
        IReadOnlyQuestTable Quest { get; }
        IReadOnlyList<IReadOnlyDataReward> Rewards { get; }
    }

    public class QuestRewardRelData : IReadOnlyQuestRewardRelData
    {
        public QuestTable Quest { get; set; }
        public List<DataReward> Rewards { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyQuestTable IReadOnlyQuestRewardRelData.Quest { get => Quest; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyList<IReadOnlyDataReward> IReadOnlyQuestRewardRelData.Rewards { get => Rewards; }
    }

    public class ChapterQuestRelData : IReadOnlyChapterQuestRelData
    {
        public ChapterTable Chapter { get; set; }
        public List<QuestRewardRelData> Quests { get; set; }
        public List<DataReward> Rewards { get; set; }
        public QuestRewardRelData QuestRewards { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyChapterTable IReadOnlyChapterQuestRelData.Chapter { get => Chapter; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyList<IReadOnlyQuestRewardRelData> IReadOnlyChapterQuestRelData.Quests { get => Quests; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyList<IReadOnlyDataReward> IReadOnlyChapterQuestRelData.Rewards { get => Rewards; }
    }
}

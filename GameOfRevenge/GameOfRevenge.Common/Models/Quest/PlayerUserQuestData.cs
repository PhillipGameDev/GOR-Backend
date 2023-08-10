using System;

namespace GameOfRevenge.Common.Models.Quest
{
    public class PlayerUserQuestData
    {
        public int PlayerId { get; set; }
        public PlayerCompleteData UserData { get; set; }
        public UserChapterAllQuestProgress QuestData { get; set; }
        public Action<PlayerQuestDataTable> QuestEventAction { get; set; }
    }
}

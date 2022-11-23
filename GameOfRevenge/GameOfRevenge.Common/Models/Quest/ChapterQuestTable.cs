using System;
using System.Data;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models.Quest
{
    public interface IReadOnlyChapterQuestTable
    {
        int ChapterQuestId { get; }
        int ChapterId { get; }
        int QuestId { get; }
    }

    public class ChapterQuestTable : IBaseTable, IReadOnlyChapterQuestTable
    {
        public int ChapterQuestId { get; set; }
        public int ChapterId { get; set; }
        public int QuestId { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            ChapterQuestId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            ChapterId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            QuestId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index);
        }
    }
}

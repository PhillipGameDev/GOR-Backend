using System;
using System.Data;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models.Quest
{
    public interface IReadOnlyPlayerChapterData
    {
        int ChapterUserDataId { get; }
        int ChapterId { get; }
        int PlayerId { get; }
        bool Redemeed { get; }
    }

    public class PlayerChapterData : IBaseTable, IReadOnlyPlayerChapterData
    {
        public int ChapterUserDataId { get; set; }
        public int PlayerId { get; set; }
        public int ChapterId { get; set; }
        public bool Redemeed { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            ChapterUserDataId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            ChapterId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            PlayerId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Redemeed = reader.GetValue(index) == DBNull.Value ? false : reader.GetBoolean(index);
        }
    }
}

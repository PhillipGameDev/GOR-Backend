using System;
using System.Data;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models.Quest
{
    public interface IReadOnlyPlayerQuestData
    {
        int QuestUserDataId { get; }
        int QuestId { get; }
        int PlayerId { get; }
        bool Completed { get; }
        string ProgressData { get; }
        bool Redemeed { get; }
    }

    public class PlayerQuestData : IBaseTable, IReadOnlyPlayerQuestData
    {
        public int QuestUserDataId { get; set; }
        public int PlayerId { get; set; }
        public int QuestId { get; set; }
        public bool Completed { get; set; }
        public string ProgressData { get; set; }
        public bool Redemeed { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            QuestUserDataId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            QuestId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            PlayerId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Completed = reader.GetValue(index) == DBNull.Value ? false : reader.GetBoolean(index); index++;
            ProgressData = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            Redemeed = reader.GetValue(index) == DBNull.Value ? false : reader.GetBoolean(index);
        }
    }
}

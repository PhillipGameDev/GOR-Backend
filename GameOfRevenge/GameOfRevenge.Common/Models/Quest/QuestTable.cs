using System;
using System.Data;
using GameOfRevenge.Common.Helper;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models.Quest
{
    public interface IReadOnlyQuestTable
    {
        int QuestId { get; }
        QuestType QuestType { get; }
        int MilestoneId { get; }
        string DataString { get; }
    }

    public class QuestTable : IBaseTable, IReadOnlyQuestTable
    {
        public int QuestId { get; set; }
        public QuestType QuestType { get; set; }
        public int MilestoneId { get; set; }
        public string DataString { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            QuestId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            QuestType = reader.GetValue(index) == DBNull.Value ? QuestType.Other : reader.GetString(index).ToEnum<QuestType>(); index++;
            MilestoneId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            DataString = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index);
        }
    }
}

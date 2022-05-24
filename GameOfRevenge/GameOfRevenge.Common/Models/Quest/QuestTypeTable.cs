//using System;
//using System.Data;
//using GameOfRevenge.Common.Helper;
//using GameOfRevenge.Common.Models.Table;

//namespace GameOfRevenge.Common.Models.Quest
//{
//    public interface IReadOnlyQuestTypeTable
//    {
//        int QuestTypeId { get; }
//        QuestType Code { get; }
//        string Name { get; }
//    }

//    public class QuestTypeTable : IBaseTable, IReadOnlyQuestTypeTable
//    {
//        public int QuestTypeId { get; set; }
//        public QuestType Code { get; set; }
//        public string Name { get; set; }

//        public void LoadFromDataReader(IDataReader reader)
//        {
//            int index = 0;
//            QuestTypeId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
//            Name = reader.GetString(index) ?? string.Empty; index++;
//            Code = reader.GetValue(index) == DBNull.Value ? QuestType.Other : reader.GetString(index).ToEnum<QuestType>();
//        }
//    }
//}

using System;
using System.Data;
using System.Runtime.Serialization;
using GameOfRevenge.Common.Helper;
using GameOfRevenge.Common.Models.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GameOfRevenge.Common.Models.Quest
{
    public static class QuestGroupType
    {
        public const int CHAPTER_QUEST = 0;
        public const int SIDE_QUEST = 99;
        public const int DAILY_QUEST = -1;
        public const int PRODUCT_PACK = 200;
    }

    public interface IReadOnlyQuestTable
    {
        int QuestId { get; }
        int QuestGroup { get; }
        QuestType QuestType { get; }
        string DataString { get; }
    }

    [DataContract]
    public class QuestTable : IBaseTable, IReadOnlyQuestTable
    {
        [DataMember]
        public int QuestId { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public int QuestGroup { get; set; }
        [DataMember(EmitDefaultValue = false), JsonConverter(typeof(StringEnumConverter))]
        public QuestType QuestType { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string DataString { get; set; }

#if !UNITY_2019_1_OR_NEWER
        private QuestType questType;
        private string dataString;
        [OnSerializing]
        void OnSerializing(StreamingContext context)
        {
            questType = QuestType;
            dataString = DataString;
            if ((QuestGroup == QuestGroupType.CHAPTER_QUEST) && (QuestType == QuestType.Custom))
            {
                QuestType = QuestType.Other;
                DataString = null;
            }
        }
        [OnSerialized]
        void OnSerialized(StreamingContext context)
        {
            QuestType = questType;
            DataString = dataString;
        }
#endif

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            QuestId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            QuestGroup = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            QuestType = reader.GetValue(index) == DBNull.Value ? QuestType.Other : (QuestType)reader.GetInt32(index); index++;
            DataString = reader.GetValue(index) == DBNull.Value ? null : reader.GetString(index);
        }
    }
}

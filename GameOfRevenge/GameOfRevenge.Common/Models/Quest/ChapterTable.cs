using System;
using System.Data;
using System.Runtime.Serialization;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models.Quest
{
    public interface IReadOnlyChapterTable
    {
        int ChapterId { get; }
        string Name { get; }
        string Description { get; }
    }

    [DataContract]
    public class ChapterTable : IBaseTable, IReadOnlyChapterTable
    {
        [DataMember]
        public int ChapterId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            ChapterId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Name = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            Description = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index);
        }
    }
}

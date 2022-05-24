using System;
using System.Data;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models.Quest
{
    public interface IReadOnlyChapterTable
    {
        int ChapterId { get; }
        string Code { get; }
        string Name { get; }
        string Description { get; }
        float Order { get; }
    }

    public class ChapterTable : IBaseTable, IReadOnlyChapterTable
    {
        public int ChapterId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Order { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            ChapterId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Name = reader.GetString(index) ?? string.Empty; index++;
            Description = reader.GetString(index) ?? string.Empty; index++;
            Code = reader.GetString(index) ?? string.Empty; index++;
            Order = (float) (reader.GetValue(index) == DBNull.Value ? 0 : reader.GetDouble(index));
        }
    }
}

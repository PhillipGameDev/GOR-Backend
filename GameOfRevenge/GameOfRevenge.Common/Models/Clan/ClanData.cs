using GameOfRevenge.Common.Models.Table;
using System;
using System.Data;

namespace GameOfRevenge.Common.Models.Clan
{
    public class ClanData : IBaseTable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            Id = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Name = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            Tag = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            Description = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            IsPublic = reader.GetValue(index) != DBNull.Value && reader.GetBoolean(index);
        }
    }
}

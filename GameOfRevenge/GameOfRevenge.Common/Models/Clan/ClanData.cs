using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Models.Table;
using System;
using System.Data;

namespace GameOfRevenge.Common.Models.Clan
{
    public class ClanData : IBaseTable, IClanData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; }
        public short BadgeGK { get; set; }
        public int Flag { get; set; }
        public int Capacity { get; set; }
        public int MemberCount { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            Id = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Name = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            Tag = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            Description = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            IsPublic = reader.GetValue(index) != DBNull.Value && reader.GetBoolean(index); index++;
            BadgeGK = reader.GetValue(index) == DBNull.Value ? (short)0 : reader.GetInt16(index); index++;
            Flag = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Capacity = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;

            if (index < reader.FieldCount)
            {
                MemberCount = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index);
            }
        }
    }
}

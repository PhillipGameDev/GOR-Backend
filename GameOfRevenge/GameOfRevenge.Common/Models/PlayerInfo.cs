using System;
using System.Data;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models
{
    public class PlayerInfo : IBaseTable
    {
        public int PlayerId { get; set; }
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsDeveloper { get; set; }

        public byte KingLevel { get; set; }
        public byte VIPLevel { get; set; }
        public byte CastleLevel { get; set; }
        public int AllianceId { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            var index = 0;
            PlayerId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Name = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            IsAdmin = reader.GetValue(index) != DBNull.Value && reader.GetBoolean(index); index++;
            IsDeveloper = reader.GetValue(index) != DBNull.Value && reader.GetBoolean(index); index++;

            KingLevel = reader.GetValue(index) == DBNull.Value ? (byte)0 : reader.GetByte(index); index++;
            VIPLevel = reader.GetValue(index) == DBNull.Value ? (byte)0 : reader.GetByte(index); index++;
            CastleLevel = reader.GetValue(index) == DBNull.Value ? (byte)0 : reader.GetByte(index); index++;
            AllianceId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index);
        }
    }
}

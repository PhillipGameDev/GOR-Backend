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
        public byte CastleLevel { get; set; }
        public byte WatchLevel { get; set; }
        public DateTime ShieldEndTime { get; set; }
        public int Invaded { get; set; }

        public int VIPPoints { get; set; }
        public int AllianceId { get; set; }
        public DateTime RegisteredDate { get; set; }
        public DateTime LastLogin { get; set; }

        public int VIPLevel => UserVIPDetails.VIPLevel(VIPPoints);

        public void LoadFromDataReader(IDataReader reader)
        {
            var index = 0;
            PlayerId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Name = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            IsAdmin = reader.GetValue(index) != DBNull.Value && reader.GetBoolean(index); index++;
            IsDeveloper = reader.GetValue(index) != DBNull.Value && reader.GetBoolean(index); index++;

            KingLevel = reader.GetValue(index) == DBNull.Value ? (byte)0 : reader.GetByte(index); index++;
            CastleLevel = reader.GetValue(index) == DBNull.Value ? (byte)0 : reader.GetByte(index); index++;
            WatchLevel = reader.GetValue(index) == DBNull.Value ? (byte)0 : reader.GetByte(index); index++;
            ShieldEndTime = reader.GetValue(index) == DBNull.Value ? new DateTime() : reader.GetDateTime(index); index++;
            Invaded = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;

            VIPPoints = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            AllianceId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            RegisteredDate = reader.GetValue(index) == DBNull.Value ? new DateTime() : reader.GetDateTime(index); index++;
            LastLogin = reader.GetValue(index) == DBNull.Value ? new DateTime() : reader.GetDateTime(index);
        }
    }
}

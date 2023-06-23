using System;
using System.Data;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models
{
    public interface IReadOnlyFriendRequestElement
    {
        long RequestId { get; }
        int FromPlayerId { get; }
        string FromPlayerName { get; }
        int ToPlayerId { get; }
        string ToPlayerName { get; }
        byte Flags { get; }
    }

    public class FriendRequestElement : IBaseTable, IReadOnlyFriendRequestElement
    {
        public long RequestId { get; set; }
        public int FromPlayerId { get; set; }
        public string FromPlayerName { get; set; }
        public int ToPlayerId { get; set; }
        public string ToPlayerName { get; set; }
        public byte Flags { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            RequestId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt64(index); index++;
            FromPlayerId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            FromPlayerName = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            ToPlayerId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            ToPlayerName = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            Flags = reader.GetValue(index) == DBNull.Value ? (byte)0 : reader.GetByte(index);
        }
    }
}

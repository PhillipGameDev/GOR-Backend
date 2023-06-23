using System;
using System.Data;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models
{
    public interface IReadOnlyFriendElement
    {
        long FriendId { get; }
        int PlayerId { get; }
        string PlayerName { get; }
        int Rank { get; }
        int ClanId { get; }
        string ClanName { get; }
    }

    public class FriendElement : IBaseTable, IReadOnlyFriendElement
    {
        public long FriendId { get; set; }
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int Rank { get; set; }
        public int ClanId { get; set; }
        public string ClanName { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            FriendId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt64(index); index++;
            PlayerId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            PlayerName = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            Rank = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            ClanId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            ClanName = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index);
        }
    }
}

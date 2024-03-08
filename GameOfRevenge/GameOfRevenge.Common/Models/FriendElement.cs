using System;
using System.Data;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models
{
    public interface IReadOnlyFriendElement
    {
        long ContactId { get; }
        int PlayerId { get; }
        string PlayerName { get; }
        int Rank { get; }
        int ClanId { get; }
        string ClanName { get; }
        ContactStatus Status { get; }
    }

    public class ContactElement : IBaseTable, IReadOnlyFriendElement, IComparable<ContactElement>
    {
        public long ContactId { get; set; }
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int Rank { get; set; }
        public int ClanId { get; set; }
        public string ClanName { get; set; }
        public ContactStatus Status { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            ContactId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt64(index); index++;
            PlayerId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            PlayerName = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            Rank = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            ClanId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            ClanName = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            if (index < reader.FieldCount)
            {
                Status = reader.GetValue(index) == DBNull.Value ? ContactStatus.Unknown : (ContactStatus)reader.GetByte(index);
            }
        }

        public int CompareTo(ContactElement other)
        {
            return ContactId.CompareTo(other.ContactId);
        }
    }

    public enum ContactStatus
    {
        Unknown = 0,
        Blocked = 1,
        Following = 2,
        Follower = 3
    }
}

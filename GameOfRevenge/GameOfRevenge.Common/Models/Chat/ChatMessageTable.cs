using System;
using System.Data;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models.Chat
{
    public class ChatMessageTable : ChatEntry<string>, IBaseTable
    {
        public void LoadFromDataReader(IDataReader reader)
        {
            var index = 0;
            ChatId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt64(index); index++;
            PlayerId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Username = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            VIPPoints = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Content = reader.GetValue(index) == DBNull.Value ? null : reader.GetString(index); index++;
            Date = reader.GetValue(index) == DBNull.Value ? DateTime.MinValue : reader.GetDateTime(index); index++;
            Flags = reader.GetValue(index) == DBNull.Value ? (byte)0 : reader.GetByte(index);
        }
    }


    public interface IChatEntry
    {
        long ChatId { get; }
        int PlayerId { get; }
        string Username { get; }
        int VIPPoints { get; }
        DateTime Date { get; }
        byte Flags { get; }
    }

    public interface IChatEntry<T> : IChatEntry
    {
        T Content { get; }
    }

    public class ChatEntry<T> : IChatEntry<T>
    {
        public long ChatId { get; set; }
        public int PlayerId { get; set; }
        public string Username { get; set; }
        public int VIPPoints { get; set; }
        public DateTime Date { get; set; }
        public byte Flags { get; set; }
        public T Content { get; set; }

        public ChatEntry()
        {
        }
    }



    [DataContract, Serializable]
    public class ChatMessages
    {
        [DataMember(EmitDefaultValue = false)]
        public List<PlayerBase> Players;

        [DataMember(EmitDefaultValue = false)]
        public List<ChatMessage> Messages;
    }

    [DataContract]
    public class ChatMessage
    {
        [DataMember]
        public long ChatId { get; set; }
        [DataMember]
        public int PlayerId { get; set; }
        [DataMember]
        public DateTime Date { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public byte Flags { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Content { get; set; }
    }

    public class PlayerBase
    {
        public int PlayerId { get; set; }
        public string Username { get; set; }
        public int VIPPoints { get; set; }
    }
}

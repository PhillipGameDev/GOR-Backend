using System;
using System.Data;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models.Chat
{
    public class ChatMessageFlagTable : IBaseTable
    {
        public byte Flags { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            var index = 0;
//            ChatId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt64(index); index++;
            Flags = reader.GetValue(index) == DBNull.Value ? (byte)0 : reader.GetByte(index);
        }
    }
}

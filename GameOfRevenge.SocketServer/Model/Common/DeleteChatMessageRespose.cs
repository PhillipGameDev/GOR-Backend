using System.Collections.Generic;
using Photon.SocketServer.Rpc;
using GameOfRevenge.Helpers;

namespace GameOfRevenge.Model
{
    public class DeleteChatMessageRespose : DictionaryEncode
    {
        [DataMember(Code = (byte)RoomParameterKey.ChatId, IsOptional = true)]
        public long ChatId { get; set; }

//        [DataMember(Code = (byte)RoomParameterKey.PlayerId, IsOptional = true)]
//        public int PlayerId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Flags, IsOptional = true)]
        public byte Flags { get; set; }
    }
}

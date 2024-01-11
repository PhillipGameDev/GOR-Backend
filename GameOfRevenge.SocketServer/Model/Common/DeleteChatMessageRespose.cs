using System.Collections.Generic;
using Photon.SocketServer.Rpc;
using GameOfRevenge.Helpers;

namespace GameOfRevenge.Model
{
    public class DeleteChatMessageRespose : DictionaryEncode
    {
        [DataMember(Code = (byte)RoomParameterKey.ChatId, IsOptional = true)]
        public long ChatId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.AllianceId, IsOptional = true)]
        public int AllianceId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Flags, IsOptional = true)]
        public byte Flags { get; set; }
    }
}

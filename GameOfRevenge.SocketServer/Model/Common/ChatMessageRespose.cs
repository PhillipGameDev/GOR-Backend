using System.Collections.Generic;
using Photon.SocketServer.Rpc;
using GameOfRevenge.Helpers;

namespace GameOfRevenge.Model
{
    public class ChatMessageRespose : DictionaryEncode
    {
        [DataMember(Code = (byte)RoomParameterKey.CommonMessage, IsOptional = true)]
        public string ChatMessage { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.UserName, IsOptional = true)]
        public string UserName { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.CurrentTime, IsOptional = true)]
        public string CurrentTime { get; set; }
    }
}

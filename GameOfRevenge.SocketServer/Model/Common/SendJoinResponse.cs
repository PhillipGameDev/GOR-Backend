using System.Collections.Generic;
using Photon.SocketServer.Rpc;
using GameOfRevenge.Helpers;

namespace GameOfRevenge.Model
{
    public class SendJoinResponse : DictionaryEncode
    {
        [DataMember(Code = (byte)RoomParameterKey.ClanId, IsOptional = true)]
        public int ClanId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.PlayerId, IsOptional = true)]
        public int PlayerId { get; set; }
    }
}

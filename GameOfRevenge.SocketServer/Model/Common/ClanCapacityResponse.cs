using System.Collections.Generic;
using Photon.SocketServer.Rpc;
using GameOfRevenge.Helpers;

namespace GameOfRevenge.Model
{
    public class ClanCapacityResponse : DictionaryEncode
    {
        [DataMember(Code = (byte)RoomParameterKey.ClanId, IsOptional = true)]
        public int ClanId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Value, IsOptional = true)]
        public int Value { get; set; }
    }
}

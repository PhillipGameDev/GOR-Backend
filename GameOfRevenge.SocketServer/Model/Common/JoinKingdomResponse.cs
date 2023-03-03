using System.Collections.Generic;
using Photon.SocketServer.Rpc;
using GameOfRevenge.Helpers;

namespace GameOfRevenge.Model
{
    public class JoinKingdomResponse : DictionaryEncode
    {
        [DataMember(Code = (byte)RoomParameterKey.X, IsOptional = true)]
        public short WorldSizeX { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Y, IsOptional = true)]
        public short WorldSizeY { get; set; }
    }
}

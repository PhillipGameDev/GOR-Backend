using System.Collections.Generic;
using Photon.SocketServer.Rpc;
using GameOfRevenge.Helpers;

namespace GameOfRevenge.Model
{
    public class GateResponse : DictEncode
    {
        public GateResponse(ref Dictionary<byte, object> dict) : base(ref dict) { }

        [DataMember(Code = (byte)RoomParameterKey.StructureLocationId, IsOptional = true)]
        public int BuildingLocationId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Hitpoint, IsOptional = true)]
        public int Hitpoint { get; set; }
    }
}

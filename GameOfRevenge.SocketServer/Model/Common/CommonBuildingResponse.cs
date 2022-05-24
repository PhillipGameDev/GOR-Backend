using System.Collections.Generic;
using Photon.SocketServer.Rpc;
using GameOfRevenge.Helpers;

namespace GameOfRevenge.Model
{
    public class CommonBuildingResponse : DictEncode
    {
        public CommonBuildingResponse(ref Dictionary<byte, object> dict) : base(ref dict)  { }

        [DataMember(Code = (byte)RoomParameterKey.StructureType, IsOptional = true)]
        public int StructureType { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.StructureLocationId, IsOptional = true)]
        public int LocationId { get; set; }
    }
}

using System.Collections.Generic;
using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class CollectResourceResponse : NewCommonBuildingResponse
    {
        [DataMember(Code = (byte)RoomParameterKey.ResourceCollected, IsOptional = true)]
        public int ResourceCollected { get; set; }
        [DataMember(Code = (byte)RoomParameterKey.ResourceTotal, IsOptional = true)]
        public long ResourceTotal { get; set; }
    }
}

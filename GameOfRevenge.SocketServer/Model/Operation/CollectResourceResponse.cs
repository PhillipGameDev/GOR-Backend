using System.Collections.Generic;
using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class CollectResourceResponse : NewCommonBuildingResponse
    {
        [DataMember(Code = (byte)RoomParameterKey.ResourceValue, IsOptional = true)]
        public int ResourceValue { get; set; }
    }
}

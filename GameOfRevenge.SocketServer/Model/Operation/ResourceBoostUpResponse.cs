using System.Collections.Generic;
using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class ResourceBoostUpResponse : NewCommonBuildingResponse
    {
        [DataMember(Code = (byte)RoomParameterKey.StartTime, IsOptional = true)]
        public string StartTime { get; set; }
        [DataMember(Code = (byte)RoomParameterKey.Duration, IsOptional = true)]
        public int Duration { get; set; }
        [DataMember(Code = (byte)RoomParameterKey.Percentage, IsOptional = true)]
        public int Percentage { get; set; }
    }
}

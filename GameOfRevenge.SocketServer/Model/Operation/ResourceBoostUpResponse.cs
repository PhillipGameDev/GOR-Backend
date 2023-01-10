using System.Collections.Generic;
using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class ResourceBoostUpResponse : NewCommonBuildingResponse
    {
        [DataMember(Code = (byte)RoomParameterKey.StartTime, IsOptional = true)]
        public string StartTime { get; set; }
        [DataMember(Code = (byte)RoomParameterKey.TotalTime, IsOptional = true)]
        public int TotalTime { get; set; }
        [DataMember(Code = (byte)RoomParameterKey.Multiplier, IsOptional = true)]
        public int Multiplier { get; set; }
    }
}

using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class ResourceBoostUpRequest : CommonBuildingRequest
    {
        public ResourceBoostUpRequest(IRpcProtocol protocol, OperationRequest request) : base(protocol, request) { }

        [DataMember(Code = (byte)RoomParameterKey.SetBoost, IsOptional = true)]
        public bool SetBoost { get; set; }

/*        [DataMember(Code = (byte)RoomParameterKey.BoostUpTime, IsOptional = true)]
        public int BoostTime { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Percentage, IsOptional = true)]
        public int Percentage { get; set; }*/
    }
}

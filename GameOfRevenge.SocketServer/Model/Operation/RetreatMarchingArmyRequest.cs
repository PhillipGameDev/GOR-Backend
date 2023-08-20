using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class RetreatMarchingArmyRequest : Operation
    {
        public RetreatMarchingArmyRequest(IRpcProtocol protocol, OperationRequest request) : base(protocol, request) { }

        [DataMember(Code = (byte)RoomParameterKey.MarchingId, IsOptional = true)]
        public long MarchingId { get; set; }
    }
}

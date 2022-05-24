using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class PlayerConnectRequest : Operation
    {
        public PlayerConnectRequest(IRpcProtocol protocol, OperationRequest request) : base(protocol, request) { }

        [DataMember(Code = (byte)RoomParameterKey.PlayerId, IsOptional = false)]
        public string PlayerId { get; set; }
    }
}

using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class CameraMoveRequest : Operation
    {
        public CameraMoveRequest(IRpcProtocol protocol, OperationRequest request) : base(protocol, request) { }

        [DataMember(Code = (byte)RoomParameterKey.X, IsOptional = false)]
        public float X { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Y, IsOptional = false)]
        public float Y { get; set; }
    }
}

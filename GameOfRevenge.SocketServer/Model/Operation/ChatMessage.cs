using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class ChatMessage : Operation
    {
        public ChatMessage(IRpcProtocol protocol, OperationRequest request) : base(protocol, request) { }

        [DataMember(Code = (byte)RoomParameterKey.CommonMessage, IsOptional = true)]
        public string Message { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.UserName, IsOptional = true)]
        public string UserName { get; set; }
    }
}

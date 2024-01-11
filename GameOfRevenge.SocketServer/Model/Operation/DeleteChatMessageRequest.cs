using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class DeleteChatMessageRequest : Operation
    {
        public DeleteChatMessageRequest(IRpcProtocol protocol, OperationRequest request) : base(protocol, request) { }

        [DataMember(Code = (byte)RoomParameterKey.ChatId, IsOptional = true)]
        public long ChatId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.AllianceId, IsOptional = true)]
        public int AllianceId { get; set; }
    }
}

using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class ChatMessageRequest : Operation
    {
        public ChatMessageRequest(IRpcProtocol protocol, OperationRequest request) : base(protocol, request) { }

        [DataMember(Code = (byte)RoomParameterKey.CommonMessage, IsOptional = true)]
        public string Message { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.AllianceId, IsOptional = true)]
        public int AllianceId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.UserName, IsOptional = true)]
        public string UserName { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.CurrentTime, IsOptional = true)]
        public string CurrentTime { get; set; }
    }
}

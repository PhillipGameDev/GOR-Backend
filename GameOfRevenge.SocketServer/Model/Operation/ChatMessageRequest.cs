using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class ChatMessageRequest : Operation
    {
        public ChatMessageRequest(IRpcProtocol protocol, OperationRequest request) : base(protocol, request) { }

//        [DataMember(Code = (byte)RoomParameterKey.PlayerId, IsOptional = true)]
//        public int PlayerId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Username, IsOptional = true)]
        public string Username { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.AllianceId, IsOptional = true)]
        public int AllianceId { get; set; }

//        [DataMember(Code = (byte)RoomParameterKey.CurrentTime, IsOptional = true)]
//        public string CurrentTime { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.ChatMessage, IsOptional = true)]
        public string ChatMessage { get; set; }
    }
}

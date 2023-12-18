using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Helpers;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class MessageRequest : Operation
    {
        public MessageRequest(IRpcProtocol protocol, OperationRequest request) : base(protocol, request) { }

        [DataMember(Code = (byte)RoomParameterKey.PlayerId, IsOptional = false)]
        public int SenderPlayerId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TargetId, IsOptional = false)]
        public int TargetPlayerId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Subject, IsOptional = false)]
        public string Subject { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Message, IsOptional = true)]
        public string Message { get; set; }
    }

    public class MessageResponse : DictionaryEncode
    {
    }
}

using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using GameOfRevenge.Helpers;

namespace GameOfRevenge.Model
{
    public class PlayerContactRequest : Operation
    {
        public PlayerContactRequest(IRpcProtocol protocol, OperationRequest request) : base(protocol, request) { }

        [DataMember(Code = (byte)RoomParameterKey.TargetId, IsOptional = false)]
        public int TargetPlayerId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Value, IsOptional = false)]
        public byte Value { get; set; }

    }

    public class PlayerContactResponse : DictionaryEncode
    {
        [DataMember(Code = (byte)RoomParameterKey.Id, IsOptional = false)]
        public long ContactId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.PlayerId, IsOptional = false)]
        public int PlayerId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TargetId, IsOptional = false)]
        public int TargetId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Data, IsOptional = false)]
        public string Data { get; set; }
    }
}

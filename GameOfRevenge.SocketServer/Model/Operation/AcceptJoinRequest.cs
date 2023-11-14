using GameOfRevenge.Common.Models.Structure;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class AcceptJoinRequest : Operation
    {
        public AcceptJoinRequest(IRpcProtocol protocol, OperationRequest request) : base(protocol, request) { }

        [DataMember(Code = (byte)RoomParameterKey.ClanId, IsOptional = true)]
        public int ClanId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.PlayerId, IsOptional = true)]
        public int PlayerId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Value, IsOptional = true)]
        public bool Value { get; set; }
    }
}

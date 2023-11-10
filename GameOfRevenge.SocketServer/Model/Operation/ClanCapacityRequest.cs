using GameOfRevenge.Common.Models.Structure;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class ClanCapacityRequest : Operation
    {
        public ClanCapacityRequest(IRpcProtocol protocol, OperationRequest request) : base(protocol, request) { }

        [DataMember(Code = (byte)RoomParameterKey.ClanId, IsOptional = true)]
        public int ClanId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Value, IsOptional = true)]
        public int Value { get; set; }
    }
}

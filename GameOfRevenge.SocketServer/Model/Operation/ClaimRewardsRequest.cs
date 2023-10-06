using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class ClaimRewardsRequest : Operation
    {
        public ClaimRewardsRequest(IRpcProtocol protocol, OperationRequest request) : base(protocol, request) { }

        [DataMember(Code = (byte)RoomParameterKey.TargetType, IsOptional = true)]
        public byte TargetType { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TargetId, IsOptional = true)]
        public int TargetId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Value, IsOptional = true)]
        public int[] Values { get; set; }
    }
}

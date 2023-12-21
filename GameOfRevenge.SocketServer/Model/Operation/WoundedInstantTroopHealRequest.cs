using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class WoundedInstantTroopHealRequest : Operation
    {
        public WoundedInstantTroopHealRequest(IRpcProtocol protocol, OperationRequest request) : base(protocol, request) { }

        [DataMember(Code = (byte)RoomParameterKey.TroopType, IsOptional = true)]
        public int TroopType { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TroopLevel, IsOptional = true)]
        public int TroopLevel { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Timestamp, IsOptional = true)]
        public long StartTime { get; set; }
    }
}

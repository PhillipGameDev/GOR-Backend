using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class WoundedTroopHealRequest : Operation
    {
        public WoundedTroopHealRequest(IRpcProtocol protocol, OperationRequest request) : base(protocol, request) { }

        [DataMember(Code = (byte)RoomParameterKey.StructureLocationId, IsOptional = true)]
        public int BuildingLocationId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TroopLevel, IsOptional = true)]
        public int[] TroopLevel { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TroopType, IsOptional = true)]
        public int[] TroopType { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.WoundedCount, IsOptional = true)]
        public int[] WoundedCount { get; set; }
    }
}

using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class UpgradeStructureRequest : Operation
    {
        public UpgradeStructureRequest(IRpcProtocol protocol, OperationRequest request) : base(protocol, request) { }

        [DataMember(Code = (byte)RoomParameterKey.StructureType, IsOptional = false)]
        public int StructureType { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.StructureLocationId, IsOptional = false)]
        public int StructureLocationId { get; set; }
    }
}

using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class UpgradeTechnologyRequest : Operation
    {
        public UpgradeTechnologyRequest(IRpcProtocol protocol, OperationRequest request) : base(protocol, request) { }

        [DataMember(Code = (byte)RoomParameterKey.StructureLocationId, IsOptional = true)]
        public int BuildingLocationId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TechLevel, IsOptional = true)]
        public int Level { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TechType, IsOptional = true)]
        public int TechType { get; set; }
    }
}

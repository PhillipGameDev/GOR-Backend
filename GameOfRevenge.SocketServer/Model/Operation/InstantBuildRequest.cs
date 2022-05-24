using GameOfRevenge.Common.Models.Structure;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class InstantBuildRequest : Operation
    {
        public InstantBuildRequest(IRpcProtocol protocol, OperationRequest request) : base(protocol, request) { }

        [DataMember(Code = (byte)RoomParameterKey.StructureType, IsOptional = true)]
        public int StructureType { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.StructureLevel, IsOptional = true)]
        public int StructureLevel { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.StructureLocationId, IsOptional = true)]
        public int StructureLocationId { get; set; }
    }
}

using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class TroopTrainingStatusRequest : Operation
    {
        public TroopTrainingStatusRequest(IRpcProtocol protocol, OperationRequest request) : base(protocol, request) { }

        [DataMember(Code = (byte)RoomParameterKey.StructureType, IsOptional = true)]
        public int StructureType { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.StructureLocationId, IsOptional = true)]
        public int LocationId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TroopType, IsOptional = true)]
        public int TroopType { get; set; }
    }
}

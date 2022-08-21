using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using GameOfRevenge.Helpers;

namespace GameOfRevenge.Model
{
    public class HelpStructureRequest : Operation
    {
        public HelpStructureRequest(IRpcProtocol protocol, OperationRequest request) : base(protocol, request) { }

        [DataMember(Code = (byte)RoomParameterKey.PlayerId, IsOptional = false)]
        public int PlayerId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.StructureType, IsOptional = false)]
        public int StructureType { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.StructureLocationId, IsOptional = false)]
        public int StructureLocationId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.HelpPower, IsOptional = false)]
        public int HelpPower { get; set; }
    }

    public class HelpStructureRespose : DictionaryEncode
    {
        [DataMember(Code = (byte)RoomParameterKey.PlayerId, IsOptional = false)]
        public int PlayerId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.StructureType, IsOptional = false)]
        public int StructureType { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.StructureLocationId, IsOptional = false)]
        public int StructureLocationId { get; set; }

    }
}

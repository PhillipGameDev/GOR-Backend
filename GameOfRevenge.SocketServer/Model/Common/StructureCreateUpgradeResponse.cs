using Photon.SocketServer.Rpc;
using GameOfRevenge.Helpers;

namespace GameOfRevenge.Model
{
    public class StructureCreateUpgradeResponse : DictionaryEncode
    {
        [DataMember(Code = (byte)RoomParameterKey.StructureLocationId, IsOptional = false)]
        public int StructureLocationId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.StructureType, IsOptional = false)]
        public int StructureType { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.StructureLevel, IsOptional = false)]
        public int StructureLevel { get; set; }
    }
}

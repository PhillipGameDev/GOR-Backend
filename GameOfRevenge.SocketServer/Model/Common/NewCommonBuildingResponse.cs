using Photon.SocketServer.Rpc;
using GameOfRevenge.Helpers;

namespace GameOfRevenge.Model
{
    public class NewCommonBuildingResponse : DictionaryEncode
    {
        [DataMember(Code = (byte)RoomParameterKey.StructureType, IsOptional = true)]
        public int StructureType { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.StructureLocationId, IsOptional = true)]
        public int LocationId { get; set; }
    }
}

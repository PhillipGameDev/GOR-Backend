using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class TimerCompleteResponse : CommonResponse
    {
        [DataMember(Code = (byte)RoomParameterKey.StructureLocationId, IsOptional = false)]
        public int LocationId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.StructureLevel, IsOptional = true)]
        public int Level { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.StructureType, IsOptional = true)]
        public int StructureType { get; set; }
    }
}

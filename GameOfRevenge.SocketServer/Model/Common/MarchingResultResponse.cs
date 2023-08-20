using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class MarchingResultResponse : CommonResponse
    {
        [DataMember(Code = (byte)RoomParameterKey.MarchingId, IsOptional = true)]
        public long MarchingId { get; set; }
        [DataMember(Code = (byte)RoomParameterKey.MarchingType, IsOptional = true)]
        public string MarchingType { get; set; }
        [DataMember(Code = (byte)RoomParameterKey.AttackerId, IsOptional = true)]
        public int AttackerId { get; set; }
        [DataMember(Code = (byte)RoomParameterKey.Username, IsOptional = true)]
        public string AttackerName { get; set; }
        [DataMember(Code = (byte)RoomParameterKey.TargetId, IsOptional = true)]
        public int TargetId { get; set; }
        [DataMember(Code = (byte)RoomParameterKey.TargetName, IsOptional = true)]
        public string TargetName { get; set; }
        [DataMember(Code = (byte)RoomParameterKey.WinnerId, IsOptional = true)]
        public int WinnerId { get; set; }
    }

}

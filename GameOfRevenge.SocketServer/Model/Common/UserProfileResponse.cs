using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class UserProfileResponse : Location
    {
        [DataMember(Code = (byte)RoomParameterKey.PlayerId, IsOptional = false)]
        public int PlayerId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Username, IsOptional = false)]
        public string UserName { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.AllianceId, IsOptional = false)]
        public int AllianceId { get; set; }
    }
}

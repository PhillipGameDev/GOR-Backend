using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class IaExitResponse : CommonResponse
    {
        [DataMember(Code = (byte)RoomParameterKey.PlayerId, IsOptional = false)]
        public int playerId { get; set; }
    }
}

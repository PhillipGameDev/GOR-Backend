using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class IaExitResponse : CommonResponse
    {
        [DataMember(Code = (byte)RoomParameterKey.PlayerId, IsOptional = false)]
        public int PlayerId { get; set; }

        public IaExitResponse(int playerId)
        {
            PlayerId = playerId;
        }
    }
}

using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class AttackResultResponse : CommonResponse
    {
        [DataMember(Code = (byte)RoomParameterKey.WinnerId, IsOptional = true)]
        public int WinnerId { get; set; }
    }

}

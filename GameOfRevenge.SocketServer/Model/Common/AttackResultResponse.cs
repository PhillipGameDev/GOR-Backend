using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class AttackResultResponse : CommonResponse
    {
        [DataMember(Code = (byte)RoomParameterKey.WinnerUserName, IsOptional = true)]
        public string WinnerUserName { get; set; }
    }

}

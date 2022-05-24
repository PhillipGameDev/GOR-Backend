using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class IaEnterResponse : Location
    {
        [DataMember(Code = (byte)RoomParameterKey.UserName, IsOptional = false)]
        public string UserName { get; set; }
    }
}

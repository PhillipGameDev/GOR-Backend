using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class Location : CommonResponse
    {
        [DataMember(Code = (byte)RoomParameterKey.X, IsOptional = false)]
        public float X { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Y, IsOptional = false)]
        public float Y { get; set; }
    }
}

using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class CommonResponse
    {
        [DataMember(Code = (byte)RoomParameterKey.CommonMessage, IsOptional = false)]
        public string Message { get; set; } = "OK";

        [DataMember(Code = (byte)RoomParameterKey.IsSuccess, IsOptional = false)]
        public bool IsSuccess { get; set; } = true;
    }
}

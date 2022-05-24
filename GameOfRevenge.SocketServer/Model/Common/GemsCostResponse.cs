using Photon.SocketServer.Rpc;
using GameOfRevenge.Helpers;

namespace GameOfRevenge.Model
{
    public class GemsCostResponse : DictionaryEncode
    {

        [DataMember(Code = (byte)RoomParameterKey.SpeedUpCost, IsOptional = true)]
        public int GemsCost { get; set; }
    }
}

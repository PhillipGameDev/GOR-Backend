using Photon.SocketServer.Rpc;
using GameOfRevenge.Helpers;

namespace GameOfRevenge.Model
{
    public class InstantResearchResponse : DictionaryEncode
    {
        [DataMember(Code = (byte)RoomParameterKey.AcademyItemId, IsOptional = false)]
        public int ItemId { get; set; }
    }
}

using GameOfRevenge.Common.Models.Quest;
using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class QuestUpdateResponse : CommonResponse
    {
        [DataMember(Code = (byte)RoomParameterKey.QuestId, IsOptional = false)]
        public int QuestId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Completed, IsOptional = false)]
        public bool Completed { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.ProgressData, IsOptional = true)]
        public string ProgressData { get; set; }
    }
}

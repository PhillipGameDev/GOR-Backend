using GameOfRevenge.Common.Models.Quest;
using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class QuestUpdateResponse : CommonResponse
    {
        [DataMember(Code = (byte)RoomParameterKey.QuestId, IsOptional = false)]
        public int QuestId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.MilestoneId, IsOptional = false)]
        public int MilestoneId { get; set; }

//        [DataMember(Code = (byte)RoomParameterKey.Name, IsOptional = false)]
//        public string Name { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.QuestType, IsOptional = false)]
        public QuestType QuestType { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Completed, IsOptional = false)]
        public bool Completed { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.InitialData, IsOptional = false)]
        public string InitialData { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.ProgressData, IsOptional = false)]
        public string ProgressData { get; set; }
    }
}

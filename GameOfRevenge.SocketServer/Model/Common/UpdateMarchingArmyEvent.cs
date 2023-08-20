using Photon.SocketServer.Rpc;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Helpers;

namespace GameOfRevenge.Model.Common
{
    public class UpdateMarchingArmyEvent : DictionaryEncode
    {
        [DataMember(Code = (byte)RoomParameterKey.MarchingId, IsOptional = true)]
        public long MarchingId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Recall, IsOptional = true)]
        public int Recall { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.AdvanceReduction, IsOptional = true)]
        public int AdvanceReduction { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.ReturnReduction, IsOptional = true)]
        public int ReturnReduction { get; set; }

        public UpdateMarchingArmyEvent(MarchingArmy marchingArmy)
        {
            MarchingId = marchingArmy.MarchingId;
            Recall = marchingArmy.Recall;
            AdvanceReduction = marchingArmy.AdvanceReduction;
            ReturnReduction = marchingArmy.ReturnReduction;
        }
    }
}

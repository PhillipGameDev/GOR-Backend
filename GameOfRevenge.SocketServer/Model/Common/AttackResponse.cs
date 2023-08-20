using Photon.SocketServer.Rpc;
using GameOfRevenge.Common.Models;

namespace GameOfRevenge.Model
{
    public class AttackResponse : CommonResponse
    {
        [DataMember(Code = (byte)RoomParameterKey.MarchingId, IsOptional = true)]
        public long MarchingId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.MarchingType, IsOptional = true)]
        public string MarchingType { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.AttackerId, IsOptional = true)]
        public int AttackerId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Username, IsOptional = true)]
        public string AttackerName { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TargetId, IsOptional = true)]
        public int TargetId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TargetName, IsOptional = true)]
        public string TargetName { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Invaded, IsOptional = true)]
        public int MonsterId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.StartTime, IsOptional = true)]
        public string StartTime { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Recall, IsOptional = true)]
        public int Recall { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Distance, IsOptional = true)]
        public int Distance { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.AdvanceReduction, IsOptional = true)]
        public int AdvanceReduction { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.ReturnReduction, IsOptional = true)]
        public int ReturnReduction { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Duration, IsOptional = true)]
        public int Duration { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.KingLevel, IsOptional = true)]
        public byte KingLevel { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.StructureLevel, IsOptional = true)]
        public byte WatchLevel { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TroopsData, IsOptional = true)]
        public int[] Troops { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.HeroesData, IsOptional = true)]
        public int[] Heroes { get; set; }

        public AttackResponse(AttackResponseData res)
        {
            MarchingId = res.MarchingId;
            MarchingType = res.MarchingType.ToString();

            AttackerId = res.AttackerId;
            AttackerName = res.AttackerName;

            TargetId = res.TargetId;
            TargetName = res.TargetName;

            MonsterId = res.MonsterId;//obsolete, use marchingType+targetId

            StartTime = res.StartTime;
            Recall = res.Recall;
            Distance = res.Distance;
            AdvanceReduction = res.AdvanceReduction;
            ReturnReduction = res.ReturnReduction;
            Duration = res.Duration;

            KingLevel = res.KingLevel;
            WatchLevel = res.WatchLevel;

            Troops = res.Troops;
            Heroes = res.Heroes;
        }
    }
}

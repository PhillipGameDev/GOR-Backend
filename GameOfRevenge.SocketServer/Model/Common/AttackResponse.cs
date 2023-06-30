using Photon.SocketServer.Rpc;
using GameOfRevenge.Common.Models;

namespace GameOfRevenge.Model
{
    public class AttackResponse : CommonResponse
    {
        [DataMember(Code = (byte)RoomParameterKey.AttackerId, IsOptional = true)]
        public int AttackerId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Username, IsOptional = true)]
        public string AttackerUsername { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TargetPlayerId, IsOptional = true)]
        public int EnemyId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TargetUsername, IsOptional = true)]
        public string EnemyUsername { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Invaded, IsOptional = true)]
        public int MonsterId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.StartTime, IsOptional = true)]
        public string StartTime { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.ReachedTime, IsOptional = true)]
        public int ReachedTime { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Duration, IsOptional = true)]
        public int BattleDuration { get; set; }

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
            AttackerId = res.AttackerId;
            AttackerUsername = res.AttackerUsername;

            EnemyId = res.EnemyId;
            EnemyUsername = res.EnemyUsername;
            MonsterId = res.MonsterId;

            KingLevel = res.KingLevel;
            WatchLevel = res.WatchLevel;

            Troops = res.Troops;
            Heroes = res.Heroes;

            StartTime = res.StartTime;
            BattleDuration = res.BattleDuration;
            ReachedTime = res.ReachedTime;
        }
    }
}

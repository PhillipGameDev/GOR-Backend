using Photon.SocketServer.Rpc;
using GameOfRevenge.Common.Models;

namespace GameOfRevenge.Model
{
    public class AttackResponse : CommonResponse
    {
        public AttackResponse(AttackSocketResponse res)
        {
            AttackerId = res.AttackerId;
            AttackerUsername = res.AttackerUsername;

            EnemyId = res.EnemyId;
            EnemyUsername = res.EnemyUsername;

            ReachedTime = res.ReachedTime;

            if (res.KingLevel >= 0) KingLevel = res.KingLevel;

            TroopCount = res.TroopCount;
            TroopType = res.TroopType;

            HeroIds = res.HeroIds;

            StartTime = res.StartTime.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
            BattleDuration = res.BattleDuration;
//            CurrentTime = res.CurrentTime;
//            IsReturnFromAttack = res.IsReturnFromAttack;
        }

        [DataMember(Code = (byte)RoomParameterKey.AttackerId, IsOptional = true)]
        public int AttackerId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Username, IsOptional = true)]
        public string AttackerUsername { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.EnemyId, IsOptional = true)]
        public int EnemyId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.EnemyUsername, IsOptional = true)]
        public string EnemyUsername { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.StartTime, IsOptional = true)]
        public string StartTime { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.ReachedTime, IsOptional = true)]
        public int ReachedTime { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.KingLevel, IsOptional = true)]
        public int KingLevel { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TroopCount, IsOptional = true)]
        public int[] TroopCount { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TroopType, IsOptional = true)]
        public int[] TroopType { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.HeroIds, IsOptional = true)]
        public int[] HeroIds { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.BattleDuration, IsOptional = true)]
        public int BattleDuration { get; set; }

//        [DataMember(Code = (byte)RoomParameterKey.IsReturnFromAttack, IsOptional = true)]
//        public bool IsReturnFromAttack { get; set; }

    }
}

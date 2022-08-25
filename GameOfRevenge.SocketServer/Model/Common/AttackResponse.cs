using Photon.SocketServer.Rpc;
using GameOfRevenge.Common.Models;

namespace GameOfRevenge.Model
{
    public class AttackResponse : CommonResponse
    {
        public AttackResponse(AttackSocketResponse res)
        {
            AttckerUserName = res.AttckerUserName;
            EnemyUserName = res.EnemyUserName;
            ReachedTime = res.ReachedTime;
            TroopCount = res.TroopCount;
            TroopType = res.TroopType;
            HeroIds = res.Heros;
            CurrentTime = res.CurrentTime;
            IsReturnFromAttack = res.IsReturnFromAttack;
        }

        [DataMember(Code = (byte)RoomParameterKey.AttackerUserName, IsOptional = true)]
        public string AttckerUserName { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.EnemyUserName, IsOptional = true)]
        public string EnemyUserName { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.ReachedTime, IsOptional = true)]
        public int ReachedTime { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TroopCount, IsOptional = true)]
        public int[] TroopCount { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TroopType, IsOptional = true)]
        public int[] TroopType { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.HeroIds, IsOptional = true)]
        public int[] HeroIds { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.CurrentTime, IsOptional = true)]
        public double CurrentTime { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.IsReturnFromAttack, IsOptional = true)]
        public bool IsReturnFromAttack { get; set; }

    }
}

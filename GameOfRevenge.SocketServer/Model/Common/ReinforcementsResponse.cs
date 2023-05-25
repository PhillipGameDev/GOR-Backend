using Photon.SocketServer.Rpc;
using GameOfRevenge.Common.Models;

namespace GameOfRevenge.Model
{
    public class ReinforcementsResponse : CommonResponse
    {
        [DataMember(Code = (byte)RoomParameterKey.PlayerId, IsOptional = true)]
        public int PlayerId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Username, IsOptional = true)]
        public string PlayerUsername { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TargetPlayerId, IsOptional = true)]
        public int TargetPlayerId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TargetUsername, IsOptional = true)]
        public string TargetPlayerUsername { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TroopsData, IsOptional = true)]
        public int[] Troops { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.HeroesData, IsOptional = true)]
        public int[] Heroes { get; set; }

        public ReinforcementsResponse()
        {
        }

        public ReinforcementsResponse(AttackSocketResponse res)
        {
            PlayerId = res.AttackerId;
            PlayerUsername = res.AttackerUsername;

            TargetPlayerId = res.EnemyId;
            TargetPlayerUsername = res.EnemyUsername;

            Troops = res.Troops;

            Heroes = res.Heroes;
        }
    }
}

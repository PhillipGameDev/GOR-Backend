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

        [DataMember(Code = (byte)RoomParameterKey.TargetId, IsOptional = true)]
        public int TargetPlayerId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TargetName, IsOptional = true)]
        public string TargetPlayerUsername { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TroopsData, IsOptional = true)]
        public int[] Troops { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.HeroesData, IsOptional = true)]
        public int[] Heroes { get; set; }

        public ReinforcementsResponse()
        {
        }

        public ReinforcementsResponse(AttackResponseData res)
        {
            PlayerId = res.AttackerId;
            PlayerUsername = res.AttackerName;

            TargetPlayerId = res.TargetId;
            TargetPlayerUsername = res.TargetName;

            Troops = res.Troops;

            Heroes = res.Heroes;
        }
    }
}

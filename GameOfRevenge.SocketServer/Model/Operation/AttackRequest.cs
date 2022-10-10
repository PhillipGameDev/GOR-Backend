using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class AttackRequest : Operation
    {
        public AttackRequest(IRpcProtocol protocol, OperationRequest request) : base(protocol, request) { }


        [DataMember(Code = (byte)RoomParameterKey.EnemyId, IsOptional = true)]
        public int EnemyId { get; set; }
         
        [DataMember(Code = (byte)RoomParameterKey.TroopType, IsOptional = true)]
        public int[] TroopType { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TroopLevel, IsOptional = true)]
        public int[] TroopLevel { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TroopCount, IsOptional = true)]
        public int[] TroopCount { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.HeroIds, IsOptional = true)]
        public int[] HeroIds { get; set; }
    }
}

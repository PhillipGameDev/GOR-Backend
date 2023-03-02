﻿using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class AttackRequest : Operation
    {
        public AttackRequest(IRpcProtocol protocol, OperationRequest request) : base(protocol, request) { }


        [DataMember(Code = (byte)RoomParameterKey.TargetPlayerId, IsOptional = true)]
        public int EnemyId { get; set; }
         
        [DataMember(Code = (byte)RoomParameterKey.TroopsData, IsOptional = true)]
        public int[] Troops { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.HeroIds, IsOptional = true)]
        public int[] HeroIds { get; set; }
    }
}

﻿using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class SendArmyRequest : Operation
    {
        public SendArmyRequest(IRpcProtocol protocol, OperationRequest request) : base(protocol, request) { }

        [DataMember(Code = (byte)RoomParameterKey.TargetType, IsOptional = true)]
        public byte TargetType { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TargetId, IsOptional = true)]
        public int TargetId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TroopsData, IsOptional = true)]
        public int[] Troops { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.HeroIds, IsOptional = true)]
        public int[] HeroIds { get; set; }
    }
}

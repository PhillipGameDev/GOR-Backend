﻿using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Helpers;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class JoinUnionRequest : Operation
    {
        public JoinUnionRequest(IRpcProtocol protocol, OperationRequest request) : base(protocol, request) { }

        [DataMember(Code = (byte)RoomParameterKey.FromClanId, IsOptional = true)]
        public int FromClanId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.ToClanId, IsOptional = true)]
        public int ToClanId { get; set; }
    }

    public class JoinUnionResponse : DictionaryEncode
    {
        [DataMember(Code = (byte)RoomParameterKey.FromClanId, IsOptional = true)]
        public int FromClanId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.ToClanId, IsOptional = true)]
        public int ToClanId { get; set; }
    }
}
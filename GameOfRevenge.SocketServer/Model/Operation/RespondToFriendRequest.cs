﻿using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using GameOfRevenge.Helpers;

namespace GameOfRevenge.Model
{
    public class RespondToFriendRequest : Operation
    {
        public RespondToFriendRequest(IRpcProtocol protocol, OperationRequest request) : base(protocol, request) { }

        [DataMember(Code = (byte)RoomParameterKey.RequestId, IsOptional = false)]
        public long RequestId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TargetId, IsOptional = false)]
        public int TargetPlayerId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Value, IsOptional = false)]
        public byte Value { get; set; }
    }

    public class RespondToFriendRequestResponse : DictionaryEncode
    {
        [DataMember(Code = (byte)RoomParameterKey.RequestId, IsOptional = false)]
        public long RequestId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.PlayerId, IsOptional = false)]
        public int FromPlayerId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Username, IsOptional = true)]
        public string FromPlayerName { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TargetId, IsOptional = false)]
        public int ToPlayerId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Flags, IsOptional = false)]
        public byte Flags { get; set; }
    }
}

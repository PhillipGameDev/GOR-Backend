﻿using System.Collections.Generic;
using Photon.SocketServer.Rpc;
using GameOfRevenge.Helpers;

namespace GameOfRevenge.Model
{
    public class ChatMessageRespose : DictionaryEncode
    {
        [DataMember(Code = (byte)RoomParameterKey.ChatId, IsOptional = true)]
        public long ChatId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.PlayerId, IsOptional = true)]
        public int PlayerId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Username, IsOptional = true)]
        public string Username { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.VIPPoints, IsOptional = true)]
        public int VIPPoints { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.AllianceId, IsOptional = true)]
        public int AllianceId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Timestamp, IsOptional = true)]
        public long Timestamp { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.ChatMessage, IsOptional = true)]
        public string ChatMessage { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Flags, IsOptional = true)]
        public byte Flags { get; set; }
    }
}

﻿using System;
using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class UserProfileResponse : Location
    {
        [DataMember(Code = (byte)RoomParameterKey.PlayerId, IsOptional = false)]
        public int PlayerId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Username, IsOptional = false)]
        public string UserName { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.AllianceId, IsOptional = false)]
        public int AllianceId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.VIPPoints, IsOptional = false)]
        public int VIPPoints { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.KingLevel, IsOptional = false)]
        public byte KingLevel { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.CastleLevel, IsOptional = false)]
        public byte CastleLevel { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Timestamp, IsOptional = false)]
        public string LastLogin { get; set; }
    }
}

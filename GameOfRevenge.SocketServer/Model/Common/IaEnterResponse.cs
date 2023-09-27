using System;
using Photon.SocketServer.Rpc;
using GameOfRevenge.GameHandlers;

namespace GameOfRevenge.Model
{
    public class IaEnterResponse : Location
    {
        [DataMember(Code = (byte)RoomParameterKey.PlayerId, IsOptional = false)]
        public int PlayerId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Username, IsOptional = false)]
        public string Username { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.KingLevel, IsOptional = false)]
        public byte KingLevel { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.CastleLevel, IsOptional = false)]
        public byte CastleLevel { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.WatchLevel, IsOptional = false)]
        public byte WatchLevel { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.ShieldEndTime, IsOptional = false)]
        public string ShieldEndTime { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Invaded, IsOptional = true)]
        public int Invaded { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.VIPPoints, IsOptional = false)]
        public int VIPPoints { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.AllianceId, IsOptional = false)]
        public int AllianceId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Timestamp, IsOptional = false)]
        public string LastLogin { get; set; }

        public IaEnterResponse(PlayerInstance player)
        {
            X = player.WorldRegion.X;
            Y = player.WorldRegion.Y;
            PlayerId = player.PlayerId;
            Username = player.PlayerInfo.Name;

            KingLevel = player.PlayerInfo.KingLevel;
            CastleLevel = player.PlayerInfo.CastleLevel;
            WatchLevel = player.PlayerInfo.WatchLevel;
            ShieldEndTime = player.PlayerInfo.ShieldEndTime.ToUniversalTime().ToString("s") + "Z";
            Invaded = 0;

            VIPPoints = player.PlayerInfo.VIPPoints;
            AllianceId = player.PlayerInfo.AllianceId;
            LastLogin = player.PlayerInfo.LastLogin.ToUniversalTime().ToString("s") + "Z";
        }
    }
}

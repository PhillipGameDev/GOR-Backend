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

        [DataMember(Code = (byte)RoomParameterKey.Invaded, IsOptional = false)]
        public int Invaded { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.VIPPoints, IsOptional = false)]
        public int VIPPoints { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.AllianceId, IsOptional = false)]
        public int AllianceId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Timestamp, IsOptional = false)]
        public string LastLogin { get; set; }

        public IaEnterResponse(MmoActor actor)
        {
            X = actor.WorldRegion.X;
            Y = actor.WorldRegion.Y;
            PlayerId = actor.PlayerId;
            Username = actor.PlayerData.Name;

            KingLevel = actor.PlayerData.KingLevel;
            CastleLevel = actor.PlayerData.CastleLevel;
            WatchLevel = actor.PlayerData.WatchLevel;
            ShieldEndTime = actor.PlayerData.ShieldEndTime.ToUniversalTime().ToString("s") + "Z";
            Invaded = actor.PlayerData.Invaded;

            VIPPoints = actor.PlayerData.VIPPoints;
            AllianceId = actor.PlayerData.AllianceId;
            LastLogin = actor.PlayerData.LastLogin.ToUniversalTime().ToString("s") + "Z";
        }
    }
}

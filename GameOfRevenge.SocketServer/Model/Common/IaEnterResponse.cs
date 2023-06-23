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

        public IaEnterResponse(MmoActor actor)
        {
            X = actor.WorldRegion.X;
            Y = actor.WorldRegion.Y;
            PlayerId = actor.PlayerId;
            Username = actor.PlayerData.Name;
            AllianceId = actor.PlayerData.AllianceId;
            VIPPoints = actor.PlayerData.VIPPoints;
            KingLevel = actor.PlayerData.KingLevel;
            CastleLevel = actor.PlayerData.CastleLevel;
            LastLogin = actor.PlayerData.LastLogin.ToUniversalTime().ToString("s") + "Z";
        }
    }
}

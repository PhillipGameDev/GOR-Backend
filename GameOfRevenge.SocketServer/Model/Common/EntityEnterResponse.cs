using GameOfRevenge.Common;
using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class EntityEnterResponse : Location
    {
        [DataMember(Code = (byte)RoomParameterKey.Flags, IsOptional = false)]
        public int Seed { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TargetType, IsOptional = false)]
        public byte EntityType { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TargetId, IsOptional = false)]
        public int EntityId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.HitPoints, IsOptional = false)]
        public int HitPoints { get; set; }

        public EntityEnterResponse(int x, int y, int seed, EntityType entityType, int entityId, int hitPoints)
        {
            X = x;
            Y = y;
            Seed = seed;
            EntityType = (byte)entityType;
            EntityId = entityId;
            HitPoints = hitPoints;
        }
    }
}

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

    public class FortressEnterResponse : EntityEnterResponse
    {
        [DataMember(Code = (byte)RoomParameterKey.Attack, IsOptional = false)]
        public int Attack { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Defense, IsOptional = false)]
        public int Defense { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.AllianceId, IsOptional = false)]
        public int ClanId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Name, IsOptional = true)]
        public string Name { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.PlayerId, IsOptional = true)]
        public int PlayerId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.StartTime, IsOptional = true)]
        public string StartTime { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.Duration, IsOptional = true)]
        public int Duration { get; set; }

//        [DataMember(Code = (byte)RoomParameterKey.TroopsData, IsOptional = true)]
//        public string PlayerTroops { get; set; }

        public FortressEnterResponse(int x, int y, int seed, EntityType entityType, int entityId, int hitPoints, int attack, int defense) :
                base(x, y, seed, entityType, entityId, hitPoints)
        {
            Attack = attack;
            Defense = defense;
        }
    }
}

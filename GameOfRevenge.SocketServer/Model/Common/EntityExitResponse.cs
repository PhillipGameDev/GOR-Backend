using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class EntityExitResponse : CommonResponse
    {
        [DataMember(Code = (byte)RoomParameterKey.TargetType, IsOptional = false)]
        public byte EntityType { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TargetId, IsOptional = false)]
        public int EntityId { get; set; }

        public EntityExitResponse(byte entityType, int entityId)
        {
            EntityType = entityType;
            EntityId = entityId;
        }
    }
}

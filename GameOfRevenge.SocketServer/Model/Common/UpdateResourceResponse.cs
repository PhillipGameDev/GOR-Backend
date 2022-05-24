using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class UpdateResourceResponse : CommonResponse
    {
        public UpdateResourceResponse(int resourceId, float val)
        {
            ResourceId = resourceId;
            ResourceValue = val;
        }

        [DataMember(Code = (byte)RoomParameterKey.ResourceId, IsOptional = false)]
        public int ResourceId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.ResourceValue, IsOptional = false)]
        public float ResourceValue { get; set; }
    }
}

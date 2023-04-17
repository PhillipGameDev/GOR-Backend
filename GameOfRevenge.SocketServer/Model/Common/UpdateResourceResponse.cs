using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class UpdateResourceResponse : CommonResponse
    {
        public UpdateResourceResponse(int resourceId, long val)
        {
            ResourceId = resourceId;
            ResourceTotal = val;
        }

        [DataMember(Code = (byte)RoomParameterKey.ResourceId, IsOptional = false)]
        public int ResourceId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.ResourceTotal, IsOptional = false)]
        public long ResourceTotal { get; set; }
    }
}

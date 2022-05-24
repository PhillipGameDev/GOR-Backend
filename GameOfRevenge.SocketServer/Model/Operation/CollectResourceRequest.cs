using Photon.SocketServer;

namespace GameOfRevenge.Model
{
    public class CollectResourceRequest : CommonBuildingRequest
    {
        public CollectResourceRequest(IRpcProtocol protocol, OperationRequest request) : base(protocol, request) { }
    }
}

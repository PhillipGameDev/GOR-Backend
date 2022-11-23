using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class UpdateQuestRequest : Operation
    {
        public UpdateQuestRequest(IRpcProtocol protocol, OperationRequest request) : base(protocol, request) { }

        [DataMember(Code = (byte)RoomParameterKey.QuestId, IsOptional = false)]
        public int QuestId { get; set; }
    }
}

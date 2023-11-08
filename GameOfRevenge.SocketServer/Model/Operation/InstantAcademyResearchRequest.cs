using GameOfRevenge.Common.Models.Structure;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class InstantAcademyResearchRequest : Operation
    {
        public InstantAcademyResearchRequest(IRpcProtocol protocol, OperationRequest request) : base(protocol, request) { }

        [DataMember(Code = (byte)RoomParameterKey.AcademyItemId, IsOptional = true)]
        public int ItemId { get; set; }
    }
}

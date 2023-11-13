using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Helpers;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class DeleteClanRequest : Operation
    {
        public DeleteClanRequest(IRpcProtocol protocol, OperationRequest request) : base(protocol, request) { }

        [DataMember(Code = (byte)RoomParameterKey.PlayerId, IsOptional = true)]
        public int PlayerId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.ClanId, IsOptional = true)]
        public int ClanId { get; set; }
    }

    public class DeleteClanResponse : DictionaryEncode
    {
    }
}

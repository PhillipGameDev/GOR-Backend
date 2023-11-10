using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Helpers;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Model
{
    public class CreateClanRequest : Operation
    {
        public CreateClanRequest(IRpcProtocol protocol, OperationRequest request) : base(protocol, request) { }

        [DataMember(Code = (byte)RoomParameterKey.ClanName, IsOptional = true)]
        public string ClanName { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.ClanTag, IsOptional = true)]
        public string ClanTag { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.ClanDescription, IsOptional = true)]
        public string ClanDescription { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.EmbassyLevel, IsOptional = true)]
        public int EmbassyLevel { get; set; }
    }

    public class CreateClanResponse : DictionaryEncode
    {
    }
}

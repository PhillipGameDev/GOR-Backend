using System.Collections.Generic;
using Photon.SocketServer.Rpc;
using GameOfRevenge.Helpers;

namespace GameOfRevenge.Model
{
    public class UpgradeTechnologyResponse : DictEncode
    {
        public UpgradeTechnologyResponse(ref Dictionary<byte, object> dict) : base(ref dict) { }

        [DataMember(Code = (byte)RoomParameterKey.StructureLocationId, IsOptional = true)]
        public int BuildingLocationId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TechLevel, IsOptional = true)]
        public int Level { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TechType, IsOptional = true)]
        public int TechType { get; set; }
    }
}

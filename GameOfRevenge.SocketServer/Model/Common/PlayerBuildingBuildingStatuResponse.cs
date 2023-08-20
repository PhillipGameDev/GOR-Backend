using System.Collections.Generic;
using Photon.SocketServer.Rpc;
using GameOfRevenge.Helpers;

namespace GameOfRevenge.Model
{
    public class PlayerBuildingBuildingStatuResponse : DictionaryEncode
    {
        [DataMember(Code = (byte)RoomParameterKey.StructureLocationId, IsOptional = false)]
        public int LocationId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TimeLeft, IsOptional = true)]
        public int TimeLeft { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TotalTime, IsOptional = true)]
        public int TotalTime { get; set; }
    }
}

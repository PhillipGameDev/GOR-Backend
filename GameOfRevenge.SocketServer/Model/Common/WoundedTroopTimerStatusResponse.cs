using System.Collections.Generic;
using Photon.SocketServer.Rpc;
using GameOfRevenge.Helpers;

namespace GameOfRevenge.Model
{
    public class WoundedTroopTimerStatusResponse : DictEncode
    {
        public WoundedTroopTimerStatusResponse(ref Dictionary<byte, object> dict) : base(ref dict) { }

        [DataMember(Code = (byte)RoomParameterKey.StructureLocationId, IsOptional = true)]
        public int BuildingLocationId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TotalTime, IsOptional = true)]
        public double TotalHealTime { get; set; }
    }
}

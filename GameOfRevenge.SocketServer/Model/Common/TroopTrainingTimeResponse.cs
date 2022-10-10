using System.Collections.Generic;
using Photon.SocketServer.Rpc;
using GameOfRevenge.Helpers;

namespace GameOfRevenge.Model
{
    public class TroopTrainingTimeResponse : DictionaryEncode
    {
        [DataMember(Code = (byte)RoomParameterKey.StructureType, IsOptional = true)]
        public int StructureType { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.StructureLocationId, IsOptional = true)]
        public int LocationId { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TrainingTime, IsOptional = true)]
        public int TrainingTime { get; set; }

        [DataMember(Code = (byte)RoomParameterKey.TotalTime, IsOptional = true)]
        public int TotalTime { get; set; }
    }
}

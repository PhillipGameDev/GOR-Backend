using GameOfRevenge.Common.Models.Structure;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GameOfRevenge.Common.Models
{
    public class StructureInfos
    {
        public StructureType StructureType { get; set; }
        public List<StructureDetails> Buildings { get; set; }
    }

    [DataContract]
    public class StructureDetails : TimerBase
    {
        [DataMember]
        public int Level { get; set; }
        [DataMember]
        public int Location { get; set; }
        [DataMember]
        public DateTime LastCollected { get; set; }
        [DataMember]
        public int HitPoints { get; set; }
        [DataMember]
        public int Helped { get; set; }
    }
}

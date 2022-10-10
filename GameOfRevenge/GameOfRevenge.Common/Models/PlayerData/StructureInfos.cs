using GameOfRevenge.Common.Models.Structure;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GameOfRevenge.Common.Models
{
    public class StructureInfos
    {
        public long Id { get; set; }
        public StructureType StructureType { get; set; }
        public List<StructureDetails> Buildings { get; set; }
    }

    [DataContract]
    public class StructureDetails : TimerBase//, IComparer<StructureDetails>
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

/*        public int Compare(StructureDetails x, StructureDetails y)
        {
            return x.Level.CompareTo(y.Level);
        }*/
    }
}

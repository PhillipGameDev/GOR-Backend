﻿using System.Runtime.Serialization;

namespace GameOfRevenge.Common.Models
{
    [DataContract]
    public class TechnologyInfos : TimerBase
    {
        [DataMember]
        public TechnologyType TechnologyType { get; set; }
        [DataMember]
        public int Level { get; set; }
        [DataMember]
        public string Code { get => TechnologyType.ToString(); }
    }
}

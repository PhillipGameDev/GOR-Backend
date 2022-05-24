using GameOfRevenge.Common.Models.Structure;
using System;
using System.Collections.Generic;

namespace GameOfRevenge.Common.Models
{
    public class StructureInfos
    {
        public StructureType StructureType { get; set; }
        public List<StructureDetails> Buildings { get; set; }
    }

    public class StructureDetails : TimerBase
    {
        public int Level { get; set; }
        public int Location { get; set; }
        public DateTime LastCollected { get; set; }
        public int HitPoints { get; set; }
    }
}

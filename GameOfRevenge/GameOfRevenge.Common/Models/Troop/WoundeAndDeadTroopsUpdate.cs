using System;
using System.Collections.Generic;
using System.Text;

namespace GameOfRevenge.Common.Models.Troop
{
    public class WoundeAndDeadTroopsUpdate
    {
        public int Level { get; set; }
        public int WoundedCount { get; set; }
        public int DeadCount { get; set; }
        public int BuildingLocation { get; set; }
    }
}

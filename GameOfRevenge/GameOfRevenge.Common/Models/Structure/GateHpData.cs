using System.Collections.Generic;

namespace GameOfRevenge.Common.Models.Structure
{
    public class GateHpData
    {
        public int CurrentHp { get; set; }
        public int MaxHp { get; set; }
        public int MissingHp { get => MaxHp - CurrentHp; }
        public List<DataRequirement> RepairCost { get; set; }
    }
}

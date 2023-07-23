using System.Collections.Generic;
using GameOfRevenge.Common.Models;

namespace GameOfRevenge.WebAdmin.Models
{
    public class InputStructureModel
    {
        public int PlayerId { get; set; }
        public string StructureType { get; set; }
        public int StructureLocation { get; set; }
        public string StructureValues { get; set; }

        public StructureDetails Structure { get; set; }

        public InputStructureModel()
        {
        }
    }

    public class StructuresAndTroops
    {
        public List<StructureInfos> Structures { get; set; }
        public List<TroopInfos> Troops { get; set; }
    }
}

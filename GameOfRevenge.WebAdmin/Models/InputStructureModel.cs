using System.Collections.Generic;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Structure;

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

        public int CastleLevel
        {
            get
            {
                var lvl = 0;
                var structures = Structures?.Find(x => (x.StructureType == StructureType.CityCounsel));
                if ((structures != null) && (structures.Buildings?.Count > 0))
                {
                    lvl = structures.Buildings[0].CurrentLevel;
                }

                return lvl;
            }
        }
    }
}

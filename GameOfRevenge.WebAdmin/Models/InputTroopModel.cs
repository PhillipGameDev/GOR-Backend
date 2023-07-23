using System.Collections.Generic;
using GameOfRevenge.Common.Models;

namespace GameOfRevenge.WebAdmin.Models
{
    public class InputTroopModel
    {
        public int PlayerId { get; set; }
        public string TroopType { get; set; }
        public string TroopValues { get; set; }

        public List<TroopDetails> Options { get; set; }

        public static long[] Values(TroopDetails troop) => new long[] { troop.Wounded, troop.InRecoveryCount, troop.InTrainingCount, troop.Count, troop.FinalCount };

        public InputTroopModel()
        {
        }
    }
}

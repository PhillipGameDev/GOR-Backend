using System.Collections.Generic;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models;

namespace GameOfRevenge.WebAdmin.Models
{
    public class InputRewardsModel
    {
        public int PlayerId { get; set; }
        public string RewardValues { get; set; }
        public bool ApplyToAll { get; set; }

        public List<DataType> DataTypes { get; set; }
        public List<(int, DataType, string)> Options { get; set; }

        public static long[] Values(DataReward reward) => new long[] { reward.ValueId, reward.Value, reward.Count };

        public InputRewardsModel()
        {
        }
    }
}

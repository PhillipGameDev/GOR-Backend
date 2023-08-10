using System.Collections.Generic;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models;

namespace GameOfRevenge.WebAdmin.Models
{
    public class InputPackageRewardsModel
    {
        public int PackageId { get; set; }
        public int QuestId { get; set; }
        public string RewardValues { get; set; }

        public List<DataType> DataTypes { get; set; }
        public List<(int, DataType, string)> Options { get; set; }
        public List<(DataType, int, string, string)> SubOptions { get; set; }

        public static long[] Values(DataReward reward) => new long[] { reward.ValueId, reward.Value, reward.Count };

        public InputPackageRewardsModel()
        {
        }
    }
}

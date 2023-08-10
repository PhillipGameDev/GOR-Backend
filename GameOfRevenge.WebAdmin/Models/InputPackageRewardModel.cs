namespace GameOfRevenge.WebAdmin.Models
{
    public class InputPackageRewardModel
    {
        public int PackageId { get; set; }
        public int QuestId { get; set; }
        public int RewardId { get; set; }
        public int RewardValue { get; set; }

        public string Description { get; set; }
        public int Value { get; set; }

        public InputPackageRewardModel()
        {
        }
    }
}

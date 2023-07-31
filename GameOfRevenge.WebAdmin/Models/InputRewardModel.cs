namespace GameOfRevenge.WebAdmin.Models
{
    public class InputRewardModel
    {
        public int PlayerId { get; set; }
        public long PlayerDataId { get; set; }
        public int RewardValue { get; set; }

        public string Description { get; set; }
        public int Value { get; set; }

        public InputRewardModel()
        {
        }
    }
}

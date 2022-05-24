using GameOfRevenge.Common.Models.Hero;

namespace GameOfRevenge.Common.Models
{
    public class UserHeroDetails
    {
        public int HeroId { get; set; }
        public string Code { get; set; }

        public bool IsMarching { get; set; }
        public bool Unlocked { get; set; }
        public bool IsAvaliable { get => !IsMarching && Unlocked; }

        public float WarPoints { get; set; }
    }
}

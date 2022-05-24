namespace GameOfRevenge.Common.Models.Kingdom
{
    public class BattleReport
    {
        public ClientBattleReport Attacker { get; set; }
        public ClientBattleReport Defender { get; set; }
        public bool AttackerWon { get; set; }
        public string Message { get; set; }
    }
}

namespace GameOfRevenge.Common.Models
{
    public class TechnologyInfos : TimerBase
    {
        public TechnologyType TechnologyType { get; set; }
        public int Level { get; set; }
        public string Code { get => TechnologyType.ToString(); }
    }
}

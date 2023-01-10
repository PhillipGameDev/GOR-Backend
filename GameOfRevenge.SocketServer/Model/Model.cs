using System;
using GameOfRevenge.Common.Models;

namespace GameOfRevenge.Model
{

    public class BoostUpResource : TimerBase
    {
//        public DateTime StartTime { get; set; }
//        public DateTime EndTime { get; set; }
//        public int Duration { get; set; }
        public int Multiplier { get; set; }

/*        public double TimeLeft
        {
            get
            {
                double totalSeconds = (EndTime - DateTime.UtcNow).TotalSeconds;
                return totalSeconds <= 0 ? 0 : totalSeconds;
            }
        }*/
    }

/*    public class MultiplierItem
    {
        public int Multiplier { get; set; }
        public double Time { get; set; }
    }*/
}

using System;

namespace GameOfRevenge.Common.Models
{
    public class TimerBase
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        
        public double TimeLeft
        {
            get
            {
                double totalSeconds = (EndTime - DateTime.UtcNow).TotalSeconds;
                return totalSeconds <= 0 ? 0 : totalSeconds;
            }
        }

        public double TotalTime
        {
            get
            {
                double totalSeconds = (EndTime - StartTime).TotalSeconds;
                return totalSeconds <= 0 ? 0 : totalSeconds;
            }
        }
    }
}
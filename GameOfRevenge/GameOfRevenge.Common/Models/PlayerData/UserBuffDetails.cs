using GameOfRevenge.Common.Models.Inventory;
using System;

namespace GameOfRevenge.Common.Models
{
    public class UserBuffDetails
    {
        public BuffType BuffType { get; set; }
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
    }
}

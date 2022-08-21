using System;
using System.Runtime.Serialization;

namespace GameOfRevenge.Common.Models
{
    [DataContract]
    public class TimerBase
    {
        [DataMember]
        public DateTime StartTime { get; set; }
        [DataMember]
        public DateTime EndTime { get; set; }

//        [DataMember]
        public double TimeLeft
        {
            get
            {
                double totalSeconds = (EndTime - DateTime.UtcNow).TotalSeconds;
                return totalSeconds <= 0 ? 0 : totalSeconds;
            }
        }

//        [DataMember]
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
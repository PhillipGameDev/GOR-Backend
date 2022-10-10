using System;
using System.Runtime.Serialization;

namespace GameOfRevenge.Common.Models
{
    [DataContract]
    public class TimerBase
    {
        [DataMember]
        public DateTime StartTime { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int Duration { get; set; }
//        public DateTime EndTime { get; set; }

        public bool HasDuration => Duration > 0;
        public bool Started => (DateTime.UtcNow - StartTime.ToUniversalTime()).TotalMilliseconds > 0;

        public DateTime EndTime
        {
            get
            {
                return StartTime.AddSeconds(Duration);
            }
        }
//        [DataMember]
        public double TimeLeft
        {
            get
            {
                double seconds = 0;
                if (Duration > 0)
                {
                    seconds = (EndTime.ToUniversalTime() - DateTime.UtcNow).TotalSeconds;
                    if (seconds < 0) seconds = 0;
                }

                return seconds;
            }
        }

//        [DataMember]
/*        public double TotalTime
        {
            get
            {
                double totalSeconds = (EndTime - StartTime).TotalSeconds;
                return totalSeconds <= 0 ? 0 : totalSeconds;
            }
        }*/
    }
}
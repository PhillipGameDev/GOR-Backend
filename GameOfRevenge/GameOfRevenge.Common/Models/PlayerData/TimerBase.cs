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

        public bool HasDuration => Duration > 0;
        public bool Started => (DateTime.UtcNow - StartTime.ToUniversalTime()).TotalSeconds > 0;

        public DateTime EndTime => StartTime.AddSeconds(Duration);

        public float TimeLeft
        {
            get
            {
                float seconds = 0;
                if (Duration > 0)
                {
                    seconds = Duration - (float)(DateTime.UtcNow - StartTime.ToUniversalTime()).TotalSeconds;
                    if (seconds < 0) seconds = 0;
                }

                return seconds;
            }
        }
    }
}
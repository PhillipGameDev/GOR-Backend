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
        public bool Started => TimeSinceStart > 0;

        public DateTime EndTime => StartTime.AddSeconds(Duration);

        public float TimeLeft
        {
            get
            {
                float seconds = 0;
                if (Duration > 0)
                {
                    seconds = Duration - TimeSinceStart;
                    if (seconds < 0) seconds = 0;
                }

                return seconds;
            }
        }

        public float TimeSinceStart => (float)(DateTime.UtcNow - StartTime.ToUniversalTime()).TotalSeconds;
        public float TimeSinceEnd => (float)(DateTime.UtcNow - EndTime.ToUniversalTime()).TotalSeconds;

        public float TimeElapsed => (Duration - TimeLeft);
        public float Percentage => (TimeElapsed / Duration);
    }
}
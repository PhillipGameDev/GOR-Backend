using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GameOfRevenge.Common.Models
{
    [DataContract]
    public class UserKingDetails : TimerBase
    {
        private static readonly List<int> levels = new List<int>() { 0, 30, 120, 300, 600, 1000, 2000, 4000, 6000, 8000, 10000 };

        [DataMember]
        public int MaxStamina { get; set; } = 20;
        [DataMember]
        public int Experience { get; set; }

        [DataMember]
        public int BattleCount { get; set; }

#if UNITY_2019_4_OR_NEWER
        [DataMember]
        public int NextLevelXP { get; set; }
        [DataMember]
        public byte Level { get; set; }
#else
        [DataMember]
        public int NextLevelXP
        {
            get => ((Level + 1) >= levels.Count) ? levels[Level] : levels[Level + 1];
        }

        [DataMember]
        public byte Level
        {
            //TODO: improve this when we remove levels array from this class.
            //we should order levels and get the lowest value
            get => (byte)(levels.FindIndex(x => (x > Experience)) - 1);
        }
#endif

        public int Stamina
        {
            get
            {
                int stamina = MaxStamina;
                double timeLeft = TimeLeft;
                if (timeLeft > 0)
                {

                    stamina -= (int)Math.Ceiling(timeLeft / (60 * 10));
                    if (stamina < 0) stamina = 0;
                }
                return stamina;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
//using GameOfRevenge.Common.Models.Hero;
//using Newtonsoft.Json;

namespace GameOfRevenge.Common.Models
{
    [DataContract]
    public class UserKingDetails : TimerBase
    {
        private static readonly List<int> levels = new List<int>() { 0, 30, 120, 300, 600, 1000, 2000, 4000, 6000, 8000, 10000 };

        [DataMember]
        public int MaxStamina { get; set; } = 20;
        //        public long Id { get; set; }//id
        //        public string Code { get; set; }
        //        public int Experience { get; set; }
        [DataMember]
        public int Experience { get; set; }//value

#if UNITY_2019_4_OR_NEWER
        [DataMember]
        public int NextLevelXP { get; set; }
        [DataMember]
        public int Level { get; set; }
#else
        [DataMember]
        public int NextLevelXP
        {
            get => ((Level + 1) >= levels.Count) ? levels[Level] : levels[Level + 1];
        }

        [DataMember]
        public int Level
        {
            //TODO: improve this when we remove levels array from this class.
            //we should order levels and get the lowest value
            get => levels.FindIndex(x => (x > Experience)) - 1;
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

        /*        //        [JsonIgnore]
                //SERVER SIDE FUNCTION
                public int Level => 1 + (int)Math.Floor(WarPoints / 10f);// 10 warpoints = 1 level
                                                                         //        [JsonIgnore]
                                                                         //SERVER SIDE FUNCTION
        //        public int WarPoints => (int)Math.Floor(BattleCount / 5f);// 5 battles = 1 war point
        */

        //        public bool Unlocked { get; set; }
        //        public bool IsMarching { get; set; }
        //        [JsonIgnore]
        //        public bool IsAvaliable { get => !IsMarching && Unlocked; }
    }
}

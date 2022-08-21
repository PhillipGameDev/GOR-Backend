using System;
using System.Runtime.Serialization;
//using GameOfRevenge.Common.Models.Hero;
//using Newtonsoft.Json;

namespace GameOfRevenge.Common.Models
{
    [DataContract]
    public class UserKingDetails : TimerBase
    {
        [DataMember]
        public int MaxStamina { get; set; }
        //        public long Id { get; set; }//id
        //        public string Code { get; set; }
        //        public int Experience { get; set; }
        [DataMember]
        public int BattleCount { get; set; }//value

#if UNITY_2019_4_OR_NEWER
        public int Level { get; set; }
#else
        [DataMember]
        public int Level
        {
            get
            {
                if (BattleCount >= 600)
                    return 4;
                else if (BattleCount >= 300)
                    return 3;
                else if (BattleCount >= 120)
                    return 2;
                else if (BattleCount >= 30)
                    return 1;
                else
                    return 0;
            }
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

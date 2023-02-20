using System;
using System.Runtime.Serialization;
//using GameOfRevenge.Common.Models.Hero;
//using Newtonsoft.Json;

namespace GameOfRevenge.Common.Models
{
    [System.Serializable]
    [DataContract]
    public class UserHeroDetails
    {
        public const int UNLOCK_POINTS = 10;
//        public long Id { get; set; }//id

        [DataMember]
        public string HeroCode { get; set; }

        [DataMember]
        public int Points { get; set; }//1
        [DataMember]
        public int Power { get; set; }//2
        [DataMember]
        public int AttackCount { get; set; }//3
        [DataMember]
        public int AttackFail { get; set; }//4
        [DataMember]
        public int DefenseCount { get; set; }//5
        [DataMember]
        public int DefenseFail { get; set; }//6

//        public bool Unlocked { get; set; }
        [DataMember]
        public bool IsMarching { get; set; }
//        [JsonIgnore]
//        public bool IsAvaliable { get => !IsMarching && Unlocked; }

        public int Level => (int)Math.Floor(Points / (float)UNLOCK_POINTS);// 10 warpoints = 1 level
//        public int WarPoints => (int)Math.Floor(Points / 5f);// 5 battles = 1 war point

    }
}

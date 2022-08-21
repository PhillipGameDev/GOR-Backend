using System;
using System.Runtime.Serialization;
//using GameOfRevenge.Common.Models.Hero;
//using Newtonsoft.Json;

namespace GameOfRevenge.Common.Models
{
    [DataContract]
    public class UserHeroDetails
    {
        [DataMember]
        public long Id { get; set; }//id
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public int BattleCount { get; set; }//value

//        [JsonIgnore]
        //SERVER SIDE FUNCTION
        public int Level => 1 + (int)Math.Floor(WarPoints / 10f);// 10 warpoints = 1 level
//        [JsonIgnore]
        //SERVER SIDE FUNCTION
        public int WarPoints => (int)Math.Floor(BattleCount / 5f);// 5 battles = 1 war point

//        public bool Unlocked { get; set; }
        [DataMember]
        public bool IsMarching { get; set; }
//        [JsonIgnore]
//        public bool IsAvaliable { get => !IsMarching && Unlocked; }
    }
}

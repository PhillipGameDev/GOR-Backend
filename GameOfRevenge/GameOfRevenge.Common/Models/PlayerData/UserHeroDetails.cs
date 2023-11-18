using System;
using System.Runtime.Serialization;
using GameOfRevenge.Common.Models.Hero;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GameOfRevenge.Common.Models
{
    [DataContract, Serializable]
    public class UserHeroDetails
    {
        public const int UNLOCK_POINTS = 10;
        public const int MAX_LEVEL = 5;

        //        [DataMember]
        //        public long HeroId { get; set; }//id

        [DataMember, JsonConverter(typeof(StringEnumConverter))]
        public HeroType HeroType { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int Points { get; set; }//1
        [DataMember(EmitDefaultValue = false)]
        public int Power { get; set; }//2
        [DataMember(EmitDefaultValue = false)]
        public int AttackCount { get; set; }//3
        [DataMember(EmitDefaultValue = false)]
        public int AttackFail { get; set; }//4
        [DataMember(EmitDefaultValue = false)]
        public int DefenseCount { get; set; }//5
        [DataMember(EmitDefaultValue = false)]
        public int DefenseFail { get; set; }//6

        [DataMember(EmitDefaultValue = false)]
        public bool IsMarching { get; set; }

        public int Level => GetLevel(Points);

        public static int GetLevel(int points)
        {
            var lvl = 0;
            if (points > 0) lvl = (int)Math.Floor(points / (float)UNLOCK_POINTS);
            return lvl;
        }
    }
}

using System.Runtime.Serialization;
using GameOfRevenge.Common.Models.Hero;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GameOfRevenge.Common.Models.Kingdom.AttackAlertReport
{
    [DataContract]
    public class TroopData
    {
        [DataMember, JsonConverter(typeof(StringEnumConverter))]
        public TroopType Type { get; set; }
        [DataMember]
        public int Level { get; set; }

        [DataMember]
        public int Count { get; set; }
    }

    [DataContract]
    public class HeroData
    {
        [DataMember, JsonConverter(typeof(StringEnumConverter))]
        public HeroType Type { get; set; }
        [DataMember]
        public int Level { get; set; }
    }
}
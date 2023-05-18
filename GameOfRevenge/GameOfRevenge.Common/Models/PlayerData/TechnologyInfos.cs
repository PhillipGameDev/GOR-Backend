using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GameOfRevenge.Common.Models
{
    [DataContract, Serializable]
    public class TechnologyInfos : TimerBase
    {
        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public TechnologyType TechnologyType { get; set; }
        [DataMember]
        public int Level { get; set; }
        [DataMember]
        public string Code { get => TechnologyType.ToString(); }

        public int CurrentLevel => (TimeLeft == 0)? Level : (Level - 1);
    }

/*    [DataContract]
    public class SubTechnologyInfos : TimerBase
    {
        [DataMember]
        public SubTechnologyType SubTechnologyType { get; set; }
        [DataMember]
        public int Level { get; set; }
        [DataMember]
        public string Code { get => SubTechnologyType.ToString(); }
    }*/
}

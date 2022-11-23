using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Runtime.Serialization;

namespace GameOfRevenge.Common.Models.Quest.Template
{
    [DataContract]
    public class QuestResourceData
    {
        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public ResourceType ResourceType { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int Count { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int Iteration { get; set; }
    }
}

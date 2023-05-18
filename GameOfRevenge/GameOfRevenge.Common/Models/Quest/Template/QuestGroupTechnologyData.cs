using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GameOfRevenge.Common.Models.Quest.Template
{
    [DataContract]
    public class QuestGroupTechnologyData
    {
        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public GroupTechnologyType GroupTechnologyType { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int Count { get; set; }
    }
}

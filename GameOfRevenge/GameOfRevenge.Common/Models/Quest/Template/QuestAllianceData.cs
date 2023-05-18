using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GameOfRevenge.Common.Models.Quest.Template
{
    [DataContract]
    public class QuestAllianceData
    {
        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public AllianceTaskType AllianceTaskType { get; set; }
    }

    public enum AllianceTaskType
    {
        JoinOrCreate = 1
    }
}

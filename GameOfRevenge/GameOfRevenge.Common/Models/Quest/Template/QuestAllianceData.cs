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
        [DataMember]
        public int? Count { get; set; }
    }

    public enum AllianceTaskType
    {
        JoinOrCreate = 1,
        Reinforce = 2,
        SendGift = 3,
        Help = 4
    }
}

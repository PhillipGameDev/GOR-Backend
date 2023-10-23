using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GameOfRevenge.Common.Models.Quest.Template
{
    [DataContract]
    public class QuestCustomData
    {
        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public CustomTaskType CustomTaskType { get; set; }
    }

    public enum CustomTaskType
    {
        SendGlobalChat = 1,
        AttackPlayer = 2,
        ItemBoxExploring = 3
    }
}

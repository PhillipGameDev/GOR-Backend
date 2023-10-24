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
        [DataMember]
        public int? Level { get; set; }
        [DataMember]
        public int? Count { get; set; }
        [DataMember]
        public string Value { get; set; }
    }

    public enum CustomTaskType
    {
        SendGlobalChat = 1,
        AttackPlayer = 2,
        ItemBoxExploring = 3,
        AttackMonster = 4,
        GetHeroPoints = 5,
        BuildOrUpgrade = 6,
        ResearchTechnology = 7,
        AttackOasis = 8,
        TradeMarket = 9
    }
}

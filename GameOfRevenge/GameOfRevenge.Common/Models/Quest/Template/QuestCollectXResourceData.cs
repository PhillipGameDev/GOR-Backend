using Newtonsoft.Json;
using System;

namespace GameOfRevenge.Common.Models.Quest.Template
{
    public class QuestCollectXResourceData : IBaseQuestTemplateData, IEquatable<QuestCollectXResourceData>
    {
        public string ResourceType { get; set; }
        public int Count { get; set; }

        public QuestType QuestType { get => QuestType.ResourceCollection; }
        public string Name { get => $"Collect {Count} {ResourceType}"; }

        public bool Equals(QuestCollectXResourceData other)
        {
            return Count >= other.Count;
        }

        public void SetData(string template)
        {
            var data = JsonConvert.DeserializeObject<QuestCollectXResourceData>(template);

            ResourceType = data.ResourceType;
            Count = data.Count;
        }
    }
}

using Newtonsoft.Json;
using System;

namespace GameOfRevenge.Common.Models.Quest.Template
{
    public class QuestHaveXBuildingData : IBaseQuestTemplateData, IEquatable<QuestHaveXBuildingData>
    {
        public string StructureType { get; set; }
        public int Count { get; set; }
        public QuestType QuestType { get => QuestType.XBuildingCount; }
        public string Name { get => $"Have {Count} {StructureType}"; }

        public bool Equals(QuestHaveXBuildingData other)
        {
            return Count >= other.Count;
        }

        public void SetData(string template)
        {
            var data = JsonConvert.DeserializeObject<QuestHaveXBuildingData>(template);

            StructureType = data.StructureType;
            Count = data.Count;
        }
    }
}

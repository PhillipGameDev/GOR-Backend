using Newtonsoft.Json;
using System;

namespace GameOfRevenge.Common.Models.Quest.Template
{
    public class QuestBuildingData : IBaseQuestTemplateData, IEquatable<QuestBuildingData>
    {
        public string StructureType { get; set; }
        public int Level { get; set; }
        public QuestType QuestType { get => QuestType.BuildingUpgrade; }
        public string Name { get => Level == 1 ? $"Create building {StructureType}" : $"Upgrade {StructureType} to level {Level}"; }

        public bool Equals(QuestBuildingData other)
        {
            return Level >= other.Level;
        }

        public void SetData(string template)
        {
            var data = JsonConvert.DeserializeObject<QuestBuildingData>(template);

            StructureType = data.StructureType;
            Level = data.Level;
        }
    }
}

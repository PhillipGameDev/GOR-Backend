using Newtonsoft.Json;
using System;

namespace GameOfRevenge.Common.Models.Quest.Template
{
    public class QuestTrainXTroopData : QuestHaveXTroopData, IBaseQuestTemplateData, IEquatable<QuestTrainXTroopData>
    {
        public int Level { get; set; }

        public override QuestType QuestType { get => QuestType.XTroopCount; }
        public override string Name { get => $"Train {Count} {TroopType} {Level}"; }

        public bool Equals(QuestTrainXTroopData other)
        {
            return Count >= other.Count && Level >= other.Level;
        }

        public override void SetData(string template)
        {
            var data = JsonConvert.DeserializeObject<QuestTrainXTroopData>(template);

            TroopType = data.TroopType;
            Count = data.Count;
            Level = data.Level;
        }
    }
}

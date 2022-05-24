using Newtonsoft.Json;
using System;

namespace GameOfRevenge.Common.Models.Quest.Template
{
    public class QuestHaveXTroopData : IBaseQuestTemplateData, IEquatable<QuestHaveXTroopData>
    {
        public string TroopType { get; set; }
        public int Count { get; set; }

        public virtual QuestType QuestType { get => QuestType.TrainTroops; }
        public virtual string Name { get => $"Have {Count} {TroopType}"; }

        public bool Equals(QuestHaveXTroopData other)
        {
            return Count >= other.Count;
        }

        public virtual void SetData(string template)
        {
            var data = JsonConvert.DeserializeObject<QuestHaveXTroopData>(template);

            TroopType = data.TroopType;
            Count = data.Count;
        }
    }
}

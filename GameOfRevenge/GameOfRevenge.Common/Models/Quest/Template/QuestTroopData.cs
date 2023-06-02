using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Runtime.Serialization;

namespace GameOfRevenge.Common.Models.Quest.Template
{
    [DataContract]
    public class QuestTroopData
    {
        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public TroopType TroopType { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int Level { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int Count { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int Iteration { get; set; }


        public QuestTroopData()
        {
        }

        public QuestTroopData(QuestTroopData data)
        {
            TroopType = data.TroopType;
            Level = data.Level;
            Count = data.Count;
            Iteration = data.Iteration;
        }
    }
}

using GameOfRevenge.Common.Models.Structure;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Runtime.Serialization;

namespace GameOfRevenge.Common.Models.Quest.Template
{
    [DataContract]
    public class QuestAccountData
    {
        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public AccountTaskType AccountTaskType { get; set; }
    }

    public enum AccountTaskType
    {
        SignIn = 1,
        ChangeName = 2
    }
}

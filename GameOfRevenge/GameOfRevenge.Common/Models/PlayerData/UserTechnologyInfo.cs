using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GameOfRevenge.Common.Models
{
    [DataContract, Serializable]
    public class UserTechnologyInfo
    {
        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public AcademyTechnologyType TechnologyType { get; set; }
        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public AcademyCategoryType CategoryType { get; set; }
        [DataMember]
        public int Effect { get; set; }

        public bool IsResourceBoost(ResourceType type)
        {
            if (CategoryType != AcademyCategoryType.Resource) return false;

            if (TechnologyType == AcademyTechnologyType.ProductionBoost) return true;

            if (type == ResourceType.Food && TechnologyType == AcademyTechnologyType.FoodProduction) return true;
            if (type == ResourceType.Wood && TechnologyType == AcademyTechnologyType.WoodProduction) return true;
            if (type == ResourceType.Ore && TechnologyType == AcademyTechnologyType.OreProduction) return true;

            return false;
        }
    }
}

//using Newtonsoft.Json;
//using Newtonsoft.Json.Converters;

namespace GameOfRevenge.Common.Models.Boost
{
//    [JsonConverter(typeof(StringEnumConverter))]
    public enum BoostType
    {
        Unknown = 0,
        Shield = 1,//Shield_resourceProduction
        Blessing = 2,//Blessing_constructionSpeed
        LifeSaver = 3,//LifeSaver_traningSpeed
        ProductionBoost = 4,//ProductionBoost_recoverySpeed
        SpeedGathering = 5,//SpeedGathering_armyAttack
        Fog = 6,//Fog_armyDefence
        TechBoost = 7
    }
}

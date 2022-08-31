using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GameOfRevenge.Common
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DataType
    {
        Unknown = 0,
        Resource = 1,//TODO: when this enum is used on a requirement table the value is a mix between resources and level requirement.
                     //maybe we should use another value in the database, like Custom(7) or a "Requirement" named property
                     //DataType+ValueId+Value
                     //Requirement+Level+value / Requirement+Resource+value
        Structure = 2,
        Troop = 3,
        Marching = 4,
        Inventory = 5,

        Timestamp = 6,
        Custom = 7,

        Technology = 8,

        ActiveBoost = 10,
        Hero = 15,
        IncomingAttack = 17,
        Gift = 1017,
        Product = 1018,
        Activity = 1019,
        SubTechnology = 1020
    }
}

﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GameOfRevenge.Common
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DataType
    {
        Unknown = 0,
        Resource = 1,
        Structure = 2,
        Troop = 3,
        Marching = 4,
        Inventory = 5,

        Timestamp = 6,
        Custom = 7,

        Technology = 8,
        CityBoost = 9,

        ActiveBoost = 10,
        Hero = 15,
        IncomingAttack = 17,
        BattleHistory = 18,

        Gift = 1017,
        Product = 1018,
        Activity = 1019,
        SubTechnology = 1020,
        Reward = 1021,

        ShopItem = 1022,
        PackageItem = 1023,

        Item = 1101,
        RawResource = 1102
    }
}

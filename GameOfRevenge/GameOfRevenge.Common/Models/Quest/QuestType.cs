﻿namespace GameOfRevenge.Common.Models.Quest
{
    public enum QuestType : int
    {
        Other = 0,

        BuildingUpgrade = 1,
        XBuildingCount = 2,

        ResourceCollection = 3,

        TrainTroops = 4,
        XTroopCount = 5,

        TrainHero = 6,
        XHeroCount = 7,

        Custom = 8,

        Account = 9,
        Alliance = 10,
        ResearchTechnology = 11
//      MarketQuest
//        Custom = 11
    }
}

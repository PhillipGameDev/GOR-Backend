namespace GameOfRevenge.Common
{
    public enum AcademyTechnologyType : int
    {
        Unknown = 0,
        // Resource
        FoodProduction = 1,
        WoodProduction = 2,
        OreProduction = 3,
        ProductionBoost = 4,
        StorageBoost = 5,
        // War
        TroopTraining = 10,
        TroopAttack = 11,
        TroopDefense = 12,
        TroopRecovery = 13,
        TroopMarching = 14,
        InfantryAttack = 15,
        InfantryDefense = 16,
        ArcherAttack = 17,
        ArcherDefense = 18,
        KnightAttack = 19,
        KnightDefense = 20,
        SlingshotAttack = 21,
        SlingshotDefense = 22,
        InfantryTraining = 23,
        ArcherTraining = 24,
        KnightTraining = 25,
        SlingshotTraining = 26,
        // Development
        ConstructionBoost = 30,
        ResearchBoost = 31,
        BlacksmithBoost = 32,
        KingItemEffectDuration = 33
    }
}

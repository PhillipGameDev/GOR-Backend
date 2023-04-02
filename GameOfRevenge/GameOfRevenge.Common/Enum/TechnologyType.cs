using GameOfRevenge.Common.Models.Boost;

namespace GameOfRevenge.Common
{
    public enum GroupTechnologyType
    {
        Unknown = 0,
        KingdomTechnologies = 1,
        AttackTechnologies = 2,
        DefenseTechnologies = 3
/*        ResourceProduction = 1,
        ConstructionSpeed = 2,
        TrainingSpeed = 3,
        RecoverySpeed = 4,
        ArmyAttack = 5,
        ArmyDefense = 6*/
    }

    public enum TechnologyType
    {
        Unknown = 0,
        ConstructionTechnology = NewBoostType.Construction,
        ResearchSpeedTechnology = NewBoostType.ResearchSpeed,
        TroopLoadTechnology = NewBoostType.TroopLoad,
        StorageTechnology = NewBoostType.ResourceStorage,
        InfirmaryCapacityTechnology = NewBoostType.InfirmaryCapacity,
        KingStaminaRecoveryTechnology = NewBoostType.StaminaRecovery,
        TrainSpeedTechnology = NewBoostType.TrainingSpeed,
        HealSpeedTechnology = NewBoostType.HealingSpeed,
        UpkeepTechnology = NewBoostType.UpkeepReduction,

        BarracksAttackTechnology = NewBoostType.InfantryAttack,
        StableAttackTechnology = NewBoostType.CavalryAttack,
        WorkshopAttackTechnology = NewBoostType.SiegeAttack,
        ShootingRangeAttackTechnology = NewBoostType.BowmenAttack,

        BarracksDefenseTechnology = NewBoostType.InfantryDefense,
        StableDefenseTechnology = NewBoostType.CavalryDefense,
        WorkshopDefenseTechnology = NewBoostType.SiegeDefense,
        ShootingRangeDefenseTechnology = NewBoostType.BowmenDefense


/*        FoodProduction1 = 1,
        WoodProduction1 = 2,
        IronProduction1 = 3,
        WallDefense1 = 4,
        DefenderAttack1 = 5,
        DefenderDefense1 = 6,
        DefenderHealth1 = 7,
        MarchSpeed1 = 8,
        MarchCapacity1 = 9,
        InfantryHealth1 = 10,
        InfantryAttack1 = 11,
        TrainSpeed1 = 12,
        FastHeal1 = 13,
        HospitalCapacity1 = 14*/
    }
}

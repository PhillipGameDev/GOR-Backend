//using Newtonsoft.Json;
//using Newtonsoft.Json.Converters;

namespace GameOfRevenge.Common.Models.Boost
{
    //    [JsonConverter(typeof(StringEnumConverter))]
    public enum CityBoostType : byte
    {
        Unknown = 0,
        Shield = NewBoostType.Shield,//Shield_resourceProduction
        Blessing = NewBoostType.Blessing,//Blessing_constructionSpeed
        LifeSaver = NewBoostType.LifeSaver,//LifeSaver_traningSpeed
        ProductionBoost = NewBoostType.ProductionBoost,//ProductionBoost_recoverySpeed
                                                       //        SpeedGathering = 5,//SpeedGathering_armyAttack
        TechBoost = NewBoostType.TechBoost,
        Fog = NewBoostType.Fog//Fog_armyDefence
                              //        ConstructionBoost = NewBoostType.Construction
    }

    public enum VIPBoostType : byte
    {
        Unknown = 0,
        VIP = NewBoostType.VIP
        //SVIP
    }

    public enum NewBoostType : byte
    {
        Unknown = 0,
        Shield,//=NewBoostTech.ProtectionShield
        Blessing,
        LifeSaver,//=NewBoostTech.TroopHealingSpeedMultiplier
        ProductionBoost,//=NewBoostTech.ResourceProductionMultiplier
        TechBoost,//=NewBoostTech.AcademyResearchSpeedMultiplier
        Fog,//=NewBoostTech.ProtectionFog

        VIP = 15,
        KING = 16,
        //SVIP

        Construction = 20,
        ResearchSpeed,
        TroopLoad,
        ResourceStorage,
        InfirmaryCapacity,
        StaminaRecovery,
        TrainingSpeed,
        HealingSpeed,
        UpkeepReduction,

        InfantryAttack = 40,
        SiegeAttack,
        CavalryAttack,
        BowmenAttack,

        InfantryDefense = 60,
        SiegeDefense,
        CavalryDefense,
        BowmenDefense
    }

    public enum VIPBoostTech : byte
    {
        ResourceProductionMultiplier = NewBoostTech.ResourceProductionMultiplier,
        BuildingTimeBonus = NewBoostTech.BuildingTimeBonus,
        TroopRecoverySpeedMultiplier = NewBoostTech.TroopRecoverySpeedMultiplier,
        TroopAttackMultiplier = NewBoostTech.TroopAttackMultiplier,
        TroopDefenseMultiplier = NewBoostTech.TroopDefenseMultiplier,
        InfirmaryCapacityMultiplier = NewBoostTech.InfirmaryCapacityMultiplier,
        TroopTrainingSpeedMultiplier = NewBoostTech.TroopTrainingSpeedMultiplier,
        ResourceStorageMultiplier = NewBoostTech.ResourceStorageMultiplier
    }

    public enum KINGBoostTech : byte
    {
        TroopAttackMultiplier = NewBoostTech.TroopAttackMultiplier,
        TroopDefenseMultiplier = NewBoostTech.TroopDefenseMultiplier,
        InfantryAttackMultiplier = NewBoostTech.InfantryAttackMultiplier,
        InfantryDefenseMultiplier = NewBoostTech.InfantryDefenseMultiplier,
        SiegeAttackMultiplier = NewBoostTech.SiegeAttackMultiplier,
        SiegeDefenseMultiplier = NewBoostTech.SiegeDefenseMultiplier,
        CavalryAttackMultiplier = NewBoostTech.CavalryAttackMultiplier,
        CavalryDefenseMultiplier = NewBoostTech.CavalryDefenseMultiplier,
        BowmenAttackMultiplier = NewBoostTech.BowmenAttackMultiplier,
        BowmenDefenseMultiplier = NewBoostTech.BowmenDefenseMultiplier,
    }

    public enum NewBoostTech : byte
    {
        Unknown = 0,
        ProtectionFog,
        ProtectionShield,
        CityWallDefensePower,
        CityTroopAttackMultiplier,
        CityTroopDefenseMultiplier,
        KingStaminaRecoverySpeedMultiplier,
        BuildingSpeedMultiplier,
        ResourceProductionMultiplier,
        ResourceStorageMultiplier,
        ResearchSpeedMultiplier,
        InfirmaryCapacityMultiplier,
        InfirmaryCapacityAmount,
        TroopLoadMultiplier,
        TroopTrainingSpeedMultiplier,
        TroopRecoverySpeedMultiplier,
        TroopMarchingSpeedMultiplier,
        TroopUpkeepReductionMultiplier,
        TroopTrainingTimeBonus,
        TroopRecoveryTimeBonus,
        BuildingTimeBonus,
        ResearchTimeBonus,
        KingPowerMultiplier,
        InfantryPowerMultiplier,
        SiegePowerMultiplier,
        CavalryPowerMultiplier,
        BowmenPowerMultiplier,
        TroopMarchingReductionMultiplier,

        AttackPower = 40,
        TroopAttackMultiplier,
        InfantryAttackMultiplier,
        InfantryAttackPower,
        SiegeAttackMultiplier,
        SiegeAttackPower,
        CavalryAttackMultiplier,
        CavalryAttackPower,
        BowmenAttackMultiplier,
        BowmenAttackPower,


        DefensePower = 70,
        TroopDefenseMultiplier,
        InfantryDefenseMultiplier,
        InfantryDefensePower,
        SiegeDefenseMultiplier,
        SiegeDefensePower,
        CavalryDefenseMultiplier,
        CavalryDefensePower,
        BowmenDefenseMultiplier,
        BowmenDefensePower

        /*        public const int KING_STAMINA_RECOVERY_SPEED_MULTIPLIER = 1;
                public const int BUILDING_SPEED_MULTIPLIER = 2;
                public const int RESOURCE_STORAGE_MULTIPLIER = 4;
                public const int ACADEMY_RESEARCH_SPEED_MULTIPLIER = 8;
                public const int INFIRMARY_CAPACITY_MULTIPLIER = 16;
        32 TROOP_LOAD_MULTIPLIER float
        64 TROOP_TRAINING_SPEED_MULTIPLIER float
        128 TROOP_HEALING_SPEED_MULTIPLIER float
        256 TROOP_UPKEEP_REDUCTION_MULTIPLIER float
                }
                ----------KT

        -----------AT
        1 ATTACK_POWER int
        2 INFANTRY_ATTACK_MULTIPLIER float
        4 INFANTRY_ATTACK_POWER int
        8 *SIEGE_ATTACK_MULTIPLIER float
        16 *SIEGE_ATTACK_POWER int
        32 CAVALRY_ATTACK_MULTIPLIER float
        64 CAVALRY_ATTACK_POWER int
        128 BOWMEN_ATTACK_MULTIPLIER float
        256 BOWMEN_ATTACK_POWER int

        ------------DT
        1 DEFENSE_POWER int
        2 INFANTRY_DEFENSE_MULTIPLIER float
        4 INFANTRY_DEFENSE_POWER int
        8 *SIEGE_DEFENSE_MULTIPLIER float
        16 *SIEGE_DEFENSE_POWER int
        32 CAVALRY_DEFENSE_MULTIPLIER float
        64 CAVALRY_DEFENSE_POWER int
        128 BOWMEN_DEFENSE_MULTIPLIER float
        256 BOWMEN_DEFENSE_POWER int*/
    }
}

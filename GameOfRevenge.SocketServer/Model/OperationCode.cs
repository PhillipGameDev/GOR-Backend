namespace GameOfRevenge.Model
{
    public enum OperationCode : byte
    {
        PlayerConnectToServer = 1,
        Ping = 2,
        UserTeleport = 3,
        JoinKingdomRoom = 4,
        LeaveKingdomRoom = 5,
        PlayerCameraMove = 6,
        CreateStructure = 7,
        UpgradeStructure = 8,
        PlayerBuildingStatus = 9,
        RecruitTroopRequest = 10,
        RecruitTroopStatus = 11,
        TroopTrainerTimeBoost = 12,
        CollectResourceRequest = 13,
        BoostResourceTime = 14,
        AttackRequest = 15,
        WoundedHealReqeust = 16,
        WoundedHealTimerRequest = 17,
        UpgradeTechnology = 18,
        RepairGate = 19,
        GateHp = 20,

        GlobalChat = 21,
        AllianceChat = 29,
        DeleteChat = 32,

        CheckUnderAttack = 22,

        InstantBuild = 23,
        SpeedUpBuild = 24,
        SpeedUpBuildCost = 25,
        GetInstantBuildCost = 26,

        InstantRecruit = 27,
        HelpStructure = 28,
        UpdatePlayerData = 30,
        HelpStructureRequest = 31,
        SendReinforcementsRequest = 33
    }
}

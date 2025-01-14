﻿namespace GameOfRevenge.Model
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
        SendReinforcementsRequest = 33,
        UpdateMarchingArmy = 36,
        RecallMarchingArmy = 37,

        SendFriendRequest = 34,
        RespondToFriendRequest = 35,
        SetPlayerContact = 55,

        ClaimRewardsRequest = 38,

        ItemBoxExploring = 39,
        BuildOrUpgrade = 40,

        InstantResearch = 41,
        SpeedUpResearch = 42,

        // Alliance
        JoinAllianceRequest = 43,
        AcceptJoinRequest = 44,
        UpdateClanCapacity = 45,
        UpdateClanRole = 46,
        LeaveAllianceRequest = 47,
        JoinUnionRequest = 48,
        AcceptUnionRequest = 49,
        LeaveUnionRequest = 50,
        CreateClan = 51,
        DeleteClan = 52,

        InstantWoundedHealRequest = 53,
        GetKingdomInformation = 54,

        // Mail
        SendMail = 60
    }
}

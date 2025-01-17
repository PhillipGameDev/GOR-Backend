﻿namespace GameOfRevenge.Model
{
    public enum EventCode : byte
    {
        UserProfile = 1,
        IaEnter = 2,
        IaExit = 3,
        UpdateResource = 4,
        CompleteTimer = 5,
        AttackEvent = 6,
        MarchingResult = 7,
        UpdateQuest = 8,
        UnderAttack = 9,
        ReinforcementsEvent = 10,
        UpdateMarchingArmyEvent = 11,
        EntityEnter = 12,
        EntityExit = 13,
        HelpStructure = 14,
        FortressStatus = 15,
        BattleResult = 16,
        ReceiveMail = 17
    }
}

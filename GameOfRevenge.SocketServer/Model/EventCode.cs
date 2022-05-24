namespace GameOfRevenge.Model
{
    public enum EventCode : byte
    {
        UserProfile = 1,
        IaEnter = 2,
        IaExit = 3,
        UpdateResource = 4,
        CompleteTimer = 5,
        AttackResponse = 6,
        AttackResult = 7,
        UpdateQuest = 8,
        UnderAttack = 9
    }
}

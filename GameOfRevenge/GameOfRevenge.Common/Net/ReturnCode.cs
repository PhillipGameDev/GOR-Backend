namespace GameOfRevenge.Common.Net
{
    public enum ReturnCode : byte
    {
        OK = 1,
        InvalidOperationParameter = 2,
        InvalidOperation = 3,
        WorldNotFound = 4,
        Failed = 5,
        AttackNotSelected = 6,
        CharacterNotFound = 7,
        UserNotFoundInInterestArea = 8
    }
}

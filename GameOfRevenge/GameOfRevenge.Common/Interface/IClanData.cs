namespace GameOfRevenge.Common.Interface
{
    public interface IClanData
    {
        int Id { get; }
        string Name { get; }
        string Tag { get; }
        string Description { get; }
        bool IsPublic { get; }
        short BadgeGK { get; }
        int Flag { get; }
        int Capacity { get; }
        int MemberCount { get; }
    }
}

namespace GameOfRevenge.Common.Interface.Model.Table
{
    public interface IReadOnlyBaseRefEnumTable<T>// where T : Enum
    {
        int Id { get; }
        T Code { get; }
        string Name { get; }
    }
}

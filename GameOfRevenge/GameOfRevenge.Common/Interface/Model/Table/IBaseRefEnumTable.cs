namespace GameOfRevenge.Common.Interface.Model.Table
{
    public interface IBaseRefEnumTable<T> : IReadOnlyBaseRefEnumTable<T>// where T : Enum
    {
        new int Id { get; set; }
        new T Code { get; set; }
        new string Name { get; set; }
    }
}

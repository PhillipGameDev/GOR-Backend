namespace GameOfRevenge.Common.Interface.Model.Table
{
    public interface IReadOnlyBaseRefEnumLevelDataTable
    {
        int DataId { get; }
        int Id { get; }
        int Level { get; }
    }
}

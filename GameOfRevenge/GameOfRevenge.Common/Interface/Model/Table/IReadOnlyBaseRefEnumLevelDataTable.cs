namespace GameOfRevenge.Common.Interface.Model.Table
{
    public interface IReadOnlyBaseRefEnumLevelDataTable
    {
        int DataId { get; }
        int InfoId { get; }
        int Level { get; }
    }
}

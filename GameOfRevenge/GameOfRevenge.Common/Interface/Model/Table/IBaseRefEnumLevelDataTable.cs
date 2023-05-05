namespace GameOfRevenge.Common.Interface.Model.Table
{
    public interface IBaseRefEnumLevelDataTable : IReadOnlyBaseRefEnumLevelDataTable
    {
        new int DataId { get; set; }
        new int InfoId { get; set; }
        new int Level { get; set; }
    }
}

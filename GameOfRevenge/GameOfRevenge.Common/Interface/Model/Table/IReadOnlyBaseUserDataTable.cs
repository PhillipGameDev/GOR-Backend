namespace GameOfRevenge.Common.Interface.Model.Table
{
    public interface IReadOnlyBaseUserDataTable<T, D> : IReadOnlyBaseDataTypeTable<T, D>
    {
        long Id { get; }
    }
}

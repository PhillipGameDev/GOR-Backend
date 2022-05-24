namespace GameOfRevenge.Common.Interface.Model.Table
{
    public interface IBaseUserDataTable<T, D> : IBaseDataTypeTable<T, D>, IReadOnlyBaseDataTypeTable<T, D>, IReadOnlyBaseUserDataTable<T, D>
    {
        new long Id { get; set; }
    }
}

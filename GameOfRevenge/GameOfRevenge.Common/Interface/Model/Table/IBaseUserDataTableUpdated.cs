namespace GameOfRevenge.Common.Interface.Model.Table
{
    public interface IBaseUserDataTableUpdated<T, D> : IBaseUserDataTable<T, D>, IBaseDataTypeTable<T, D>, IReadOnlyBaseDataTypeTable<T, D>, IReadOnlyBaseUserDataTable<T, D>
    {
        new string OldValue { get; set; }
    }
}

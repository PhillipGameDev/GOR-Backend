namespace GameOfRevenge.Common.Interface.Model.Table
{
    public interface IReadOnlyBaseDataTypeTable<T, D>
    {
        DataType DataType { get; }
        T ValueId { get; }
        D Value { get; }
    }
}

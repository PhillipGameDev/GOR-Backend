namespace GameOfRevenge.Common.Interface.Model.Table
{
    public interface IBaseDataTypeTable<T, D> : IReadOnlyBaseDataTypeTable<T, D>
    {
        new DataType DataType { get; set; }
        new T ValueId { get; set; }
        new D Value { get; set; }
    }
}

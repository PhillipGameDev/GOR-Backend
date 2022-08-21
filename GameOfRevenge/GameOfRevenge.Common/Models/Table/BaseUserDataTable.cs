using GameOfRevenge.Common.Interface.Model.Table;

namespace GameOfRevenge.Common.Models.Table
{
    public abstract class BaseUserDataTable<T, D> : IBaseUserDataTable<T, D>, IReadOnlyBaseUserDataTable<T, D>, IBaseDataTypeTable<T, D>, IReadOnlyBaseDataTypeTable<T, D>
    {
        public long Id { get; set; }
        public DataType DataType { get; set; }
        public T ValueId { get; set; }
        public D Value { get; set; }
    }
}

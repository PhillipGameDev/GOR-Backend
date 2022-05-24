using GameOfRevenge.Common.Interface.Model.Table;
using System;
using System.Data;

namespace GameOfRevenge.Common.Models.Table
{
    public abstract class BaseDataTypeTable<T, D> : BaseTable, IBaseTable, IBaseDataTypeTable<T, D>, IReadOnlyBaseDataTypeTable<T, D>
    {
        public DataType DataType { get; set; }
        public T ValueId { get; set; }
        public D Value { get; set; }
    }
}

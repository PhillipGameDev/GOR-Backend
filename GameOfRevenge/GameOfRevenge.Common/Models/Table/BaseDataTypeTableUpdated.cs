using GameOfRevenge.Common.Interface.Model.Table;
using System;
using System.Data;

namespace GameOfRevenge.Common.Models.Table
{
    public abstract class BaseDataTypeTableUpdated<T, D> : BaseDataTypeTable<T, D>, IBaseTable, IBaseDataTypeTable<T, D>, IReadOnlyBaseDataTypeTable<T, D>
    {
        public string OldValue { get; set; }
    }
}

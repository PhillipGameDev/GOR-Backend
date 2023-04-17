using GameOfRevenge.Common.Interface.Model.Table;
using System;
using System.Data;
using System.Runtime.Serialization;

namespace GameOfRevenge.Common.Models.Table
{
    [DataContract]
    public abstract class BaseDataTypeTable<T, D> : BaseTable, IBaseTable, IBaseDataTypeTable<T, D>, IReadOnlyBaseDataTypeTable<T, D>
    {
        [DataMember]
        public DataType DataType { get; set; }
        [DataMember]
        public T ValueId { get; set; }
        [DataMember]
        public D Value { get; set; }
    }
}

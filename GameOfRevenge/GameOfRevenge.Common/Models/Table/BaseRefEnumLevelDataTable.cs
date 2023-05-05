using System.Runtime.Serialization;
using GameOfRevenge.Common.Interface.Model.Table;

namespace GameOfRevenge.Common.Models.Table
{
//    [DataContract]
    public abstract class BaseRefEnumLevelDataTable : BaseTable, IBaseTable, IBaseRefEnumLevelDataTable, IReadOnlyBaseRefEnumLevelDataTable
    {
//        [DataMember]
        public int DataId { get; set; }
        public int InfoId { get; set; }
//        [DataMember]
        public int Level { get; set; }
    }
}

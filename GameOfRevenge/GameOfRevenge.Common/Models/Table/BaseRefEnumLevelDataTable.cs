using GameOfRevenge.Common.Interface.Model.Table;

namespace GameOfRevenge.Common.Models.Table
{
    public abstract class BaseRefEnumLevelDataTable : BaseTable, IBaseTable, IBaseRefEnumLevelDataTable, IReadOnlyBaseRefEnumLevelDataTable
    {
        public int DataId { get; set; }
        public int Id { get; set; }
        public int Level { get; set; }
    }
}

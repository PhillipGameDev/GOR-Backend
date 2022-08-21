using GameOfRevenge.Common.Interface.Model.Table;
using Newtonsoft.Json;

namespace GameOfRevenge.Common.Models.Table
{
    public abstract class BaseUserDataTableUpdated<T, D> : BaseUserDataTable<T, D>
    {
        public string OldValue { get; set; }
    }
}

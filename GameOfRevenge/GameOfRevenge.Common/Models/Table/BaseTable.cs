using System.Data;

namespace GameOfRevenge.Common.Models.Table
{
    public interface IBaseTable
    {
        void LoadFromDataReader(IDataReader reader);
    }

    public abstract class BaseTable : IBaseTable
    {
        protected BaseTable() { }
        public virtual void LoadFromDataReader(IDataReader reader) { }
    }
}

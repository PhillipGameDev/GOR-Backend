using GameOfRevenge.Business.DataLink;

namespace GameOfRevenge.Business.Manager.Base
{
    public class BaseManager
    {
        private readonly BaseDbManager baseDbManager = new BaseDbManager();
        internal string ConnectionString { get => baseDbManager.ConnectionString; set => baseDbManager.ConnectionString = value; }
        internal IBaseDbManager Db { get => baseDbManager; }
    }
}

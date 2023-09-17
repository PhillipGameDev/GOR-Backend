using System.Collections.Concurrent;

namespace GameOfRevenge.Interface
{
    public interface IWorldHandler 
    {
        IWorld DefaultWorld { get; }
        ConcurrentDictionary<int, IWorld> Worlds { get; }

        void SetupWorld(string worldCode);
    }
}

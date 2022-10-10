using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common.Models.Kingdom;


namespace GameOfRevenge.Interface
{
    public interface IWorldHandler 
    {
        IWorld DefaultWorld { get; }
        ConcurrentDictionary<int, IWorld> Worlds { get; }

        void SetupPvpWorld(int worldId, List<WorldDataTable> worldData);
    }
}

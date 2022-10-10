using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Interface.Model;

namespace GameOfRevenge.ResourcesHandler
{
    public interface IPlayerResources
    {
        IReadOnlyResourceTable ResourceInfo { get; }
        long Value { get; }

        void IncrementResourceValue(int val);
        bool HasAvailableRequirement(IReadOnlyDataRequirement requirment);
    }
}

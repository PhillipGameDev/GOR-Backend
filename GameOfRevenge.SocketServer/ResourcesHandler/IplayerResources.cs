using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Interface.Model;

namespace GameOfRevenge.ResourcesHandler
{
    public interface IPlayerResources
    {
        IReadOnlyResourceTable ResourceInfo { get; }
        float Value { get; }

        void UpdateResourceValue(float val);
        bool HasAvailableRequirment(IReadOnlyDataRequirement requirment);
    }
}

using GameOfRevenge.Model;

namespace GameOfRevenge.Interface
{
    public interface IPlayerAttackHandler
    {
        bool AttackRequest(AttackRequest request);
    }
}

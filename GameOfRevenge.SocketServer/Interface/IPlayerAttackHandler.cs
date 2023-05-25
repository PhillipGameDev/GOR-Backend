using System.Threading.Tasks;
using GameOfRevenge.Model;

namespace GameOfRevenge.Interface
{
    public interface IPlayerAttackHandler
    {
        Task<bool> AttackRequestAsync(SendArmyRequest request);
    }
}

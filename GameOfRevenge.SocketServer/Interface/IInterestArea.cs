using GameOfRevenge.GameHandlers;

namespace GameOfRevenge.Interface
{
    public interface IInterestArea
    {
        void UpdateInterestArea(Region newRegion);
        void JoinKingdomView();
        void LeaveKingdomView();
        void CameraMove(Region r);
        void Dispose();
    }
}

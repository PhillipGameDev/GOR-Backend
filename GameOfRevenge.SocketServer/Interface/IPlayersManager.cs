using GameOfRevenge.GameHandlers;

namespace GameOfRevenge.Interface
{
    public interface IPlayersManager
    {
        void AddPlayer(int playerId, MmoActor player);
        void RemovePlayer(int playerId);
        MmoActor GetPlayer(int playerId);
        void BroadCastToWorld(byte evCode, object data);
        //TODO: Implement Broadcast to clan
        void ClearAll();
    }
}

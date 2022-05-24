using GameOfRevenge.GameHandlers;

namespace GameOfRevenge.Interface
{
    public interface IPlayersManager
    {
        void AddPlayer(string playerId, MmoActor player);
        void RemovePlayer(string playerId);
        MmoActor GetPlayer(string playerId);
        void BroadCastToWorld(byte evCode, object data);
        void ClearAll();
    }
}

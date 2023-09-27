using GameOfRevenge.GameHandlers;

namespace GameOfRevenge.Interface
{
    public interface IPlayersManager
    {
        void AddPlayer(int playerId, PlayerInstance player);
        void RemovePlayer(int playerId);
        PlayerInstance GetPlayer(int playerId);
        void BroadcastToWorld(byte evCode, object data);
        //TODO: Implement Broadcast to clan
        void ClearAll();
    }
}

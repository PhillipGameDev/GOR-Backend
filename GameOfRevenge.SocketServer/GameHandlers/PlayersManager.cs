
using GameOfRevenge.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfRevenge.GameHandlers
{

    public class PlayersManager : IPlayersManager, IDisposable
    {
        // Locking the sync root guarantees thread safe access.
        protected readonly object SyncRoot = new object(); // that is for world user access
        private readonly Dictionary<int, MmoActor> players;
        public PlayersManager()
        {
            players = new Dictionary<int, MmoActor>();
        }
        public void AddPlayer(int userId, MmoActor player)
        {
            lock (SyncRoot)
            {
                if (!players.ContainsKey(userId)) players.Add(userId, player);
            }
        }
        public void RemovePlayer(int userId)
        {
            lock (SyncRoot)
            {
                if (players.ContainsKey(userId)) players.Remove(userId);
            }
        }
        public MmoActor GetPlayer(int userId)
        {
            return players.ContainsKey(userId) ? players[userId] : null;
        }
        public void BroadCastToWorld(byte evCode, object data)
        {
            foreach (var p in players)
                p.Value.SendEvent(evCode, data);
        }
        public void ClearAll()
        {
            players.Clear();
        }
        public void Dispose()
        {
            players?.Clear();
            GC.SuppressFinalize(this);
        }
    }
}

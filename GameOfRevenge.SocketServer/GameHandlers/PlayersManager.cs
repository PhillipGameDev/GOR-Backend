
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
        private readonly Dictionary<string, MmoActor> players;
        public PlayersManager()
        {
            this.players = new Dictionary<string, MmoActor>();
        }
        public void AddPlayer(string userName, MmoActor player)
        {
            lock (this.SyncRoot)
                if (!this.players.ContainsKey(userName))
                    this.players.Add(userName, player);
        }
        public void RemovePlayer(string userName)
        {
            lock (this.SyncRoot) if (this.players.ContainsKey(userName)) this.players.Remove(userName);
        }
        public MmoActor GetPlayer(string userName)
        {
            if (this.players.ContainsKey(userName))
                return this.players[userName];
            return null;
        }
        public void BroadCastToWorld(byte evCode, object data)
        {
            foreach (var p in this.players)
                p.Value.SendEvent(evCode, data);
        }
        public void ClearAll()
        {
            this.players.Clear();
        }
        public void Dispose()
        {
            if (this.players != null)
                this.players.Clear();
            GC.SuppressFinalize(this);
        }
    }
}


using System;
using System.Collections.Generic;
using GameOfRevenge.Interface;

namespace GameOfRevenge.GameHandlers
{
    public class PlayersManager : IPlayersManager, IDisposable
    {
        private readonly Dictionary<int, PlayerInstance> players = new Dictionary<int, PlayerInstance>();

        public PlayersManager()
        {
        }

        public void AddPlayer(int playerId, PlayerInstance player)
        {
            lock (players)
            {
                if (!players.ContainsKey(playerId)) players.Add(playerId, player);
            }
        }

        public void RemovePlayer(int playerId)
        {
            lock (players)
            {
                if (players.ContainsKey(playerId)) players.Remove(playerId);
            }
        }

        public PlayerInstance GetPlayer(int playerId)
        {
            PlayerInstance player = null;
            lock (players)
            {
                if (players.ContainsKey(playerId)) player = players[playerId];
            }

            return player;
        }

        public void BroadcastToWorld(byte evCode, object data)
        {
            List<PlayerInstance> list;
            lock (players)
            {
                list = new List<PlayerInstance>(players.Values);
            }

            foreach (var player in list)
            {
                player.SendEvent(evCode, data);
            }
        }

        public void ClearAll()
        {
            lock (players)
            {
                players.Clear();
            }
        }

        public void Dispose()
        {
            ClearAll();

            GC.SuppressFinalize(this);
        }
    }
}

﻿using ExitGames.Concurrency.Fibers;
using ExitGames.Logging;
using GameOfRevenge.GameApplication;
using GameOfRevenge.Helpers;
using GameOfRevenge.Interface;
using GameOfRevenge.Model;
using Photon.SocketServer;
using Photon.SocketServer.Concurrency;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GameOfRevenge.GameHandlers
{
    public class MmoActor : UserProfile
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        
        //that user is included of that IA
        public IWorld World { get; private set; }
        public Region Tile { get; private set; }
        public IInterestArea InterestArea { get; private set; }
        public IPlayerSocketDataManager PlayerDataManager { get; private set; }
        public IPlayerAttackHandler PlayerAttackHandler { get { return this.PlayerDataManager.AttackHandler; } }
        
        public MmoActor(string playerId, IWorld world, Region tile) : base(playerId)
        {
            World = world;
            Tile = tile;
        }
        
        public void PlayerSpawn(IInterestArea interestArea, IGorMmoPeer peer, IPlayerSocketDataManager playerDataManager)
        {
            Peer = peer;
            InterestArea = interestArea;
            PlayerDataManager = playerDataManager;
        }
        public void PlayerTeleport(Region tile)
        {
            Tile = tile;
        }

        public void JoinKingdomView()
        {
            log.InfoFormat("Join KIngdom View Call {0} ",this.PlayerId);
            
            IsInKingdomView = true;
            InterestUsers.Clear();
        }
        public void LeaveKingdomView()
        {
            IsInKingdomView = false;
            InterestUsers.Clear();
        }

        public void StartOnReal()
        {
            Fiber = new PoolFiber();
            Fiber.Start();
        }
        public void StopOnReal()
        {
            try
            {
                if (PlayerDataManager != null)
                {
                    PlayerDataManager.Dispose();
                    PlayerDataManager = null;
                }

                if (Fiber != null) Fiber.Dispose();

                if (InterestArea != null)
                {
                    InterestArea.Dispose();
                    InterestArea = null;
                }
            }
            catch (Exception ex)
            {
                log.Error("Exception in StopOnReal", ex);
            }
        }
    }
}

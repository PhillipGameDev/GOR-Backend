using System;
using ExitGames.Concurrency.Fibers;
using ExitGames.Logging;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Interface;

namespace GameOfRevenge.GameHandlers
{
    public class MmoActor : UserProfile
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        
        //that user is included of that IA
        public IWorld World { get; private set; }
        public Region WorldRegion { get; private set; }
        public IInterestArea InterestArea { get; private set; }
        public IPlayerSocketDataManager InternalPlayerDataManager { get; private set; }
        public IPlayerAttackHandler PlayerAttackHandler => InternalPlayerDataManager.AttackHandler;
        
        public MmoActor(int playerId, PlayerInfo playerInfo, IWorld world, Region region) : base(playerId, playerInfo)
        {
            World = world;
            WorldRegion = region;
            //playerInfo assigned on base
        }
        
        public void PlayerSpawn(IInterestArea interestArea, IGorMmoPeer peer, IPlayerSocketDataManager playerDataManager)
        {
            Peer = peer;
            InterestArea = interestArea;
            InternalPlayerDataManager = playerDataManager;
        }
        public void PlayerTeleport(Region region)
        {
            WorldRegion = region;
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
                if (InternalPlayerDataManager != null)
                {
                    InternalPlayerDataManager.Dispose();
                    InternalPlayerDataManager = null;
                }

                if (Fiber != null)
                {
                    Fiber.Dispose();
                    Fiber = null;
                }

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

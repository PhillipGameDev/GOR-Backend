using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExitGames.Concurrency.Fibers;
using ExitGames.Logging;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Quest;
using GameOfRevenge.Common.Services;
using GameOfRevenge.GameApplication;
using GameOfRevenge.Interface;
using GameOfRevenge.Model;

namespace GameOfRevenge.GameHandlers
{
    public class PlayerInstance : UserProfile
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        
        //that user is included of that IA
        public IWorld World { get; private set; }
        public Region WorldRegion { get; private set; }
        public IInterestArea InterestArea { get; private set; }
        public IPlayerSocketDataManager InternalPlayerDataManager { get; private set; }
        public IPlayerAttackHandler PlayerAttackHandler => InternalPlayerDataManager.AttackHandler;
        
        public PlayerInstance(int playerId, PlayerInfo playerInfo, IWorld world, Region region) : base(playerId, playerInfo)
        {
            World = world;
            WorldRegion = region;
            //playerInfo assigned on base
        }
        
        public void PlayerSpawn(IInterestArea interestArea, IGorMmoPeer peer, IPlayerSocketDataManager playerDataManager, PlayerInfo playerInfo)
        {
            InterestArea = interestArea;
            Peer = peer;
            InternalPlayerDataManager = playerDataManager;
            peer.PlayerInstance = this;//vice versa mmoactor into the peer and reverse
            PlayerInfo = playerInfo;

            var profile = new UserProfileResponse(playerInfo, WorldRegion.X, WorldRegion.Y);
            SendEvent(EventCode.UserProfile, profile);

            log.InfoFormat("Send profile data to client X {0} Y {1} userName {2} castle {3} ",
                            profile.X, profile.Y, profile.UserName, profile.CastleLevel);
        }

        public async Task SendEventToAlliance(EventCode eventCode, object data, bool includeOwner = false)
        {
            if ((Peer != null) && (PlayerInfo.AllianceId > 0))
            {
                var resp = await GameService.BClanManager.GetClanMembers(PlayerInfo.AllianceId);
                if (resp.IsSuccess && resp.HasData)
                {
                    var members = resp.Data.ConvertAll(x => x.PlayerId);
                    SendEventToUsers(members, eventCode, data, includeOwner);
                }
            }
        }

        public void TryAddPlayerQuestData()
        {
            GameService.RealTimeUpdateManagerQuestValidator.TryAddPlayerQuestData(PlayerId, QuestAction);
        }

        void QuestAction(PlayerQuestDataTable questUpdated)
        {
            if (questUpdated == null) return;

            var objResp = new QuestUpdateResponse()
            {
                IsSuccess = true,
                Message = "Update quest data",

                QuestId = questUpdated.QuestId,
                Completed = questUpdated.Completed,
                ProgressData = questUpdated.Completed ? null : questUpdated.ProgressData
            };

            new DelayedAction().WaitForCallBack(() =>
            {
                try
                {
                    SendEvent(EventCode.UpdateQuest, objResp);
                }
                catch (Exception ex)
                {
                    log.Info("EXCEPTION sending event " + ex.Message);
                }
            }, 100);
        }

        public void PlayerTeleport(Region region)
        {
            WorldRegion = region;
        }

        public void JoinKingdomView()
        {
            log.InfoFormat("Join Kingdom View Call {0} ",this.PlayerId);
            
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

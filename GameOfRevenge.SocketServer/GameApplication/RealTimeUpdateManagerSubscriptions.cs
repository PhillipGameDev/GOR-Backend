using System;
using System.Linq;
using ExitGames.Logging;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Services;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Business.CacheData;

namespace GameOfRevenge.GameApplication
{
    public class RealTimeUpdateManagerSubscriptions
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly IUserShopManager shopManager;

        private bool running;
        private int currentCount;
        private DateTime tomorrow;
        private DateTime lastUpdate;
        private int ticks;

        private const int UPDATE_INTERVAL = 5000;

        public RealTimeUpdateManagerSubscriptions(IUserShopManager userShopManager)
        {
            shopManager = userShopManager;
            Running = true;

            tomorrow = DateTime.UtcNow.AddDays(1).Date;
            lastUpdate = DateTime.UtcNow;
        }

        public bool Running
        {
            get => running;
            set
            {
                running = value;
                if (running) UpdateAsync();
            }
        }

        public void UpdateAsync()
        {
            if ((currentCount == 0) && (DateTime.UtcNow >= tomorrow))
            {
                RunDailyProcess(2);
            }
            if ((DateTime.UtcNow - lastUpdate).TotalMilliseconds < UPDATE_INTERVAL) return;

            lastUpdate = DateTime.UtcNow;
            ticks++;
            if (ticks > 100)
            {
                log.Info("subscription thread alive");
                ticks = 0;
            }

            if (running) new DelayedAction().WaitForCallBack(UpdateAsync, UPDATE_INTERVAL + 1);
        }

        private void RunDailyProcess(int count)
        {
            log.Info("RunDailyProcess: " + count);
            currentCount = count;
            if (currentCount == 0) return;

            new DelayedAction().WaitForCallBack(async () =>
            {
                var packageLists = CacheShopDataManager.AllSubscriptionPackages;
                var noErrors = true;
                var subscriptionResp = await shopManager.GetAllSubscriptionsNotRewarded();
                if (subscriptionResp.IsSuccess && subscriptionResp.HasData)
                {
                    foreach (var subscription in subscriptionResp.Data)
                    {
                        var subscriptionId = subscription.SubscriptionId;
                        var productId = subscription.ProductId;
                        var package = packageLists.FirstOrDefault(x => x.Name == productId);
                        if (package == null)
                        {
                            log.Error("Package "+productId+" not found for subscription "+subscriptionId);
                            continue;
                        }

                        var playerId = subscription.PlayerId;
                        var resp = await shopManager.AddSubscriptionDailyReward(playerId, subscriptionId);
                        if (!resp.IsSuccess)
                        {
                            log.Error("Failed to register daily reward: " + resp.Message);
                            noErrors = false;
                            continue;
                        }

                        var collectResp = await shopManager.CollectPackage(playerId, package);
                        if (collectResp != ReturnCode.OK)
                        {
                            log.Error("Failed to collect rewards for subscription "+subscriptionId+"(player:"+playerId+")");
                        }
                    }
                }

                if (noErrors)
                {
                    tomorrow = DateTime.UtcNow.AddDays(1).Date;
                    count = 0;
                }
                else
                {
                    count--;
                }

                RunDailyProcess(count);
            }, 1000);
        }
    }
}
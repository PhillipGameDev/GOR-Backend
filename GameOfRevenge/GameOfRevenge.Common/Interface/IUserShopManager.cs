using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.PlayerData;

namespace GameOfRevenge.Common.Interface
{
    public interface IUserShopManager
    {
        Task<Response> BuyShopItem(int playerId, int itemId);
        Task<Response> BuyPackage(int playerId, int packageId);
        Task<Response> RedeemPurchaseShopItem(int playerId, IReadOnlyShopItemTable shopItem);
        Task<Response> RedeemPurchasePackage(int playerId, int packageId);
        Task<ReturnCode> CollectPackage(int playerId, PackageList package);

        Task<Response> AddSubscription(int playerId, string store, string transactionId, DateTime transactionDate, string productId, int days);
        Task<Response<SubscriptionTable>> GetSubscription(int playerId);
        Task<Response> ValidateSubscription(int playerId, string transactionId, bool active);
        Task<Response> AddSubscriptionDailyReward(int playerId, int subscriptionId);
        Task<Response<List<SubscriptionProduct>>> GetAllSubscriptionsNotRewarded();
    }
}

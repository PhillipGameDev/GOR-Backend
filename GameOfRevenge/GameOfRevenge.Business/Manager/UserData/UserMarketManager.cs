using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Interface;

namespace GameOfRevenge.Business.Manager.UserData
{
    public class UserMarketManager : BaseManager, IUserMarketManager
    {
        public async Task<Response<List<MarketGiftTable>>> GetAllGifts(int playerId)
        {
            var spParam = new Dictionary<string, object>()
            {
                { "PlayerId", playerId },
            };

            return await Db.ExecuteSPMultipleRow<MarketGiftTable>("GetAllGiftResource", spParam);
        }

        public async Task<Response> GiftResource(int playerId, int toPlayerId, int food, int wood, int ore)
        {
            var spParam = new Dictionary<string, object>()
            {
                { "FromPlayerId", playerId },
                { "TargetPlayerId", toPlayerId },
                { "Food", food },
                { "Wood", wood },
                { "Ore", ore }
            };

            return await Db.ExecuteSPNoData("GiftResource", spParam);
        }

        public async Task<Response> RedeemGiftResource(int giftid)
        {
            var spParam = new Dictionary<string, object>()
            {
                { "MarketPlayerDataId", giftid },
            };

            return await Db.ExecuteSPNoData("RedeemGiftResource", spParam);
        }

        public async Task<Response> BuyProduct(int playerId, int food, int wood, int ore)
        {
            var spParam = new Dictionary<string, object>()
            {
                { "PlayerId", playerId },
                { "Food", food },
                { "Wood", wood },
                { "Ore", ore }
            };

            return await Db.ExecuteSPNoData("BuyProduct", spParam);
        }
    }
}

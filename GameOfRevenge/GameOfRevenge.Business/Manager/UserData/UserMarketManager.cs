using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Common;

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

        public async Task<Response> RedeemPurchaseProduct(int playerId, string productId)
        {
//            if (productId already redeemed) return new Response(201, "Product already redemeed");

            var iapProducts = CacheProductDataManager.GetPackageProducts();
            var pack = iapProducts.FirstOrDefault(x => (x.ProductId == productId));
            if (pack != null)
            {
                await UserQuestManager.CollectRewards(playerId, pack.Rewards); //TODO:implement response error

                //TODO: register transacton
/*                var redemeedResp = await Db.ExecuteSPNoData("RedeemChapterReward", new Dictionary<string, object>()
                {
                    { "PlayerChapterUserId", chapterData.ChapterUserDataId }
                });*/

                return new Response(CaseType.Success, "Product redeemed");
            }
            else
            {
                return new Response(CaseType.Error, "Invalid product");
            }
        }
    }
}

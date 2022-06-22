using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Business.CacheData;

namespace GameOfRevenge.WebServer.Controllers.Api
{
    public class MarketController : BaseApiController
    {
        private readonly IUserStructureManager userStructureManager;
        private readonly IUserMarketManager userMarketManager;

        public MarketController(IUserStructureManager userStructureManager, IUserMarketManager userMarketManager)
        {
            this.userStructureManager = userStructureManager;
            this.userMarketManager = userMarketManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGifts() => ReturnResponse(await userMarketManager.GetAllGifts(Token.PlayerId));


        [HttpPost]
        public async Task<IActionResult> RedeemGiftResource(int giftId) => ReturnResponse(await userMarketManager.RedeemGiftResource(giftId));


        [HttpPost]
        public async Task<IActionResult> GiftResource(int toPlayerId, int food, int wood, int ore) => ReturnResponse(await userStructureManager.GiftResource(Token.PlayerId, toPlayerId,  food,  wood,  ore));


        [HttpGet]
        public IActionResult GetAllProducts()
        {
            var response = CacheProductDataManager.ProductList;
            return ReturnResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> BuyProduct(int food, int wood, int ore) => ReturnResponse(await userMarketManager.BuyProduct(Token.PlayerId, food, wood, ore));
    }
}

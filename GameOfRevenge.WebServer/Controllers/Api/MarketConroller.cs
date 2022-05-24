using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Common.Interface;

namespace GameOfRevenge.WebServer.Controllers.Api
{
    public class MarketController : BaseApiController
    {
        private readonly IUserStructureManager userStructureManager;
        private readonly IMarketManager marketManager;

        public MarketController(IUserStructureManager userStructureManager, IMarketManager marketManager)
        {
            this.userStructureManager = userStructureManager;
            this.marketManager = marketManager;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllGifts() => ReturnResponse(await marketManager.GetAllGifts(Token.PlayerId));


        [HttpPost]
        public async Task<IActionResult> RedeemGiftResource(int giftId) => ReturnResponse(await marketManager.RedeemGiftResource(giftId));


        [HttpPost]
        public async Task<IActionResult> GiftResource(int toPlayerId, int food, int wood, int ore) => ReturnResponse(await userStructureManager.GiftResource(Token.PlayerId, toPlayerId,  food,  wood,  ore));
    }
}

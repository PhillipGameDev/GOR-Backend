using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Common.Interface
{
    public interface IUserMarketManager
    {
        Task<Response<List<MarketGiftTable>>> GetAllGifts(int playerId);
        Task<Response> GiftResource(int playerId, int toPlayerId, int food, int wood, int ore);
        Task<Response> RedeemGiftResource(int giftid);
        Task<Response> BuyProduct(int playerId, int food, int wood, int ore);

        Task<Response> RedeemPurchaseProduct(int playerId, string productId);
    }
}

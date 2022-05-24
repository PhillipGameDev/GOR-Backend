using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Common.Interface
{
    public interface IMarketManager
    {
        Task<Response<List<MarketGiftTable>>> GetAllGifts(int playerId);
        Task<Response> RedeemGiftResource(int giftid);
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Common.Interface
{
    public interface IUserShopManager
    {
        Task<Response> BuyShopItem(int playerId, int itemId);
        Task<Response> BuyPackage(int playerId, int packageId);
        Task<Response> RedeemPurchaseShopItem(int playerId, IReadOnlyShopItemTable shopItem);
        Task<Response> RedeemPurchasePackage(int playerId, int packageId);
    }
}

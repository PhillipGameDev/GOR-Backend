using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Interface;

namespace GameOfRevenge.Business.Manager.GameDef
{
    public class MarketManager : BaseManager, IMarketManager
    {
        public async Task<Response<List<MarketProductTable>>> GetAllProducts()
        {
            return await Db.ExecuteSPMultipleRow<MarketProductTable>("GetAllProducts", (s) => { });
        }
    }
}

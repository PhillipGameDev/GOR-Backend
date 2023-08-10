using System.Threading.Tasks;
using System.Collections.Generic;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Business.Manager.Base;

namespace GameOfRevenge.Business.Manager.GameDef
{
    public class MarketManager : BaseManager, IMarketManager
    {
        public async Task<Response<List<MarketProductTable>>> GetAllProducts()
        {
            return await Db.ExecuteSPMultipleRow<MarketProductTable>("GetAllProducts");
        }

        public async Task<Response<List<StorePackageTable>>> GetStorePackages(bool? active = null)
        {
            var sdParams = new Dictionary<string, object>();
            if (active != null) sdParams.Add("Active", active);

            return await Db.ExecuteSPMultipleRow<StorePackageTable>("GetStorePackages", sdParams);
        }
    }
}

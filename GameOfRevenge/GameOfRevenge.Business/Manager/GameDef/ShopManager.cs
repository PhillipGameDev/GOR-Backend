using System.Threading.Tasks;
using System.Collections.Generic;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Business.Manager.Base;

namespace GameOfRevenge.Business.Manager.GameDef
{
    public class ShopManager : BaseManager, IShopManager
    {
        public async Task<Response<List<ShopCategoryTable>>> GetAllShopCategories()
        {
            return await Db.ExecuteSPMultipleRow<ShopCategoryTable>("GetAllShopCategories");
        }
        public async Task<Response<List<ShopItemTable>>> GetAllShopItems()
        {
            return await Db.ExecuteSPMultipleRow<ShopItemTable>("GetAllShopItems");
        }
        public async Task<Response<List<PackageListTable>>> GetAllPackageLists()
        {
            return await Db.ExecuteSPMultipleRow<PackageListTable>("GetAllPackageLists");
        }
        public async Task<Response<List<PackageItemTable>>> GetAllPackageItems()
        {
            return await Db.ExecuteSPMultipleRow<PackageItemTable>("GetAllPackageItems");
        }
    }
}

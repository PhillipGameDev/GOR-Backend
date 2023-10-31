using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Common.Interface
{
    public interface IShopManager
    {
        Task<Response<List<ShopCategoryTable>>> GetAllShopCategories();
        Task<Response<List<ShopItemTable>>> GetAllShopItems();
        Task<Response<List<PackageListTable>>> GetAllPackageLists();
        Task<Response<List<PackageItemTable>>> GetAllPackageItems();
    }
}

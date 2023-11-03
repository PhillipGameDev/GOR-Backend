using GameOfRevenge.Common.Models.Inventory;
using GameOfRevenge.Common.Net;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameOfRevenge.Common.Interface
{
    public interface IInventoryManager
    {
        Task<Response<List<InventoryTable>>> GetAllInventoryItems();
        Task<Response<List<InventoryDataTable>>> GetAllInventoryData();
    }
}

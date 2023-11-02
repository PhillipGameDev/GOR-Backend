using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Common.Interface
{
    public interface IItemManager
    {
        Task<Response<List<ItemTable>>> GetAllItems();
    }
}

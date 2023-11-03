using System.Threading.Tasks;
using System.Collections.Generic;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Business.Manager.Base;

namespace GameOfRevenge.Business.Manager.GameDef
{
    public class ItemManager : BaseManager, IItemManager
    {
        public async Task<Response<List<ItemTable>>> GetAllItems()
        {
            return await Db.ExecuteSPMultipleRow<ItemTable>("GetAllItems");
        }
    }
}

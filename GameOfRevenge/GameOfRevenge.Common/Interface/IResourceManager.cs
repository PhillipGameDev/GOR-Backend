using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common.Interface.Model;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Common.Interface
{
    public interface IResourceManager
    {
        Task<Response<List<ResourceTable>>> GetAllResources();
    }
}

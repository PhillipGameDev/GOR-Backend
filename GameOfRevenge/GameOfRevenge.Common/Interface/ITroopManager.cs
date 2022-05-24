using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Troop;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Common.Interface
{
    public interface ITroopManager
    {
        Task<Response<List<TroopDataRequirementRel>>> GetAllTroopDataRequirementRel();
        Task<Response<List<TroopTable>>> GetAllTroops();
        Task<Response<List<TroopDataTable>>> GetAllTroopDatas();
        Task<Response<List<DataRequirement>>> GetAllTroopDataRequirements();
    }
}

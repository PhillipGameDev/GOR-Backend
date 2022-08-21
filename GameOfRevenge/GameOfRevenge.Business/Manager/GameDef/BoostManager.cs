using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common.Models.Boost;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Business.Manager.GameDef
{
    public class BoostManager : BaseManager
    {
//        public async Task<Response<List<BoostTable>>> GetAllBoosts() => await Db.ExecuteSPMultipleRow<BoostTable>("GetAllBoosts");
        public async Task<Response<List<BoostTypeTable>>> GetAllBoostTypes() => await Db.ExecuteSPMultipleRow<BoostTypeTable>("GetAllBoostTypes");
/*        public async Task<Response<List<BoostTypeRel>>> GetAllBoostRelData()
        {
            var types = await GetAllBoostTypes();
            if (!types.IsSuccess) return new Response<List<BoostTypeRel>>(types.Case, types.Message);

            var boosts = await GetAllBoosts();
            if (!boosts.IsSuccess) return new Response<List<BoostTypeRel>>(boosts.Case, boosts.Message);

            var response = new Response<List<BoostTypeRel>>(new List<BoostTypeRel>(), CaseType.Success, "Boost rel datas");

            foreach (var type in types.Data)
            {
                var boostData = new BoostTypeRel()
                {
                    Info = type,
                    Values = boosts.Data.Where(x=>x.BoostTypeId == type.BoostTypeId).ToList()
                };

                response.Data.Add(boostData);
            }

            return response;
        }*/
    }
}

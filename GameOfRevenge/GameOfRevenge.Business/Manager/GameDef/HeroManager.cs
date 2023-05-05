using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Services;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Hero;
using System.Linq;
using GameOfRevenge.Business.CacheData;

namespace GameOfRevenge.Business.Manager.GameDef
{
    public class HeroManager : BaseManager
    {
        public async Task<Response<List<HeroTable>>> GetAllHeroes() => await Db.ExecuteSPMultipleRow<HeroTable>("GetAllHeroes");
        public async Task<Response<List<HeroRequirementTable>>> GetAllHeroRequirements() => await Db.ExecuteSPMultipleRow<HeroRequirementTable>("GetAllHeroRequirements");
        public async Task<Response<List<HeroBoostTable>>> GetAllHeroBoosts() => await Db.ExecuteSPMultipleRow<HeroBoostTable>("GetAllHeroBoosts");
        public async Task<Response<List<HeroDataRel>>> GetAllHeroDataRelation() => await Db.ExecuteSPMultipleRow<HeroDataRel>("GetAllHeroDataRelation");

        public async Task<Response<List<HeroDataRequirementRel>>> GetAllHeroDatas()
        {
            var heroes = await GetAllHeroes();
            if (!heroes.IsSuccess) return new Response<List<HeroDataRequirementRel>>(heroes.Case, heroes.Message);


            var reqs = await GetAllHeroRequirements();
            if (!reqs.IsSuccess) return new Response<List<HeroDataRequirementRel>>(reqs.Case, reqs.Message);

            var boosts = await GetAllHeroBoosts();
            if (!boosts.IsSuccess) return new Response<List<HeroDataRequirementRel>>(boosts.Case, boosts.Message);

            var resp = new Response<List<HeroDataRequirementRel>>(new List<HeroDataRequirementRel>(), heroes.Case, heroes.Message);
            var structures = CacheStructureDataManager.StructureInfos;

            foreach (var hero in heroes.Data)
            {
                var data = new HeroDataRequirementRel
                {
                    Info = hero,
                    Requirements = reqs.Data.Where(x => x.HeroId == hero.HeroId).ToList(),
                    Boosts = boosts.Data.Where(x => x.HeroId == hero.HeroId).ToList()
                };

                foreach (var req in data.Requirements)
                {
                    foreach (var structure in structures)
                    {
                        foreach (var buildings in structure.Levels)
                        {
                            if (buildings.Data.DataId == req.StructureDataId)
                            {
                                req.StructureId = buildings.Data.InfoId;
                            }
                        }
                    }
                }

                resp.Data.Add(data);
            }

            return resp;
        }
    }
}

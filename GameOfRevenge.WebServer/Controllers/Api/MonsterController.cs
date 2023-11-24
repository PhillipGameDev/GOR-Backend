using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using GameOfRevenge.Common.Interface;
using Microsoft.AspNetCore.Authorization;
using GameOfRevenge.Business.CacheData;

namespace GameOfRevenge.WebServer.Controllers.Api
{
    public class MonsterController : BaseApiController
    {
        private readonly IMonsterManager monsterManager;
        private readonly IPlayerDataManager playerDataManager;

        public MonsterController(IMonsterManager monsterManager, IPlayerDataManager playerDataManager)
        {
            this.monsterManager = monsterManager;
            this.playerDataManager = playerDataManager;
        }

        #region Monsters
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAllMonsterData() => ReturnResponse(CacheMonsterManager.AllItems);

        [HttpGet]
        public async Task<IActionResult> GetAllWorldMonsters()
        {
            var response = await monsterManager.GetMonstersByWorldTileId(Token.WorldTileId);
            return ReturnResponse(response);
        }
        #endregion
    }
}

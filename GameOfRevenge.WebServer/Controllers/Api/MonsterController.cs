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
        private readonly IAccountManager accountManager;

        public MonsterController(IMonsterManager monsterManager, IPlayerDataManager playerDataManager, IAccountManager accountManager)
        {
            this.monsterManager = monsterManager;
            this.playerDataManager = playerDataManager;
            this.accountManager = accountManager;
        }

        #region Monsters
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAllMonsterData() => ReturnResponse(CacheMonsterManager.AllItems);

        [HttpGet]
        public async Task<IActionResult> GetAllWorldMonsters()
        {
            var resp = accountManager.GetAccountInfo(Token.PlayerId);
            var playerInfo = resp.Result.Data;
            var response = await monsterManager.GetMonstersByWorldTileId(playerInfo.WorldTileId);
            return ReturnResponse(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetNearestMonster()
        {
            var response = await monsterManager.GetNearestMonsterByPlayerId(Token.PlayerId);
            return ReturnResponse(response);
        }
        #endregion
    }
}

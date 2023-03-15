using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.WebServer.Services;
using GameOfRevenge.Business;

namespace GameOfRevenge.WebServer.Controllers.Api
{
    public class AccountController : BaseApiController
    {
        private readonly IAccountManager accountManager;

        public AccountController(IAccountManager accountManager)
        {
            this.accountManager = accountManager;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> TryLoginOrRegister(string identifier, string name, bool accept, int version = 0)
        {
            var response = await accountManager.TryLoginOrRegister(identifier, name, accept, version);
            if (response.IsSuccess && response.HasData) response.Data.GenerateToken();

            return ReturnResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeName(string name)
        {
            var response = await accountManager.SetProperties(Token.PlayerId, name: name);
            return ReturnResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> Debug(int dip)
        {
            var response = await accountManager.Debug(Token.PlayerId, dip);
            return ReturnResponse(response);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> GetAccountInfoByIdentifier(string identifier)
        {
            var response = await accountManager.GetAccountInfo(identifier);
            return ReturnResponse(response);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> GetAccountInfoById(int id)
        {
            var response = await accountManager.GetAccountInfo(id);
            return ReturnResponse(response);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> GetTutorialInfo(string playerId)
        {
            var response = await accountManager.GetTutorialInfo(playerId);
            return ReturnResponse(response);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateTutorialInfo(string playerId, string data, bool isComplete)
        {
            var response = await accountManager.UpdateTutorialInfo(playerId, data, isComplete);
            return ReturnResponse(response);
        }
    }
}

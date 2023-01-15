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
            var response = await accountManager.ChangeName(Token.PlayerId, name);
            return ReturnResponse(response);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> GetAccountInfoByIdentifier(string identifier)
        {
            var response = await accountManager.GetAccountInfo(identifier);
            response.Data.JwtToken = string.Empty;
            return ReturnResponse(response);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> GetAccountInfoById(int id)
        {
            var response = await accountManager.GetAccountInfo(id);
            if (response.IsSuccess)
                response.Data.JwtToken = string.Empty;
            return ReturnResponse(response);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> GetTutorialInfo(string identifier)
        {
            var response = await accountManager.GetTutorialInfo(identifier);
            return ReturnResponse(response);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateTutorialInfo(string identifier, string data, bool isComplete)
        {
            var response = await accountManager.UpdateTutorialInfo(identifier, data, isComplete);
            return ReturnResponse(response);
        }
    }
}

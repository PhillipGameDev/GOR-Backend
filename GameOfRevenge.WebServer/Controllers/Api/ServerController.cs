using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.WebServer.Services;
using GameOfRevenge.Business;

namespace GameOfRevenge.WebServer.Controllers.Api
{
    public class ServerController : BaseApiController
    {
        private readonly IAccountManager accountManager;

        public ServerController()
        {
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetCurrentServerTimeFormated()
        {
            return ReturnResponse(Config.UtcTime.ToString("dd-MM-yyyy HH-mm-ss"));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetCurrentServerTime()
        {
            return ReturnResponse(Config.UtcTime.ToString());
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ResetConnection()
        {
            await Startup.ReloadDataBaseDataAsync();
            var response = new Response(CaseType.Success, "Server database reset succesfully");
            return ReturnResponse(response);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult CheckConnection()
        {
            return ReturnResponse("Server is working");
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.WebServer.Services;

namespace GameOfRevenge.WebServer.Controllers.Api
{
    public class AccountController : BaseApiController
    {
        private readonly IAccountManager accountManager;

        public AccountController(IAccountManager accountManager)
        {
            this.accountManager = accountManager;
        }

/*        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> TryLoginOrRegister(string identifier, string name, bool accept, int version = 0)
        {
            var response = await accountManager.TryLoginOrRegister(identifier, accept, version);
            if (response.IsSuccess && response.HasData) response.Data.GenerateToken();

            return ReturnResponse(response);
        }*/

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Handshake(string identifier, bool accept, int version, string platform)
        {
            var response = await accountManager.Handshake(identifier, accept, version, platform);
            if (response.IsSuccess && response.HasData)
            {
                string str;
                if (response.Data.IsDeveloper)
                {
                    //we can opt to change the server for this user
                    //webserver and photonserver address:port
                    str = "141.95.53.0:9001,141.95.53.0:4530";
                }
                else//default
                {
                    //we return ports for webserver and photonserver
                    if (version >= 915)//https
                    {
                        str = "2053,gameofrevenge.com:2083";
                    }
                    else//http
                    {
                        str = "9001,4530";
                    }
                }
                str = StringCipher.Encrypt(str, "2r2#818ir98$&@av");
                string data = str;
                if (version >= 10000)//1.00.00
                {
                    var config = new GameConfig()
                    {
                        ServerConfig = data,
                        Data1Version = 1,
                        Data2Version = 3,
                        Data3Version = 1,
//                        PolicyURL = "https://www.privacypolicygenerator.info/live.php?token=RPjMfHISZFvhOyqUjAQwEvhckbyG4N4Y",
//                        ContactURL = "https://gamelegendstudio.com/contact-us",
//                        VetURL = "https://discord.gg/QEBNBteg"
                    };
                    switch (platform)
                    {
                        case "IOS": config.ShareURL = "https://apps.apple.com/us/app/id1662056046"; break;
                        case "Android": config.ShareURL = "https://play.google.com/store/apps/details?id=com.gamelegendsestab.gameofrevenge"; break;
                    }
                    data = Newtonsoft.Json.JsonConvert.SerializeObject(config);
                }

                return ReturnResponse(new Common.Net.Response<string>() { Case = response.Case, Data = data, Message = response.Message });
            }
            else
            {
                return ReturnResponse(new Common.Net.Response<string>() { Case = response.Case, Data = null, Message = response.Message });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginOrRegister(string identifier, bool accept, int version, string platform)
        {
            var response = await accountManager.TryLoginOrRegister(identifier, accept, version, platform);
            if (response.IsSuccess && response.HasData) response.Data.GenerateToken();

            return ReturnResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeName(string name)
        {
            var response = await accountManager.ChangeName(Token.PlayerId, name: name);
            return ReturnResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> LinkAccount(string identifier)
        {
            var response = await accountManager.SetProperties(Token.PlayerId, firebaseId: identifier);
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

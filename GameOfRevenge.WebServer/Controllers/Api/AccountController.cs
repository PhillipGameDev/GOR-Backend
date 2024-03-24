using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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
//                    str = "141.95.53.0:9000,141.95.53.0:4530";//redirect all to dev server //9000 https, 9001 http
                }
                str = StringCipher.Encrypt(str, "2r2#818ir98$&@av");
                string data = str;
                if (version >= 10000)//1.00.00
                {
                    var config = new GameConfig()
                    {
                        ServerConfig = data,
                        Data1Version = 1,
                        Data2Version = 4,
                        Data3Version = 1,
//                        TermsURL = "https://www.gameofrevenge.com/termsofuse",
//                        PolicyURL = "https://www.gameofrevenge.com/privacypolicy",
//                        ContactURL = "https://gamelegendstudio.com/contact-us",
//                        VetURL = "https://discord.gg/QEBNBteg"
                    };
                    switch (platform)
                    {
                        case "IOS": config.ShareURL = "https://apps.apple.com/us/app/id1662056046"; break;
                        case "Android": config.ShareURL = "https://play.google.com/store/apps/details?id=com.gamelegendsestab.gameofrevenge"; break;
                    }
                    config.ShareURL = "https://www.gameofrevenge.com/app?join=" + response.Data.PlayerId;
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
        public async Task<IActionResult> LoginOrRegister(string identifier, bool accept, int version, string platform, int? referredPlayerId)
        {
            var response = await accountManager.TryLoginOrRegister(identifier, accept, version, platform, referredPlayerId);
            if (response.IsSuccess && response.HasData) response.Data.GenerateToken();

            return ReturnResponse(response);
        }

/*        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginOrRegisterWithReferredId(string identifier, int referredPlayerId, bool accept, int version, string platform)
        {
            var response = await accountManager.TryLoginOrRegister(identifier, accept, version, platform, referredPlayerId);
            if (response.IsSuccess && response.HasData) response.Data.GenerateToken();

            return ReturnResponse(response);
        }*/

        private string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "";
//            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ClaimRewards(string userId, string dynamicUserId, string platform, string placementName,
                                                    string itemName, int amount, string eventId, string timestamp, string signature)//country, publisherSubId
        {
            if (!GetLocalIPAddress().Contains("141.95.53.0"))
            {
                var url = "http://141.95.53.0:9001/api/account/claimRewards/";
                var args = "?userId=" + userId;
                args += "&dynamicUserId=" + dynamicUserId;
                args += "&platform=" + platform;
                args += "&placementName=" + placementName;
                args += "&itemName=" + itemName;
                args += "&amount=" + amount;
                args += "&eventId=" + eventId;
                args += "&timestamp=" + timestamp;
                args += "&signature=" + signature;
                var request = (HttpWebRequest)WebRequest.Create(url + args);
                request.Method = "GET";
                request.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

                using (var resp = (HttpWebResponse)request.GetResponse())
                {
                    return Ok(eventId + ":OK");

/*                    if (resp.StatusCode == HttpStatusCode.OK)
                    {
                    }
                    else
                    {
                        using (var stream = resp.GetResponseStream())
                        {
                            return StatusCode((int)resp.StatusCode, new StreamReader(stream).ReadToEnd());
                        }
                    }*/
                }
            }



            //            var remoteIpAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            var key = "";
            switch (platform)
            {
                case "IOS": key = "ff878f"; break;
                case "Android": key = "e9947b"; break;
            }
            var input = timestamp + eventId + userId + amount + key;
            string hash = null;
            using (var md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                var sb = new StringBuilder();
                for (int idx = 0; idx < hashBytes.Length; idx++)
                {
                    sb.Append(hashBytes[idx].ToString("x2"));
                }
                hash = sb.ToString();
            }

            if (hash == signature)
            {
                var playerId = 0;
                if (dynamicUserId != null)
                {
                    int.TryParse(dynamicUserId, out playerId);
                }
                else if (userId != null)
                {
                    int.TryParse(userId, out playerId);
                }

                var response = await accountManager.ClaimRewards(playerId, itemName, amount, eventId, timestamp);
                if (response.IsSuccess)
                {
                    if (response.Case == 100)
                    {
                    }
                }
            }

            return Ok(eventId + ":OK");
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

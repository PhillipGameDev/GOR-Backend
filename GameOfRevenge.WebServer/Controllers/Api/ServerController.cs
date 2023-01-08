using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.IO;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Business;
using Newtonsoft.Json;

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
            return ReturnResponse(Config.UtcTime.ToString("dd/MM/yyyy HH:mm:ss"));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetCurrentServerTime()
        {
            return ReturnResponse(Config.UtcTime.ToString("s"));
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

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> StoreGameData(int type, IFormFile file)
        {
            if (file?.Length > 0)
            {
                string contents = null;
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    contents = System.Text.Encoding.UTF8.GetString(stream.GetBuffer());
                }

                string json = null;
                try
                {
                    var tempObj = JsonConvert.DeserializeObject(contents);
                    json = JsonConvert.SerializeObject(tempObj);
                }
                catch
                {
                    return ReturnResponse("Error processing data");
                }

                var resp = await CacheStructureDataManager.StoreData(type, json);
                return ReturnResponse(resp);
            }

            return ReturnResponse("Error storing data");
        }
    }
}

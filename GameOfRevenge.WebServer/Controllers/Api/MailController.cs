using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using GameOfRevenge.Common.Interface;

namespace GameOfRevenge.WebServer.Controllers.Api
{
    public class MailController : BaseApiController
    {
        private readonly IUserMailManager mailManager;

        public MailController(IUserMailManager mailManager)
        {
            this.mailManager = mailManager;
        }

        [HttpPost]
        public async Task<IActionResult> ReadMail(int id)
        {
            var response = await mailManager.ReadMail(id);
            return ReturnResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMail(int id)
        {
            var response = await mailManager.DeleteMail(id);
            return ReturnResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> XSendMail(int id, string type, string content)
        {

            Common.Email.MailType mailType = (Common.Email.MailType)System.Enum.Parse(typeof(Common.Email.MailType), type, true);
            var response = await mailManager.SendMail(id, mailType, content);
//            var response = await mailManager.SendMail(id, Common.Email.MailType.Text, content);
            return ReturnResponse(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMail()
        {
            var response = await mailManager.GetAllMail(Token.PlayerId);
            return ReturnResponse(response);
        }
    }
}

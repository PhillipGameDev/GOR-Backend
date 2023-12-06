using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Email;

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
        public async Task<IActionResult> ReadMail(int mailId)
        {
            var response = await mailManager.ReadMail(mailId);
            return ReturnResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> SaveMail(int mailId, bool saved)
        {
            var response = await mailManager.SaveMail(mailId, saved);
            return ReturnResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMail(int mailId)
        {
            var response = await mailManager.DeleteMail(mailId);
            return ReturnResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> SendMail(int playerId, string subject, string message)
        {
            var mailMessage = new MailMessage()
            {
                Subject = subject,
                Message = message,
                SenderId = Token.PlayerId,
                SenderName = Token.PlayerIdentifier
            };
            string content = null;
            try
            {
                content = Newtonsoft.Json.JsonConvert.SerializeObject(mailMessage);
            }
            catch { }
            var response = await mailManager.SendMail(playerId, MailType.Message, content);
            return ReturnResponse(response);
        }

        public async Task<IActionResult> XSendMail(int mailId, string type, string content)
        {
            MailType mailType = (MailType)Enum.Parse(typeof(MailType), type, true);
            var response = await mailManager.SendMail(mailId, mailType, content);
//            var response = await mailManager.SendMail(id, Common.Email.MailType.Text, content);
            return ReturnResponse(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMail(int? lastId)
        {
            var response = await mailManager.GetAllMail(Token.PlayerId, lastId);
            return ReturnResponse(response);
        }
    }
}

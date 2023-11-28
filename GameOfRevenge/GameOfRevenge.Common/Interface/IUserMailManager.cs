using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common.Email;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Common.Interface
{
    public interface IUserMailManager
    {
        Task<Response> SendMail(int playerId, MailType type, string content);
        Task<Response> ReadMail(int mailId);
        Task<Response> SaveMail(int mailId, bool saved);
        Task<Response> DeleteMail(int mailId);
        Task<Response<List<MailTable>>> GetAllMail(int playerId);
        Task<Response<List<MailTable>>> GetAllMail(int playerId, DateTime lastTime);
    }
}

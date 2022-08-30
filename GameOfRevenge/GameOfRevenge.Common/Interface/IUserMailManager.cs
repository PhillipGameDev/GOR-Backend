﻿using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common.Email;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Common.Interface
{
    public interface IUserMailManager
    {
        Task<Response> SaveMail(int playerId, MailType type, string content);
        Task<Response> ReadMail(int mailId);
        Task<Response> DeleteMail(int mailId);
        Task<Response<List<MailTable>>> GetAllMail(int playerId);
    }
}

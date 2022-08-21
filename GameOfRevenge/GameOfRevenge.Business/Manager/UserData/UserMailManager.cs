﻿using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common.Email;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Net;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameOfRevenge.Business.Manager.UserData
{
    public class UserMailManager : BaseManager, IUserMailManager
    {
        public async Task<Response> DeleteMail(int mailId)
        {
            return await Db.ExecuteSPNoData("DeleteMail", new Dictionary<string, object>()
            {
                { "MailId", mailId }
            });
        }

        public async Task<Response<List<MailTable>>> GetAllMail(int playerId)
        {
            return await Db.ExecuteSPMultipleRow<MailTable>("GetAllMail", new Dictionary<string, object>()
            {
                { "PlayerId", playerId }
            });
        }

        public async Task<Response> ReadMail(int mailId)
        {
            return await Db.ExecuteSPNoData("ReadMail", new Dictionary<string, object>()
            {
                { "MailId", mailId }
            });
        }

        public async Task<Response> SendMail(int playerId, MailType type, string content)
        {
            return await Db.ExecuteSPNoData("CreateMail", new Dictionary<string, object>()
            {
                { "PlayerId", playerId },
                { "ContentType", type.ToString() },
                { "Content", content },
            });
        }
    }
}

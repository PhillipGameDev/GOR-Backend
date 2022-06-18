using GameOfRevenge.Common.Helper;
using GameOfRevenge.Common.Models.Table;
using Newtonsoft.Json;
using System;
using System.Data;

namespace GameOfRevenge.Common.Email
{
    public class MailTable : BaseMailType<string>, IBaseTable
    {
        public void LoadFromDataReader(IDataReader reader)
        {
            var index = 0;
            PrimaryId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            PlayerId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            MailType = reader.GetValue(index) == DBNull.Value ? default : reader.GetString(index).ToEnum<MailType>(); index++;
            Content = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            Read = reader.GetValue(index) != DBNull.Value && reader.GetBoolean(index); index++;
            Date = reader.GetValue(index) == DBNull.Value ? DateTime.MinValue : reader.GetDateTime(index);
        }

        public BaseMailType<T> GetType<T>()
        {
            return new BaseMailType<T>()
            {
                PrimaryId = PrimaryId,
                PlayerId = PlayerId,
                MailType = MailType,
                Read = Read,
                Date = Date,
                Content = JsonConvert.DeserializeObject<T>(Content)
            };
        }
    }
}

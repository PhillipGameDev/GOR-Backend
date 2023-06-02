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
            MailId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
//            PlayerId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            MailType = reader.GetValue(index) == DBNull.Value ? MailType.Unknown : (MailType)reader.GetByte(index); index++;
            Content = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            Read = reader.GetValue(index) != DBNull.Value && reader.GetBoolean(index); index++;
            Saved = reader.GetValue(index) != DBNull.Value && reader.GetBoolean(index); index++;
            Date = reader.GetValue(index) == DBNull.Value ? DateTime.MinValue : reader.GetDateTime(index);
        }

        public T GetContent<T>() where T : new()
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(Content);
            }
            catch (System.Exception)
            {
                return default;
            }
        }

/*        public BaseMailType<T> GetType<T>()
        {
            try
            {
                var content = JsonConvert.DeserializeObject<T>(Content);
                return new BaseMailType<T>()
                {
                    MailId = MailId,
    //                PlayerId = PlayerId,
                    MailType = MailType,
                    Read = Read,
                    Date = Date,
                    Content = content
                };
            }
            catch {}

            return null;
        }*/
    }
}

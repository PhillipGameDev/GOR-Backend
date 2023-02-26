using System;
using System.Data;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models
{
    public class Player : IBaseTable
    {
        public int PlayerId { get; set; }
        public string PlayerIdentifier { get; set; }
        public int RavasAccountId { get; set; }
        public string Name { get; set; }
        public bool AcceptedTermAndCondition { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsDeveloper { get; set; }
        public string JwtToken { get; set; }
        public int VIPPoints { get; set; }
        public int WorldTileId { get; set; }

        public PlayerInfo Info { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            var index = 0;
            PlayerId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            PlayerIdentifier = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            RavasAccountId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Name = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            AcceptedTermAndCondition = reader.GetValue(index) != DBNull.Value && reader.GetBoolean(index); index++;
            IsAdmin = reader.GetValue(index) != DBNull.Value && reader.GetBoolean(index); index++;
            IsDeveloper = reader.GetValue(index) != DBNull.Value && reader.GetBoolean(index); index++;
            VIPPoints = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            WorldTileId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;

            if (reader.GetValue(index) != DBNull.Value)
            {
                var info = reader.GetString(index);
                try
                {
                    Info = Newtonsoft.Json.JsonConvert.DeserializeObject<PlayerInfo>(info);
                }
                catch { }
            }

            JwtToken = string.Empty;
        }
    }
}

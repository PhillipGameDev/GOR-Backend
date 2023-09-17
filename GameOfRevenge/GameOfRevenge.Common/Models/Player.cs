using System;
using System.Data;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models
{
    public class Player : IBaseTable
    {
        public int PlayerId { get; set; }
        public string PlayerIdentifier { get; set; }
        public string FirebaseId { get; set; }
        public bool AcceptedTermAndCondition { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsDeveloper { get; set; }
        public int WorldTileId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public string Name { get; set; }
        public int VIPPoints { get; set; }

        public PlayerInfo Info { get; set; }

        public string JwtToken { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            var index = 0;
            PlayerId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            PlayerIdentifier = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            FirebaseId = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            AcceptedTermAndCondition = reader.GetValue(index) != DBNull.Value && reader.GetBoolean(index); index++;
            IsAdmin = reader.GetValue(index) == DBNull.Value ? false : reader.GetBoolean(index); index++;
            IsDeveloper = reader.GetValue(index) == DBNull.Value ? false : reader.GetBoolean(index); index++;
            WorldTileId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            X = reader.GetValue(index) == DBNull.Value ? -1 : reader.GetInt32(index); index++;
            Y = reader.GetValue(index) == DBNull.Value ? -1 : reader.GetInt32(index); index++;

            Name = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            VIPPoints = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;

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

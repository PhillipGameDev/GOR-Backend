using System;
using System.Data;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models.Structure
{
    public class MarketGiftTable : IBaseTable
    {
        public long MarketGiftTableId { get; set; }
        public int FromPlayerId { get; set; }
        public string FromPlayerName { get; set; }
        public int ToPlayerId { get; set; }
        public DateTime StartTime { get; set; }
        public bool IsRedeemed { get; set; }
        public int Food { get; set; }
        public int Wood { get; set; }
        public int Ore { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            MarketGiftTableId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt64(index); index++;
            FromPlayerId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            FromPlayerName = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            ToPlayerId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            StartTime = reader.GetValue(index) == DBNull.Value ? DateTime.MinValue : reader.GetDateTime(index); index++;
            IsRedeemed = reader.GetValue(index) == DBNull.Value ? false: reader.GetBoolean(index); index++;
            Food = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Wood = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Ore = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index);
        }
    }
}

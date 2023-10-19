using System;
using System.Data;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models
{
    public class GloryKingdomData : IBaseTable
    {
        public int GloryKingdomEventId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            GloryKingdomEventId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            StartTime = reader.GetValue(index) == DBNull.Value ? DateTime.MaxValue : reader.GetDateTime(index); index++;
            EndTime = reader.GetValue(index) == DBNull.Value ? DateTime.MinValue : reader.GetDateTime(index);
        }
    }
}

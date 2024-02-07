using System;
using System.Data;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models.PlayerData
{
    public class SubscriptionProduct : IBaseTable
    {
        public int PlayerId { get; set; }
        public int SubscriptionId { get; set; }
        public string ProductId { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            PlayerId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            SubscriptionId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            ProductId = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index);
        }
    }
}

using System;
using System.Data;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models.PlayerData
{
    public class SubscriptionTable : IBaseTable
    {
        public int SubscriptionId { get; set; }
//        public int PlayerId { get; set; }
        public string TransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime LastModified { get; set; }
        public string ProductId { get; set; }
        public string Store { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            SubscriptionId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
//            PlayerId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            TransactionId = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            TransactionDate = reader.GetValue(index) == DBNull.Value ? DateTime.MinValue : reader.GetDateTime(index); index++;
            EndDate = reader.GetValue(index) == DBNull.Value ? DateTime.MinValue : reader.GetDateTime(index); index++;
            LastModified = reader.GetValue(index) == DBNull.Value ? DateTime.MinValue : reader.GetDateTime(index); index++;

            ProductId = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            Store = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index);
        }
    }
}

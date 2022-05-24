using GameOfRevenge.Common.Models.Table;
using System;
using System.Data;

namespace GameOfRevenge.Common.Models
{
    public class PlayerTutorialData : IBaseTable
    {
        public int PlayerTutorialId { get; set; }
        public int PlayerId { get; set; }
        public string PlayerIdentifier { get; set; }
        public string ProgressData { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime StartedOn { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            var index = 0;
            PlayerTutorialId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            PlayerId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            PlayerIdentifier = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            ProgressData = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            IsCompleted = reader.GetValue(index) != DBNull.Value && reader.GetBoolean(index); index++;
            StartedOn = reader.GetValue(index) == DBNull.Value ? DateTime.MinValue : reader.GetDateTime(index); index++;
        }
    }
}

using System;
using System.Data;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models.Chat
{
    public class BlockedPlayerTable : IBaseTable
    {
        public int PlayerId { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            var index = 0;
            PlayerId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index);
        }
    }
}

using GameOfRevenge.Common.Models.Table;
using Newtonsoft.Json;
using System;
using System.Data;

namespace GameOfRevenge.Common.Models.Kingdom
{
    public class BattleHistory : BaseTable, IBaseTable
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public bool IsAttacker { get; set; }
        public string Replay { get; set; }

        public override void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            Id = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            PlayerId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            IsAttacker = reader.GetValue(index) == DBNull.Value ? false : reader.GetBoolean(index); index++;
            Replay = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
        }
    }
}

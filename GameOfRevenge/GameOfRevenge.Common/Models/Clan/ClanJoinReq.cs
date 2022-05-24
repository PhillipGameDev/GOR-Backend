using GameOfRevenge.Common.Models.Table;
using System;
using System.Data;

namespace GameOfRevenge.Common.Models.Clan
{
    public class ClanJoinReq : IBaseTable
    {
        public int Id { get; set; }
        public int ClanId { get; set; }
        public int PlayerId { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            Id = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            ClanId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            PlayerId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index);
        }
    }
}

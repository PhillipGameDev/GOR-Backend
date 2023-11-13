using GameOfRevenge.Common.Models.Table;
using System;
using System.Data;

namespace GameOfRevenge.Common.Models.Clan
{
    public class ClanUnion : IBaseTable
    {
        public int Id { get; set; }
        public int FromClanId { get; set; }
        public int ToClanId { get; set; }
        public bool Accepted { get; set; }
        public ClanData ClanData { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            Id = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            FromClanId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            ToClanId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Accepted = reader.GetValue(index) == DBNull.Value ? false : reader.GetBoolean(index); index++;
            
            ClanData = new ClanData();
            ClanData.LoadFromDataReader(reader, index);
        }
    }
}

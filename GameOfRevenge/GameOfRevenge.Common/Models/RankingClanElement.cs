using System;
using System.Data;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models
{
    public interface IReadOnlyRankingClanElement
    {
        int ClanId { get; }
        string ClanName { get; }
        string LeaderName { get; }
        int[] Ranks { get; }
        long[] Values { get; }
    }

    public class RankingClanElement : IBaseTable, IReadOnlyRankingClanElement
    {
        public int ClanId { get; set; }
        public string ClanName { get; set; }
        public string LeaderName { get; set; }
        public int[] Ranks { get; set; }
        public long[] Values { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            ClanId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            ClanName = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            LeaderName = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;

            Ranks = new int[3];
            Values = new long[3];
            for (int idx = 0; idx < 3; idx++)
            {
                Ranks[idx] = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
                Values[idx] = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt64(index); index++;
            }
        }
    }
}

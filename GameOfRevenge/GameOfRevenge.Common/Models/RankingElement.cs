using System;
using System.Data;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models
{
    public interface IReadOnlyRankingElement
    {
        int PlayerId { get; }
        string PlayerName { get; }
        int[] Ranks { get; }
        long[] Values { get; }
    }

    public class RankingElement : IBaseTable, IReadOnlyRankingElement
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int[] Ranks { get; set; }
        public long[] Values { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            PlayerId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            PlayerName = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;

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

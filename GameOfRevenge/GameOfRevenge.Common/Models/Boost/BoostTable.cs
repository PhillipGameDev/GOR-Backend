using System;
using System.Data;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models.Boost
{
    public interface IReadOnlyBoostTable
    {
        int BoostId { get; }
        int BoostTypeId { get; }
        int Percentage { get; }
    }

    public class BoostTable : IBaseTable, IReadOnlyBoostTable
    {
        public int BoostId { get; set; }
        public int BoostTypeId { get; set; }
        public int Percentage { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            BoostId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            BoostTypeId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Percentage = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index);
        }
    }
}

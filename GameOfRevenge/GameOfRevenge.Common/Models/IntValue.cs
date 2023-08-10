using System;
using System.Data;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models
{
    public class IntValue : IBaseTable
    {
        public int Value { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            Value = reader.GetValue(0) == DBNull.Value ? 0 : reader.GetInt32(0);
        }
    }
}

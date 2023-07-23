using System;
using System.Data;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models
{
    public class ActiveUsersTable : BaseTable
    {
        public int WithinOneMonth { get; set; }
        public int WithinThreeMonths { get; set; }
        public int WithinSixMonths { get; set; }

        public override void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            WithinOneMonth = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            WithinThreeMonths = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            WithinSixMonths = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index);
        }
    }
}

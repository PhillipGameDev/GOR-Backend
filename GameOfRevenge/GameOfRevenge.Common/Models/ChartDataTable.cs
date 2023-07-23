using System;
using System.Data;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models
{
    public class ChartDataTable : BaseTable
    {
        public string NewUsers { get; set; }
        public string Recurring { get; set; }

        public override void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            NewUsers = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            Recurring = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index);
        }
    }
}

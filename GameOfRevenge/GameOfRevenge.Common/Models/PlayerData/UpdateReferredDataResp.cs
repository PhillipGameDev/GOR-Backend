using System;
using System.Data;
using GameOfRevenge.Common.Helper;
using GameOfRevenge.Common.Interface.Model.Table;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models
{

    public class UpdateReferredDataResp : IBaseTable
    {
        public int ADD_REWARD { get; set; }
        public int ReferredPlayerId { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            ADD_REWARD = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            ReferredPlayerId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
        }
    }
}

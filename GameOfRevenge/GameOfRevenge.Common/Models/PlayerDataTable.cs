using System;
using System.Data;
using GameOfRevenge.Common.Interface.Model.Table;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models
{
    public class PlayerDataTable : BaseDataTypeTable<int, string>, IBaseUserDataTable<int, string>
    {
        public long Id { get; set; }
        public int PlayerId { get; set; }

        public override void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            Id = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt64(index); index++;
            PlayerId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            DataType = reader.GetValue(index) == DBNull.Value ? DataType.Other : (DataType)Enum.Parse(typeof(DataType), reader.GetString(index)); index++;
            ValueId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Value = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index);
        }
    }
}

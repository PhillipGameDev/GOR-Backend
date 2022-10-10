using GameOfRevenge.Common.Interface.Model.Table;
using GameOfRevenge.Common.Models.Table;
using System;
using System.Data;

namespace GameOfRevenge.Common.Models.Technology
{
    public interface IReadOnlyBoostDataTable// : IReadOnlyBaseRefEnumLevelDataTable
    {
//        int DataId { get; }
//        int Id { get; }
//        int Level { get; }
        int Id { get; }
        byte Level { get; }
        int TimeTaken { get; }
    }

    public class BoostDataTable : BaseTable, IReadOnlyBoostDataTable//BaseRefEnumLevelDataTable,IBaseRefEnumLevelDataTable
    {
        public int Id { get; set; }
        public byte Level { get; set; }
        public int TimeTaken { get; set; }

        public override void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
//            DataId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Id = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Level = reader.GetValue(index) == DBNull.Value ? (byte)0 : reader.GetByte(index); index++;
//            Value = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            TimeTaken = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); 
        }
    }
}

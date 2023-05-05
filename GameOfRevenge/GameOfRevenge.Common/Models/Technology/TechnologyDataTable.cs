using GameOfRevenge.Common.Interface.Model.Table;
using GameOfRevenge.Common.Models.Table;
using System;
using System.Data;

namespace GameOfRevenge.Common.Models.Technology
{
    public interface IReadOnlySubTechnologyDataTable : IReadOnlyBaseRefEnumLevelDataTable
    {
        int Value { get; }
        int TimeTaken { get; }
    }

    public class SubTechnologyDataTable : BaseRefEnumLevelDataTable, IBaseTable, IBaseRefEnumLevelDataTable, IReadOnlyBaseRefEnumLevelDataTable, IReadOnlySubTechnologyDataTable
    {
        public int Value { get; set; }
        public int TimeTaken { get; set; }
        public override void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            DataId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            InfoId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Level = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Value = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            TimeTaken = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index);
        }
    }




    public interface IReadOnlyTechnologyDataTable : IReadOnlyBaseRefEnumLevelDataTable
    {
        int Value { get; }
        int TimeTaken { get; }
    }

    public class TechnologyDataTable : BaseRefEnumLevelDataTable, IBaseTable, IBaseRefEnumLevelDataTable, IReadOnlyBaseRefEnumLevelDataTable, IReadOnlyTechnologyDataTable
    {
        public int Value { get; set; }
        public int TimeTaken { get; set; }
        public override void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            DataId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            InfoId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Level = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Value = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            TimeTaken = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); 
        }
    }
}

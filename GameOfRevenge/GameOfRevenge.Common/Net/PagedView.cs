using GameOfRevenge.Common.Models.Table;
using System;
using System.Data;

namespace GameOfRevenge.Common.Net
{
    public class PagedView : IPagedView, IBaseTable
    {
        public int CurrentPage { get; set; }
        public int CountPerPage { get; set; }

        public int MaxRows { get; protected set; }
        public int MaxPages { get; protected set; }

        public int StartIndex { get; protected set; }
        public int EndIndex { get; protected set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            MaxPages = reader.GetValue(index) == DBNull.Value ? index : reader.GetInt32(index); index++;
            MaxRows = reader.GetValue(index) == DBNull.Value ? index : reader.GetInt32(index); index++;
            CurrentPage = reader.GetValue(index) == DBNull.Value ? index : reader.GetInt32(index); index++;
            CountPerPage = reader.GetValue(index) == DBNull.Value ? index : reader.GetInt32(index); index++;
            StartIndex = reader.GetValue(index) == DBNull.Value ? index : reader.GetInt32(index); index++;
            EndIndex = reader.GetValue(index) == DBNull.Value ? index : reader.GetInt32(index);
        }
    }
}

using System;
using System.Data;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models
{
    public class ByteValue : IBaseTable
    {
        public byte Value { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            Value = reader.GetValue(0) == DBNull.Value ? (byte)0 : reader.GetByte(0);
        }
    }
}

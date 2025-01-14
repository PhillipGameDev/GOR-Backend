﻿using GameOfRevenge.Common.Helper;
using GameOfRevenge.Common.Models.Table;
using System;
using System.Data;

namespace GameOfRevenge.Common.Models.Boost
{
    public interface IReadOnlyBoostTypeTable
    {
        int BoostTypeId { get; }
        string Name { get; }
        NewBoostType BoostType { get; }
//        string Description { get; }
    }

    [Serializable]
    public class BoostTypeTable : IBaseTable, IReadOnlyBoostTypeTable
    {
        public int BoostTypeId { get; set; }
        public string Name { get; set; }
        public NewBoostType BoostType { get; set; }
//        public string Description { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            BoostTypeId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Name = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            if (reader.GetValue(index) != DBNull.Value)
            {
                string code = reader.GetString(index);// index++;
                BoostType = code.ToEnum<NewBoostType>();
            }
            else
            {
                BoostType = NewBoostType.Unknown;
            }
//            Description = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index);
        }
    }
}

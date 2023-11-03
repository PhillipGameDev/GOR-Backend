using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using GameOfRevenge.Common.Helper;
using GameOfRevenge.Common.Interface.Model.Table;
using GameOfRevenge.Common.Models.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GameOfRevenge.Common.Models.Inventory
{
    public interface IReadOnlyInventoryUserDataTable
    {
        int Id { get; }// PK
        int InventoryId { get; }
        int Level { get; }
        int Order { get; }
    }

    [Serializable, DataContract]
    public class InventoryUserDataTable : TimerBase, IBaseTable, IReadOnlyInventoryUserDataTable
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int InventoryId { get; set; }
        [DataMember]
        public int Level { get; set; }
        [DataMember]
        public int Order { get; set; }


        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            Id = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            InventoryId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Level = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Order = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            StartTime = reader.GetValue(index) == DBNull.Value ? DateTime.MinValue : reader.GetDateTime(index); index++;
            Duration = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
        }
    }
}

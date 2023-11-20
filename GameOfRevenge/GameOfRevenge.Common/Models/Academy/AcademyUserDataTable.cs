using System;
using System.Data;
using System.Runtime.Serialization;
using GameOfRevenge.Common.Helper;
using GameOfRevenge.Common.Interface.Model.Table;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models.Academy
{
    public interface IReadOnlyAcademyUserDataTable
    {
        int Id { get; } // PK
        int ItemId { get; }
        int Level { get; }
    }

    [Serializable, DataContract]
    public class AcademyUserDataTable : TimerBase, IBaseTable, IReadOnlyAcademyUserDataTable
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int ItemId { get; set; }
        [DataMember]
        public int Level { get; set; }

        public int CurrentLevel => TimeLeft > 0 ? Level - 1 : Level;

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            Id = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            ItemId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Level = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            StartTime = reader.GetValue(index) == DBNull.Value ? DateTime.MinValue : reader.GetDateTime(index); index++;
            Duration = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
        }
    }
}

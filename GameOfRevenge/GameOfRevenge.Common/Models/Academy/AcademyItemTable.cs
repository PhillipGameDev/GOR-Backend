using System;
using System.Data;
using GameOfRevenge.Common.Helper;
using GameOfRevenge.Common.Interface.Model.Table;
using GameOfRevenge.Common.Models.Table;
using Newtonsoft.Json;

namespace GameOfRevenge.Common.Models.Academy
{
    public interface IReadOnlyAcademyItemTable
    {
        int Id { get; } // PK
        AcademyCategoryType CategoryType { get; }
        string Name { get; }
        AcademyTechnologyType TechnologyType { get; }
        int MaxLevel { get; }
        int Duration { get; }
        int Multiple { get; }
        int Effect { get; }
        int AcademyLevel { get; }
        int[] ParentItems { get; }
    }

    public class AcademyItemTable : IBaseTable, IReadOnlyAcademyItemTable
    {
        public int Id { get; set; }
        public AcademyCategoryType CategoryType { get; set; }
        public string Name { get; set; }
        public AcademyTechnologyType TechnologyType { get; set; }
        public int MaxLevel { get; set; }
        public int Duration { get; set; }
        public int Multiple { get; set; }
        public int Effect { get; set; }
        public int AcademyLevel { get; set; }
        public int[] ParentItems { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            Id = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            CategoryType = reader.GetValue(index) == DBNull.Value ? AcademyCategoryType.Unknown : (AcademyCategoryType)reader.GetInt32(index); index++;
            Name = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            TechnologyType = reader.GetValue(index) == DBNull.Value ? AcademyTechnologyType.Unknown : (AcademyTechnologyType)reader.GetInt32(index); index++;
            MaxLevel = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Duration = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Multiple = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Effect = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            AcademyLevel = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            ParentItems = reader.GetValue(index) == DBNull.Value ? new int[] { } : JsonConvert.DeserializeObject<int[]>(reader.GetString(index)); index++;
        }
    }
}

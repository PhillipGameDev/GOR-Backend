using System;
using System.Data;
using GameOfRevenge.Common.Helper;
using GameOfRevenge.Common.Interface.Model.Table;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models.Academy
{
    public interface IReadOnlyAcademyRequirementTable
    {
        int Id { get; } // PK
        int ItemId { get; }
        ResourceType ResourceType { get; }
        int InitValue { get; }
        int Key { get; }
        AlgorithmType AlgorithmType { get; }
    }

    public class AcademyRequirementTable : IBaseTable, IReadOnlyAcademyRequirementTable
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public ResourceType ResourceType { get; set; }
        public int InitValue { get; set; }
        public int Key { get; set; }
        public AlgorithmType AlgorithmType { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            Id = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            ItemId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            ResourceType = reader.GetValue(index) == DBNull.Value ? ResourceType.Other : (ResourceType)reader.GetInt32(index); index++;
            InitValue = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Key = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            AlgorithmType = reader.GetValue(index) == DBNull.Value ? AlgorithmType.Unknown : (AlgorithmType)reader.GetInt32(index); index++;
        }
    }
}

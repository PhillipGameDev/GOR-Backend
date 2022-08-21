using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Common.Interface
{
    public interface IStructureManager
    {
        //Task<Response<StructureTable>> GetStructure(int id);
        //Task<Response<StructureTable>> GetStructure(string code);
        //Task<Response<StructureTable>> GetStructure(StructureType code);

        //Task<Response<StructureDataTable>> AddStructureLevelData(int id, int targetLvl, int hp, int foodp, int woodp, int orep, int popSupport, int bldgSupport);
        //Task<Response<List<StructureDataTable>>> GetStructureLevelData(int id);
        //Task<Response<StructureDataTable>> GetStructureLevelData(int id, int level);
        //Task<Response<StructureDataTable>> GetStructureLevelDataById(int id);
        //Task<Response> RemoveStructureLevelData(int id);
        //Task<Response> RemoveStructureLevelData(string code, int level);
        //Task<Response> RemoveStructureLevelData(StructureType code, int level);
        //Task<Response> RemoveStructureLevelData(int structureId, int level);
        //Task<Response<List<StructureDataTable>>> GetStructureLevelData(string code);
        //Task<Response<StructureDataTable>> GetStructureLevelData(string code, int level);

        //Task<Response<DataRequirement>> AddStructureLevelRequirement(int id, DataType data, int dataId, int value);
        //Task<Response> GetStructureLevelRequirementById(int id);
        //Task<Response> GetStructureLevelRequirements(int id);
        //Task<Response> GetStructureLevelRequirements(int id, int level);
        //Task<Response> GetStructureLevelRequirementsByDataId(int id);
        //Task<Response> RemoveStructureLevelRequirement(int id);
        Task<Response<List<StructureTable>>> GetAllStructures();
        Task<Response<List<StructureDataTable>>> GetAllStructureLevelDatas();
        Task<Response<List<DataRequirement>>> GetAllStructureLevelRequirements();
        Task<Response<List<StructureDataRequirementRel>>> GetAllStructDataRequirementRel();
        Task<Response<List<StructureLocationRelTable>>> GetAllStructureLocation();
        Task<Response<List<StructureBuildLimitTable>>> GetAllStructureBuildLimit();
    }
}

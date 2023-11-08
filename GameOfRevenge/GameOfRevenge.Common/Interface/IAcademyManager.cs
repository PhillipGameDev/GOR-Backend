using GameOfRevenge.Common.Models.Academy;
using GameOfRevenge.Common.Models.Inventory;
using GameOfRevenge.Common.Net;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameOfRevenge.Common.Interface
{
    public interface IAcademyManager
    {
        Task<Response<List<AcademyItemTable>>> GetAllAcademyItem();
        Task<Response<List<AcademyRequirementTable>>> GetAllAcademyRequirement();
    }
}

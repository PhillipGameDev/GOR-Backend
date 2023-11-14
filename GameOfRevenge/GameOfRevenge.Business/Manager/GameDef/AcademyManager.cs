using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Services;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Models.Academy;
using System.Linq;

namespace GameOfRevenge.Business.Manager.GameDef
{
    public class AcademyManager : BaseManager, IAcademyManager
    {
        public async Task<Response<List<AcademyItemTable>>> GetAllAcademyItem() { 

            try
            {
                return await Db.ExecuteSPMultipleRow<AcademyItemTable>("GetAllAcademyItem");
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<AcademyItemTable>>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<AcademyItemTable>>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<List<AcademyRequirementTable>>> GetAllAcademyRequirement()
        {
            try
            {
                return await Db.ExecuteSPMultipleRow<AcademyRequirementTable>("GetAllAcademyRequirement");
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<AcademyRequirementTable>>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<AcademyRequirementTable>>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }
    }
}

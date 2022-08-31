using System;
using System.Linq;
using System.Threading.Tasks;
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Services;
using Newtonsoft.Json;

namespace GameOfRevenge.Business.Manager.UserData
{
    public class UserTechnologyManager : BaseUserDataManager, IUserTechnologyManager
    {
        private readonly IUserResourceManager userResourceManager;

        public UserTechnologyManager()
        {
            userResourceManager = new UserResourceManager();
        }


        public async Task<Response<SubTechnologyInfos>> UpgradeSubTechnology(int playerId, SubTechnologyType type)
        {
            try
            {
                var timestamp = Config.UtcTime;
                var tech = CacheTechnologyDataManager.GetFullSubTechnologyData(type);
                var compPlayerData = await GetFullPlayerData(playerId);
                if (!compPlayerData.IsSuccess || !compPlayerData.HasData) throw new DataNotExistExecption(compPlayerData.Message);

                var currentUserTech = compPlayerData.Data.SubTechnologies.FirstOrDefault(x => x.SubTechnologyType == type);
                if (currentUserTech == null) currentUserTech = new SubTechnologyInfos()
                {
                    SubTechnologyType = type,
                    Level = 0
                };

                if (currentUserTech.TimeLeft > 0) throw new RequirementExecption("Technology is already upgrading");
                var targetLvl = currentUserTech.Level + 1;
                var techData = CacheTechnologyDataManager.GetFullSubTechnologyLevelData(type, targetLvl);
                var hasResource = HasRequirements(techData.Requirements, compPlayerData.Data.Structures, compPlayerData.Data.Resources);
                if (hasResource) await userResourceManager.RemoveResourceByRequirement(playerId, techData.Requirements);
                else throw new RequirementExecption("Requirement not meet");

                currentUserTech.Level = targetLvl;
                currentUserTech.StartTime = timestamp;
                currentUserTech.EndTime = timestamp.AddSeconds(techData.Data.TimeTaken);

                var response = await manager.AddOrUpdatePlayerData(playerId, DataType.SubTechnology, tech.Info.Id, JsonConvert.SerializeObject(currentUserTech));
                var data = PlayerDataToUserSubTechnologyData(response.Data)?.Value;
                return new Response<SubTechnologyInfos>(data, response.Case, response.Message);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<SubTechnologyInfos>()
                {
                    Case = 200,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<SubTechnologyInfos>()
                {
                    Case = 201,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (RequirementExecption ex)
            {
                return new Response<SubTechnologyInfos>()
                {
                    Case = 202,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (CacheDataNotExistExecption ex)
            {
                return new Response<SubTechnologyInfos>()
                {
                    Case = 203,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (Exception)
            {
                return new Response<SubTechnologyInfos>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError()
                };
            }
        }

        public async Task<Response<SubTechnologyInfos>> UpgradeSubTechnology(int playerId, int id)
        {
            try
            {
                var tech = CacheTechnologyDataManager.GetFullSubTechnologyData(id);
                return await UpgradeSubTechnology(playerId, tech.Info.Code);
            }
            catch (CacheDataNotExistExecption ex)
            {
                return new Response<SubTechnologyInfos>()
                {
                    Case = 203,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (Exception)
            {
                return new Response<SubTechnologyInfos>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError()
                };
            }
        }

        public async Task<Response<TechnologyInfos>> UpgradeTechnology(int playerId, TechnologyType type)
        {
            try
            {
                var timestamp = Config.UtcTime;
                var tech = CacheTechnologyDataManager.GetFullTechnologyData(type);
                var compPlayerData = await GetFullPlayerData(playerId);
                if (!compPlayerData.IsSuccess || !compPlayerData.HasData) throw new DataNotExistExecption(compPlayerData.Message);

                var currentUserTech = compPlayerData.Data.Technologies.FirstOrDefault(x => x.TechnologyType == type);
                if (currentUserTech == null) currentUserTech = new TechnologyInfos()
                {
                    TechnologyType = type,
                    Level = 0
                };

                if (currentUserTech.TimeLeft > 0) throw new RequirementExecption("Technology is already upgrading");
                var targetLvl = currentUserTech.Level + 1;
                var techData = CacheTechnologyDataManager.GetFullTechnologyLevelData(type, targetLvl);
                var hasResource = HasRequirements(techData.Requirements, compPlayerData.Data.Structures, compPlayerData.Data.Resources);
                if (hasResource) await userResourceManager.RemoveResourceByRequirement(playerId, techData.Requirements);
                else throw new RequirementExecption("Requirement not meet");
                
                currentUserTech.Level = targetLvl;
                currentUserTech.StartTime = timestamp;
                currentUserTech.EndTime = timestamp.AddSeconds(techData.Data.TimeTaken);

                var response = await manager.AddOrUpdatePlayerData(playerId, DataType.Technology, tech.Info.Id, JsonConvert.SerializeObject(currentUserTech));
                var data = PlayerDataToUserTechnologyData(response.Data)?.Value;
                return new Response<TechnologyInfos>(data, response.Case, response.Message);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<TechnologyInfos>()
                {
                    Case = 200,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<TechnologyInfos>()
                {
                    Case = 201,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (RequirementExecption ex)
            {
                return new Response<TechnologyInfos>()
                {
                    Case = 202,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (CacheDataNotExistExecption ex)
            {
                return new Response<TechnologyInfos>()
                {
                    Case = 203,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (Exception)
            {
                return new Response<TechnologyInfos>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError()
                };
            }
        }

        public async Task<Response<TechnologyInfos>> UpgradeTechnology(int playerId, int id)
        {
            try
            {
                var tech = CacheTechnologyDataManager.GetFullTechnologyData(id);
                return await UpgradeTechnology(playerId, tech.Info.Code);
            }
            catch (CacheDataNotExistExecption ex)
            {
                return new Response<TechnologyInfos>()
                {
                    Case = 203,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (Exception)
            {
                return new Response<TechnologyInfos>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError()
                };
            }
        }
    }
}

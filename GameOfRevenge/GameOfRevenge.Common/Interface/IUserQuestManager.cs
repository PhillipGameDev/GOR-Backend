using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Quest;
using GameOfRevenge.Common.Models.Quest.Template;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Common.Interface
{
    public interface IUserQuestManager
    {
        Task<Response<List<PlayerRewardDataTable>>> GetUserAllRewards(int playerId);
        Task<Response<PlayerDataTableUpdated>> ConsumeReward(int playerId, long playerDataId, string context = null);

        Task CheckQuestProgressForCollectResourceAsync(PlayerUserQuestData playerData, ResourceType resourceType, int count);
        Task CheckQuestProgressForTrainTroops(PlayerUserQuestData playerData, TroopType troopType, int level, int count);
        Task CheckQuestProgressForGroupTechnologyAsync(PlayerUserQuestData playerData, GroupTechnologyType groupTechnologyType);

        Task<Response<List<PlayerQuestDataTable>>> GetAllQuestProgress(int playerId);
        Task<Response<UserChapterAllQuestProgress>> GetUserAllQuestProgress(int playerId, bool fullTree = false);
        Task<Response<(List<UserChapterQuestData>, List<PlayerQuestDataTable>)>> GetUserAllChapterAndQuestProgress(int playerId, bool fullTree);
        Task<Response<PlayerQuestDataTable>> GetQuestProgress(int playerId, int questId);

        Task<Response<PlayerQuestDataTable>> UpdateQuestData(int playerId, int questId, bool isCompleted, string progress = null);
        Task<Response<PlayerQuestDataTable>> UpdateQuestData(int playerId, int questId, bool isCompleted, object progress);
//        Task<Response<PlayerQuestDataTable>> UpdateQuestData<T>(int playerId, int questId, bool isCompleted, T progress) where T : IBaseQuestTemplateData;

        Task<Response> RedeemQuestReward(int playerId, int questId);
        Task<Response> RedeemChapterReward(int playerId, int chapterId);
    }
}

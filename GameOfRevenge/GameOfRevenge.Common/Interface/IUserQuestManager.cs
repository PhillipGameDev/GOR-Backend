using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common.Models.Quest;
using GameOfRevenge.Common.Models.Quest.Template;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Common.Interface
{
    public interface IUserQuestManager
    {
        Task<Response<List<PlayerQuestDataTable>>> GetAllQuestProgress(int playerId);
        Task<Response<UserChapterAllQuestProgress>> GetUserAllQuestProgress(int playerId, bool fullTree = false);
        Task<Response<List<UserChapterQuestData>>> GetUserAllChapterQuestProgress(int playerId, bool fullTree);
        Task<Response<PlayerQuestDataTable>> GetQuestProgress(int playerId, int questId);

        Task<Response<PlayerQuestDataTable>> UpdateQuestData(int playerId, int questId, bool isCompleted, string progress = null);
        Task<Response<PlayerQuestDataTable>> UpdateQuestData(int playerId, int questId, bool isCompleted, object progress);
        Task<Response<PlayerQuestDataTable>> UpdateQuestData<T>(int playerId, int questId, bool isCompleted, T progress) where T : IBaseQuestTemplateData;

        Task<Response> RedeemQuestReward(int playerId, int questId);
        Task<Response> RedeemChapterReward(int playerId, int chapterId);
    }
}

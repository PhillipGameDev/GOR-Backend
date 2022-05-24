using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common.Models.Quest;
using GameOfRevenge.Common.Models.Quest.Template;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Common.Interface
{
    public interface IUserQuestManager
    {
        //Task<Response<List<PlayerQuestData>>> GetAllQuestProgress(int playerId);
        Task<Response<List<UserChapterQuestData>>> GetAllQuestChapterDataWithName(int playerId);
        Task<Response<PlayerQuestData>> GetQuestProgress(int playerId, int questId);

        Task<Response<PlayerQuestData>> UpdateQuestData(int playerId, int questId, bool isCompleted, string progress);
        Task<Response<PlayerQuestData>> UpdateQuestData(int playerId, int questId, bool isCompleted, object progress);
        Task<Response<PlayerQuestData>> UpdateQuestData<T>(int playerId, int questId, bool isCompleted, T progress) where T : IBaseQuestTemplateData;

        Task<Response> RedeemQuestReward(int playerId, int questId);
        Task<Response> RedeemChapterReward(int playerId, int chapterId);
    }
}

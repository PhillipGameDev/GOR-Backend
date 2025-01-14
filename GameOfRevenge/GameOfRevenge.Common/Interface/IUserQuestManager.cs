﻿using System.Threading.Tasks;
using System.Collections.Generic;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Quest;

namespace GameOfRevenge.Common.Interface
{
    public interface IUserQuestManager
    {
        Task CheckQuestProgressForCollectResourceAsync(PlayerUserQuestData playerData, ResourceType resourceType, int count);
        Task CheckQuestProgressForTrainTroops(PlayerUserQuestData playerData, TroopType troopType, int level, int count);
        Task CheckQuestProgressForGroupTechnologyAsync(PlayerUserQuestData playerData, GroupTechnologyType groupTechnologyType);

        Task CheckPlayerQuestDataAsync(PlayerUserQuestData data);

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

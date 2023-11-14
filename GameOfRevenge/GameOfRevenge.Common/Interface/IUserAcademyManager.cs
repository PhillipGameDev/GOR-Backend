using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Academy;
using GameOfRevenge.Common.Models.Quest;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Common.Interface
{
    public interface IUserAcademyManager
    {
        Task<Response<List<AcademyUserDataTable>>> GetUserAllItems(int playerId);
        Task<Response<AcademyUserDataTable>> UpgradeItem(int playerId, int itemId, DateTime startTime);
        Task<Response<AcademyUserDataTable>> InstantUpgradeItem(int playerId, int itemId);
        Task<Response<AcademyUserDataTable>> SpeedUpUpgradeItem(int playerId, int itemId, int duration);
    }
}

using System.Collections.Generic;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.GameHandlers;
using GameOfRevenge.Common.Models.Structure;

namespace GameOfRevenge.Interface
{
    public interface IGameServices
    {
        IRealTimeUpdateManager BRealTimeUpdateManager { get; }
        IUserResourceManager BPlayerResourceManager { get; }
        Dictionary<StructureType, IGameBuildingManager> GameBuildingManager { get; }
        IUserStructureManager BPlayerStructureManager { get; }
        IUserTechnologyManager BUserTechnologyManager { get; }
        GameLobbyHandler GameLobby { get; }
        IWorldHandler WorldHandler { get; }
        IPlayerDataManager BPlayerManager { get; }
        IKingdomManager BKingdomManager { get; }
        IKingdomPvPManager BkingdomePvpManager { get; }
        IUserTroopManager BUsertroopManager { get; }
        ITechnologyManager BTechnologyManager { get; }
    }
}

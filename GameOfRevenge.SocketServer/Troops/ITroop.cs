using GameOfRevenge.Common;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Model;

namespace GameOfRevenge.Troops
{
    public interface ITroop  //Barracks, Shooting Range, Training Heroes, Infantry Camp, Workshop and Stable
    {
        IGameTroop GameTroop { get; }
        TroopType TroopType { get; }
        double TimeLeft { get; }
        bool IsConstructing { get; }

        void TrainingStart(RecruitTroopRequest request);
        void Init(UserTroopData troopData);
        void SendTroopTrainingTimeToClient();
        void TrainingTimeBoostUp();
    }
}

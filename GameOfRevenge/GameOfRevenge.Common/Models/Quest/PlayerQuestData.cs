using System;
using System.Data;
using System.Runtime.Serialization;
using GameOfRevenge.Common.Models.Quest.Template;
using GameOfRevenge.Common.Models.Table;
using Newtonsoft.Json;

namespace GameOfRevenge.Common.Models.Quest
{
    public interface IReadOnlyUserQuestProgressData
    {
//        public QuestType QuestType => QuestInfo.QuestType;
//        public string InitialData => QuestInfo.DataString;

        IReadOnlyQuestTable QuestInfo { get; }
        PlayerQuestDataTable UserData { get; }

        string Description { get; }
    }

    public struct UserQuestProgressData : IReadOnlyUserQuestProgressData
    {
        public int QuestUserDataId => (UserData != null)? UserData.QuestUserDataId : 0;
        public int QuestId => QuestInfo.QuestId;
        public bool Completed => (UserData != null)? UserData.Completed : false;
        public bool Redeemed => (UserData != null)? UserData.Redeemed : false;
        public string ProgressData => UserData?.ProgressData;

        public QuestType QuestType => QuestInfo.QuestType;
        public string InitialData => QuestInfo.DataString;

        public IReadOnlyQuestTable QuestInfo { get; set; }
        public PlayerQuestDataTable UserData { get; set; }
//        public int QuestGroup { get; set; }

        public string Description => PlayerQuestDataTable.GetDescription(QuestType, Completed, InitialData, ProgressData);

        public UserQuestProgressData(IReadOnlyQuestTable questData, PlayerQuestDataTable userQuestData)
        {
            QuestInfo = questData;
            UserData = userQuestData;
        }
    }

    public interface IReadOnlyPlayerQuestData
    {
        int QuestUserDataId { get; }
        int QuestId { get; }
//        int PlayerId { get; }

        bool Completed { get; }
        bool Redeemed { get; }
        string ProgressData { get; }
    }

    [DataContract]
    public class PlayerQuestDataTable : IBaseTable, IReadOnlyPlayerQuestData
    {
        [DataMember]
        public int QuestUserDataId { get; set; }
        [DataMember(Order = 10)]
        public int QuestId { get; set; }
//        public int PlayerId { get; set; }

        [DataMember(Order = 20)]
        public bool Completed { get; set; }
        [DataMember(Order = 21, EmitDefaultValue = false)]
        public bool Redeemed { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = false)]
        public string ProgressData { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            QuestUserDataId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            QuestId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
//            PlayerId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Completed = reader.GetValue(index) == DBNull.Value ? false : reader.GetBoolean(index); index++;
            Redeemed = reader.GetValue(index) == DBNull.Value ? false : reader.GetBoolean(index); index++;
//            ProgressData = (Completed || (reader.GetValue(index) == DBNull.Value))? null : reader.GetString(index);
            ProgressData = reader.GetValue(index) == DBNull.Value ? null : reader.GetString(index);
        }

        public static string GetDescription(QuestType questType, bool completed, string initialData, string progressData)
        {
            var name = string.Empty;

            try
            {
                switch (questType)
                {
                    case QuestType.BuildingUpgrade:
                    case QuestType.XBuildingCount:
                        var data1 = JsonConvert.DeserializeObject<QuestBuildingData>(initialData);
                        string format = null;
                        if (questType == QuestType.BuildingUpgrade)
                        {
                            format = (data1.Level == 1)? "Create building {1}" : "Upgrade {1} to level {2}";
                        }
                        else
                        {
                            format = "Have {0:n0} {1}";
                        }
                        name = string.Format(format, data1.Count, data1.StructureType, data1.Level);
                        break;
                    case QuestType.ResourceCollection:
                        var data2 = JsonConvert.DeserializeObject<QuestResourceData>(initialData);
                        string format2 = null;
                        format2 = (data2.Iteration > 0)? "Collect {1}" : "Collect {0:n0} {1}";
                        name = string.Format(format2, data2.Count, data2.ResourceType);
                        var iteration2 = 0;
                        if (progressData != null)
                        {
                            data2 = JsonConvert.DeserializeObject<QuestResourceData>(progressData);
                            iteration2 = data2.Iteration + (completed ? 1 : 0);
                        }
                        else
                        {
                            iteration2 = data2.Iteration;
                        }
                        if (iteration2 > 1) name += $" ({iteration2})";
                        break;
                    case QuestType.TrainTroops:
                    case QuestType.XTroopCount:
                        var data3 = JsonConvert.DeserializeObject<QuestTroopData>(initialData);
                        string format3 = null;
                        if (questType == QuestType.TrainTroops)
                        {
                            format3 = (data3.Level > 0) ? "Train {0:n0} {1} Lvl.{2}" : "Train {0:n0} {1}";
                        }
                        else
                        {
                            format3 = "Have {0:n0} {1}";
                        }
                        name = string.Format(format3, data3.Count, data3.TroopType, data3.Level);
                        var iteration3 = 0;
                        if (progressData != null)
                        {
                            data3 = JsonConvert.DeserializeObject<QuestTroopData>(progressData);
                            iteration3 = data3.Iteration + (completed ? 1 : 0);
                        }
                        else
                        {
                            iteration3 = data3.Iteration;
                        }
                        if (iteration3 > 1) name += $" ({iteration3})";
                        break;
                    case QuestType.Account:
                        var data4 = JsonConvert.DeserializeObject<QuestAccountData>(initialData);
                        switch (data4.AccountTaskType)
                        {
                            case AccountTaskType.SignIn: name = "Sign In 1 time"; break;
                            case AccountTaskType.ChangeName: name = "Change user name"; break;
                        }
                        break;
                    case QuestType.TrainHero:
                    case QuestType.XHeroCount:
                        var data5 = JsonConvert.DeserializeObject<QuestTroopData>(initialData);
                        if (questType == QuestType.TrainHero)
                        {
                            name = string.Format("Train {0:n0} {1} {2}", data5.Count, data5.TroopType, data5.Level);
                        }
                        else
                        {
                            name = string.Format("Have {0:n0} {1}", data5.Count, data5.TroopType);
                        }
                        break;
                }
            }
            catch { }

            return name;
        }
    }
}

﻿using System;
using System.Data;
using System.Runtime.Serialization;
using GameOfRevenge.Common.Helper;
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

//        string Description { get; }
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

//        public string Description => PlayerQuestDataTable.GetDescription(QuestType, Completed, InitialData, ProgressData);

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

        public static ILocalizationBase Localization = new Localization();

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

/*        public static string GetDescription(QuestType questType, bool completed, string initialData, string progressData)
        {
            var name = string.Empty;

            try
            {
                switch (questType)
                {
                    case QuestType.BuildingUpgrade://1
                    case QuestType.XBuildingCount://2
                        var data1 = JsonConvert.DeserializeObject<QuestBuildingData>(initialData);
                        string format = null;
                        if (questType == QuestType.BuildingUpgrade)
                        {
                            format = (data1.Level == 1)? "Create building {1}" : "Upgrade {1} to level {2}";
                        }
                        else
                        {
                            format = "Have {0:N0} {1}";
                        }
                        format = Localization.GetText(format, Localization.QUESTS);
                        var bldStr = Localization.GetText(data1.StructureType.ToString(), Localization.BUILDINGS_ID);
                        name = string.Format(format, data1.Count, bldStr, data1.Level);
                        break;
                    case QuestType.ResourceCollection://3
                        var data2 = JsonConvert.DeserializeObject<QuestResourceData>(initialData);
                        string format2 = null;
                        format2 = (data2.Iteration > 0)? "Collect {1}" : "Collect {0:N0} {1}";
                        var resStr = data2.ResourceType.ToString();
                        name = string.Format(format2, data2.Count, resStr);
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
                    case QuestType.TrainTroops://4
                    case QuestType.XTroopCount://5
                        var data3 = JsonConvert.DeserializeObject<QuestTroopData>(initialData);
                        string format3 = null;
                        if (questType == QuestType.TrainTroops)
                        {
                            format3 = (data3.Level > 0) ? "Train {0:N0} {1} {2}" : "Train {0:N0} {1}";
                        }
                        else
                        {
                            format3 = "Have {0:N0} {1}";
                        }
                        var troopStr = data3.TroopType.ToString();
                        name = string.Format(format3, data3.Count, troopStr, ("Lvl." + data3.Level));
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
                    case QuestType.TrainHero://6
                    case QuestType.XHeroCount://7
                        var data4 = JsonConvert.DeserializeObject<QuestTroopData>(initialData);
                        if (questType == QuestType.TrainHero)
                        {
                            name = string.Format("Train {0:N0} {1} {2}", data4.Count, data4.TroopType, data4.Level);
                        }
                        else
                        {
                            name = string.Format("Have {0:N0} {1}", data4.Count, data4.TroopType);
                        }
                        break;
                    case QuestType.Custom://8
                        var data5 = JsonConvert.DeserializeObject<QuestCustomData>(initialData);
                        switch (data5.CustomTaskType)
                        {
                            case CustomTaskType.SendGlobalChat: name = "Send global chat"; break;
                            case CustomTaskType.AttackPlayer: name = string.Format("Attack enemy player {0} times", data5.Count); break;
                            case CustomTaskType.ItemBoxExploring: name = "Item box exploring"; break;
                            case CustomTaskType.AttackMonster: name = data5.Level != null ? string.Format("Attack lvl {0} monster {1} times", data5.Level, data5.Count) : string.Format("Attack monster {0} time", data5.Count); break;
                            case CustomTaskType.GetHeroPoints: name = string.Format("Get {0} hero points", data5.Count); break;
                            case CustomTaskType.BuildOrUpgrade: name = string.Format("Build or upgrade {0} building", data5.Count); break;
                            case CustomTaskType.ResearchTechnology: name = string.Format("Research {0} technology", data5.Count); break;
                            case CustomTaskType.AttackOasis: name = string.Format("Attack oasis {0}", data5.Value); break;
                            case CustomTaskType.TradeMarket: name = string.Format("Trade in market {0} times", data5.Count); break;
                        }
                        break;
                    case QuestType.Account://9
                        var data6 = JsonConvert.DeserializeObject<QuestAccountData>(initialData);
                        switch (data6.AccountTaskType)
                        {
                            case AccountTaskType.SignIn: name = "Daily Sign In"; break;
                            case AccountTaskType.ChangeName: name = "Change username"; break;
                        }
                        break;
                    case QuestType.Alliance://10
                        var data7 = JsonConvert.DeserializeObject<QuestAllianceData>(initialData);
                        switch (data7.AllianceTaskType)
                        {
                            case AllianceTaskType.JoinOrCreate: name = "Join or create an alliance"; break;
                            case AllianceTaskType.Reinforce: name = "Reinforce"; break;
                            case AllianceTaskType.SendGift: name = "Send Gift"; break;
                            case AllianceTaskType.Help: name = string.Format("Help alliance army {0} times", data7.Count); break;
                        }
                        break;
                    case QuestType.ResearchTechnology://11
                        var data8 = JsonConvert.DeserializeObject<QuestGroupTechnologyData>(initialData);
                        switch (data8.GroupTechnologyType)
                        {
                            case GroupTechnologyType.KingdomTechnologies: name = "Research kingdom technology {0} times"; break;
                            case GroupTechnologyType.AttackTechnologies: name = "Research attack technology {0} times"; break;
                            case GroupTechnologyType.DefenseTechnologies: name = "Research defense technology {0} times"; break;
                        }
                        name = string.Format(name, data8.Count);
                        break;
                }
            }
            catch { }

            return name;
        }*/
    }
}

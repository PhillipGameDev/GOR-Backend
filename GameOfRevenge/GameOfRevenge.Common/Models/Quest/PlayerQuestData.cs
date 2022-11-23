using System;
using System.Data;
using GameOfRevenge.Common.Models.Quest.Template;
using GameOfRevenge.Common.Models.Table;
using Newtonsoft.Json;

namespace GameOfRevenge.Common.Models.Quest
{
    public interface IReadOnlyPlayerQuestData
    {
        int QuestUserDataId { get; }
        int QuestId { get; }
        int PlayerId { get; }
        bool Completed { get; }
        string ProgressData { get; }
        bool Redeemed { get; }
    }

    public class PlayerQuestDataTable : IBaseTable, IReadOnlyPlayerQuestData
    {
        public int QuestUserDataId { get; set; }
        public int PlayerId { get; set; }
        public int QuestId { get; set; }
        public bool Completed { get; set; }
        public string ProgressData { get; set; }
        public bool Redeemed { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            QuestUserDataId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            QuestId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            PlayerId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Completed = reader.GetValue(index) == DBNull.Value ? false : reader.GetBoolean(index); index++;
            ProgressData = reader.GetValue(index) == DBNull.Value ? null : reader.GetString(index); index++;
            Redeemed = reader.GetValue(index) == DBNull.Value ? false : reader.GetBoolean(index);
        }

        public static string GetName(QuestType questType, bool completed, string initialData, string progressData)
        {
            var name = string.Empty;

            try
            {
                switch (questType)
                {
                    case QuestType.BuildingUpgrade:
                    case QuestType.XBuildingCount:
                        var data1 = JsonConvert.DeserializeObject<QuestBuildingData>(initialData);
                        if (questType == QuestType.BuildingUpgrade)
                        {
                            if (data1.Level == 1)
                            {
                                name = string.Format("Create building {0}", data1.StructureType);
                            }
                            else
                            {
                                name = string.Format("Upgrade {0} to level {1}", data1.StructureType, data1.Level);
                            }
                        }
                        else
                        {
                            name = string.Format("Have {0:n0} {1}", data1.Count, data1.StructureType);
                        }
                        break;
                    case QuestType.ResourceCollection:
                        var data2 = JsonConvert.DeserializeObject<QuestResourceData>(initialData);
                        if (data2.Iteration > 0)
                        {
                            name = string.Format("Collect {1}", data2.Count, data2.ResourceType);
                        }
                        else
                        {
                            name = string.Format("Collect {0:n0} {1}", data2.Count, data2.ResourceType);
                        }
                        var iteration = 0;
                        if (progressData != null)
                        {
                            data2 = JsonConvert.DeserializeObject<QuestResourceData>(progressData);
                            iteration = data2.Iteration + (completed ? 1 : 0);
                        }
                        else
                        {
                            iteration = data2.Iteration;
                        }
                        if (iteration > 1) name += $" ({iteration})";
                        break;
                    case QuestType.TrainTroops:
                    case QuestType.XTroopCount:
                        var data3 = JsonConvert.DeserializeObject<QuestTroopData>(initialData);
                        if (questType == QuestType.TrainTroops)
                        {
                            name = string.Format("Train {0:n0} {1} {2}", data3.Count, data3.TroopType, data3.Level);
                        }
                        else
                        {
                            name = string.Format("Have {0:n0} {1}", data3.Count, data3.TroopType);
                        }
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

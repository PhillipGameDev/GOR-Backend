using System;
using System.Data;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using GameOfRevenge.Common.Helper;
using GameOfRevenge.Common.Models.Table;
using GameOfRevenge.Common.Models.Boost;
using GameOfRevenge.Common.Interface.Model.Table;

namespace GameOfRevenge.Common.Models
{
    public interface IReadOnlyDataReward : IBaseDataTypeTable<int, int>, IReadOnlyBaseDataTypeTable<int, int>
    {
        int RewardId { get; }
        int QuestId { get; }
        int Count { get; }
    }

    public interface IDataReward : IBaseDataTypeTable<int, int>, IReadOnlyBaseDataTypeTable<int, int>, IReadOnlyDataReward
    {
        new int RewardId { get; set; }
        new int QuestId { get; set; }
        new int Count { get; set; }
    }

    [DataContract]
    public class DataReward : BaseDataTypeTable<int, int>, IBaseTable, IBaseDataTypeTable<int, int>, IReadOnlyBaseDataTypeTable<int, int>, IDataReward, IReadOnlyDataReward
    {
        [DataMember(EmitDefaultValue = false)]
        public int RewardId { get; set; }
        public int QuestId { get; set; }
        [DataMember, JsonProperty(Order = 10)]
        public int Count { get; set; }

        public static ILocalizationBase Localization = new Localization();

        public override void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            RewardId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            QuestId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            DataType = reader.GetValue(index) == DBNull.Value ? DataType.Unknown : (DataType)reader.GetInt32(index); index++;
            ValueId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Value = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Count = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index);
        }

        public DataReward()
        {
        }

        public DataReward(int rewardId, int questId, DataType dataType, int valueId, int value, int count)
        {
            RewardId = rewardId;
            QuestId = questId;
            DataType = dataType;
            ValueId = valueId;
            Value = value;
            Count = count;
        }

        public override string ToString()
        {
            return DataType.ToString() + ": " + ValueId + ": " + Value + "x " + Count;
        }

        public (string, string, string) GetProperties() => GetProperties(this);

        public static (string, string, string) GetProperties(DataReward reward)
        {
            string value;
            if (reward.DataType == DataType.Technology || (reward.DataType == DataType.Custom && (CustomType)reward.ValueId == CustomType.VIPActivate))
            {
                if ((NewBoostTech)reward.ValueId != NewBoostTech.TroopMarchingReductionMultiplier)
                {
                    value = TimeHelper.ChangeSecondsToTimeFormat(reward.Value, false);
                }
                else
                {
                    value = reward.Value.ToString();
                }
            }
            else
            {
                value = (reward.Value >= 1000)? AmountHelper.ToKMB(reward.Value) : string.Format("{0:N0}", reward.Value);
            }

            string str;
            string title = null;
            string desc = null;
            switch (reward.DataType)
            {
                case DataType.Custom:
                    switch ((CustomType)reward.ValueId)
                    {
                        case CustomType.KingExperiencePoints:
                            str = Localization.GetText("{0} Experience Points for your King", Helper.Localization.REWARDS);
                            title = string.Format(str, value);
                            str = Localization.GetText("Use this item to add {0:N0} Experience Points to your King", Helper.Localization.REWARDS);
                            desc = string.Format(str, reward.Value);
                            break;
                        case CustomType.KingStaminaPoints:
                            str = Localization.GetText("{0} Stamina Points for your King", Helper.Localization.REWARDS);
                            title = string.Format(str, value);
                            str = Localization.GetText("Use this item to add {0:N0} Stamina Points to your King", Helper.Localization.REWARDS);
                            desc = string.Format(str, reward.Value);
                            break;
                        case CustomType.VIPPoints:
                            str = Localization.GetText("{0} VIP Points", Helper.Localization.REWARDS);
                            title = string.Format(str, value);
                            str = Localization.GetText("Use this item to receive {0:N0} VIP Points", Helper.Localization.REWARDS);
                            desc = string.Format(str, reward.Value);
                            break;
                        case CustomType.HeroPoints:
                            str = Localization.GetText("{0} Hero Points", Helper.Localization.REWARDS);
                            title = string.Format(str, value);
                            str = Localization.GetText("Use this item to receive {0:N0} Hero Points", Helper.Localization.REWARDS);
                            desc = string.Format(str, reward.Value);
                            break;
                        case CustomType.VIPActivate:
                            str = Localization.GetText("{0} VIP Activate", Helper.Localization.REWARDS);
                            title = string.Format(str, TimeHelper.ChangeSecondsToFormatTimeWords(reward.Value, camelcase: true));
                            str = Localization.GetText("Activate VIP for {0}", Helper.Localization.REWARDS);
                            desc = string.Format(str, TimeHelper.ChangeSecondsToFormatTimeWords(reward.Value));
                            break;
                    }
                    break;
                case DataType.Resource:
                    var type = (ResourceType)reward.ValueId;
                    var resName = Localization.GetText(type.ToString(), Helper.Localization.ENUMS);
                    str = Localization.GetText("{0} {1}", Helper.Localization.REWARDS);
                    title = string.Format(str, value, resName);
                    str = Localization.GetText("Use it to receive {0:N0} {1}", Helper.Localization.REWARDS);
                    desc = string.Format(str, reward.Value, resName);
                    break;
                case DataType.Technology:
                    string str2;
                    var techType = (NewBoostTech)reward.ValueId;
                    switch (techType)
                    {
                        case NewBoostTech.TroopTrainingSpeedMultiplier:/*14*/
                        case NewBoostTech.TroopTrainingTimeBonus:/*18*/
                            str = Localization.GetText("{0} Training Speedup", Helper.Localization.REWARDS);
                            title = string.Format(str, TimeHelper.ChangeSecondsToFormatTimeWords(reward.Value, true));
                            str2 = Localization.GetText("Reduces troop training time by {0}", Helper.Localization.REWARDS);
                            desc = string.Format(str2, TimeHelper.ChangeSecondsToFormatTimeWords(reward.Value));
                            break;

                        case NewBoostTech.TroopRecoverySpeedMultiplier:/*15*/
                        case NewBoostTech.TroopRecoveryTimeBonus:/*19*/
                            if (techType == NewBoostTech.TroopRecoverySpeedMultiplier)
                            {
                                str = Localization.GetText("{0} Recovery Boost", Helper.Localization.REWARDS);
                                str2 = Localization.GetText("Boost troop recovery speed for {0}", Helper.Localization.REWARDS);
                            }
                            else
                            {
                                str = Localization.GetText("{0} Recovery Speedup", Helper.Localization.REWARDS);
                                str2 = Localization.GetText("Reduces the recovery time of a troop by {0}", Helper.Localization.REWARDS);
                            }
                            title = string.Format(str, TimeHelper.ChangeSecondsToFormatTimeWords(reward.Value, true));
                            desc = string.Format(str2, TimeHelper.ChangeSecondsToFormatTimeWords(reward.Value));
                            break;
                        case NewBoostTech.TroopMarchingReductionMultiplier://27
                            str = Localization.GetText("{0} Marching Time Reduction", Helper.Localization.REWARDS);
                            title = string.Format(str, reward.Value + "%");
                            str2 = Localization.GetText("Reduces one-way marching time of the army by {0}", Helper.Localization.REWARDS);
                            desc = string.Format(str2, reward.Value + "%");
                            break;

                        case NewBoostTech.BuildingSpeedMultiplier:/*7*/
                        case NewBoostTech.BuildingTimeBonus:/*20*/
                            str = Localization.GetText("{0} Construction Speedup", Helper.Localization.REWARDS);
                            title = string.Format(str, TimeHelper.ChangeSecondsToFormatTimeWords(reward.Value, true));
                            str2 = Localization.GetText("Reduces the construction time by {0}", Helper.Localization.REWARDS);
                            desc = string.Format(str2, TimeHelper.ChangeSecondsToFormatTimeWords(reward.Value));
                            break;

                        case NewBoostTech.ResearchSpeedMultiplier:/*10*/
                        case NewBoostTech.ResearchTimeBonus:/*21*/
                            if (techType == NewBoostTech.ResearchSpeedMultiplier)
                            {
                                str = Localization.GetText("{0} Research Boost", Helper.Localization.REWARDS);
                                str2 = Localization.GetText("Boosts technology research speed for {0}", Helper.Localization.REWARDS);
                            }
                            else
                            {
                                str = Localization.GetText("{0} Research Speedup", Helper.Localization.REWARDS);
                                str2 = Localization.GetText("Reduces the research time by {0}", Helper.Localization.REWARDS);
                            }
                            title = string.Format(str, TimeHelper.ChangeSecondsToFormatTimeWords(reward.Value, true));
                            desc = string.Format(str2, TimeHelper.ChangeSecondsToFormatTimeWords(reward.Value));
                            break;
                    }
                    break;
            }

            return (title, desc, value);
        }
    }
}

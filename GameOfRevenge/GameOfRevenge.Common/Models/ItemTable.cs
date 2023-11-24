using System;
using System.Data;
using System.Drawing;
using System.Runtime.Serialization;
using GameOfRevenge.Common.Helper;
using GameOfRevenge.Common.Interface.Model.Table;
using GameOfRevenge.Common.Models.Boost;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models
{
    public interface IReadOnlyItemTable : IBaseDataTypeTable<int, int>, IReadOnlyBaseDataTypeTable<int, int>
    {
        int Id { get; } // PK
        bool IsTimeItem { get; }
        bool CanBuy { get; }
    }

    [DataContract]
    public class ItemTable : BaseDataTypeTable<int, int>, IBaseTable, IBaseDataTypeTable<int, int>, IReadOnlyBaseDataTypeTable<int, int>, IReadOnlyItemTable
    {
        [DataMember]
        public int Id { get; set; }
        public bool IsTimeItem { get; set; }
        public bool CanBuy { get; set; }

        public static ILocalizationBase Localization = new Localization();

        public override void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            Id = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            DataType = (DataType)(reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index)); index++;
            ValueId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Value = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            IsTimeItem = reader.GetValue(index) == DBNull.Value ? false : reader.GetBoolean(index); index++;
            CanBuy = reader.GetValue(index) == DBNull.Value ? false : reader.GetBoolean(index);
        }

        public override string ToString()
        {
            return DataType.ToString() + ": " + ValueId + ": " + Value;
        }

        public (string, string, string) GetProperties() => GetProperties(this);

        public static (string, string, string) GetProperties(ItemTable item)
        {
            string value;
            if (item.DataType == DataType.CityBoost || item.DataType == DataType.Technology || (item.DataType == DataType.Custom && (CustomType)item.ValueId == CustomType.VIPActivate))
            {
                if ((NewBoostTech)item.ValueId != NewBoostTech.TroopMarchingReductionMultiplier)
                {
                    value = TimeHelper.ChangeSecondsToTimeFormat(item.Value, false);
                }
                else
                {
                    value = item.Value.ToString();
                }
            }
            else
            {
                value = (item.Value >= 1000) ? AmountHelper.ToKMB(item.Value) : string.Format("{0:N0}", item.Value);
            }

            string str;
            string title = null;
            string desc = null;
            switch (item.DataType)
            {
                case DataType.Custom:
                    switch ((CustomType)item.ValueId)
                    {
                        case CustomType.KingExperiencePoints:
                            str = Localization.GetText("{0} Experience Points for your King", Helper.Localization.REWARDS);
                            title = string.Format(str, value);
                            str = Localization.GetText("Use this item to add {0:N0} Experience Points to your King", Helper.Localization.REWARDS);
                            desc = string.Format(str, item.Value);
                            break;
                        case CustomType.KingStaminaPoints:
                            str = Localization.GetText("{0} Stamina Points for your King", Helper.Localization.REWARDS);
                            title = string.Format(str, value);
                            str = Localization.GetText("Use this item to add {0:N0} Stamina Points to your King", Helper.Localization.REWARDS);
                            desc = string.Format(str, item.Value);
                            break;
                        case CustomType.VIPPoints:
                            str = Localization.GetText("{0} VIP Points", Helper.Localization.REWARDS);
                            title = string.Format(str, value);
                            str = Localization.GetText("Use this item to receive {0:N0} VIP Points", Helper.Localization.REWARDS);
                            desc = string.Format(str, item.Value);
                            break;
                        case CustomType.HeroPoints:
                            str = Localization.GetText("{0} Hero Points", Helper.Localization.REWARDS);
                            title = string.Format(str, value);
                            str = Localization.GetText("Use this item to receive {0:N0} Hero Points", Helper.Localization.REWARDS);
                            desc = string.Format(str, item.Value);
                            break;
                        case CustomType.VIPActivate:
                            str = Localization.GetText("{0} VIP Activate", Helper.Localization.REWARDS);
                            title = string.Format(str, TimeHelper.ChangeSecondsToFormatTimeWords(item.Value, camelcase: true));
                            str = Localization.GetText("Activate VIP for {0}", Helper.Localization.REWARDS);
                            desc = string.Format(str, TimeHelper.ChangeSecondsToFormatTimeWords(item.Value));
                            break;
                        case CustomType.WarHero:
                            str = Localization.GetText("{0} Hero Selection Activate", Helper.Localization.REWARDS);
                            title = string.Format(str, item.Value);
                            str = Localization.GetText("Activate Hero Selection for {0} positions", Helper.Localization.REWARDS);
                            desc = string.Format(str, item.Value);
                            break;
                    }
                    break;
                case DataType.Resource:
                    var type = (ResourceType)item.ValueId;
                    var resName = Localization.GetText(type.ToString(), Helper.Localization.ENUMS);
                    str = Localization.GetText("{0} {1}", Helper.Localization.REWARDS);
                    title = string.Format(str, value, resName);
                    str = Localization.GetText("Use it to receive {0:N0} {1}", Helper.Localization.REWARDS);
                    desc = string.Format(str, item.Value, resName);
                    break;
                case DataType.CityBoost:
                    string str3;
                    switch ((CityBoostType)item.ValueId)
                    {
                        case CityBoostType.Fog:
                            str = Localization.GetText("{0} Protection Fog", Helper.Localization.REWARDS);
                            title = string.Format(str, TimeHelper.ChangeSecondsToFormatTimeWords(item.Value, true));
                            str3 = Localization.GetText("Increase the protection fog time by {0}", Helper.Localization.REWARDS);
                            desc = string.Format(str3, TimeHelper.ChangeSecondsToFormatTimeWords(item.Value));
                            break;

                        case CityBoostType.Shield:
                            str = Localization.GetText("{0} Protection Shield", Helper.Localization.REWARDS);
                            title = string.Format(str, TimeHelper.ChangeSecondsToFormatTimeWords(item.Value, true));
                            str3 = Localization.GetText("Increase the protection shield time by {0}", Helper.Localization.REWARDS);
                            desc = string.Format(str3, TimeHelper.ChangeSecondsToFormatTimeWords(item.Value));
                            break;

                        case CityBoostType.ProductionBoost:
                            str = Localization.GetText("{0} Production Boost", Helper.Localization.REWARDS);
                            title = string.Format(str, TimeHelper.ChangeSecondsToFormatTimeWords(item.Value, true));
                            str3 = Localization.GetText("Increase the production boost time by {0}", Helper.Localization.REWARDS);
                            desc = string.Format(str3, TimeHelper.ChangeSecondsToFormatTimeWords(item.Value));
                            break;

                        case CityBoostType.Blessing:
                            str = Localization.GetText("{0} Blessing", Helper.Localization.REWARDS);
                            title = string.Format(str, TimeHelper.ChangeSecondsToFormatTimeWords(item.Value, true));
                            str3 = Localization.GetText("Increase blessing time by {0}", Helper.Localization.REWARDS);
                            desc = string.Format(str3, TimeHelper.ChangeSecondsToFormatTimeWords(item.Value));
                            break;

                        case CityBoostType.LifeSaver:
                            str = Localization.GetText("{0} Recovery Boost", Helper.Localization.REWARDS);
                            str3 = Localization.GetText("Boost troop recovery boost for {0}", Helper.Localization.REWARDS);

                            title = string.Format(str, TimeHelper.ChangeSecondsToFormatTimeWords(item.Value, true));
                            desc = string.Format(str3, TimeHelper.ChangeSecondsToFormatTimeWords(item.Value));
                            break;

                        case CityBoostType.TechBoost:
                            str = Localization.GetText("{0} Research Boost", Helper.Localization.REWARDS);
                            str3 = Localization.GetText("Boosts technology research speed for {0}", Helper.Localization.REWARDS);

                            title = string.Format(str, TimeHelper.ChangeSecondsToFormatTimeWords(item.Value, true));
                            desc = string.Format(str3, TimeHelper.ChangeSecondsToFormatTimeWords(item.Value));
                            break;

                    }
                    break;
                case DataType.Technology:
                    string str2;
                    var techType = (NewBoostTech)item.ValueId;
                    switch (techType)
                    {

                        case NewBoostTech.TroopTrainingSpeedMultiplier:/*14*/
                        case NewBoostTech.TroopTrainingTimeBonus:/*18*/
                            str = Localization.GetText("{0} Training Speedup", Helper.Localization.REWARDS);
                            title = string.Format(str, TimeHelper.ChangeSecondsToFormatTimeWords(item.Value, true));
                            str2 = Localization.GetText("Reduces troop training time by {0}", Helper.Localization.REWARDS);
                            desc = string.Format(str2, TimeHelper.ChangeSecondsToFormatTimeWords(item.Value));
                            break;

                        case NewBoostTech.TroopRecoverySpeedMultiplier:/*15*/
                        case NewBoostTech.TroopRecoveryTimeBonus:/*19*/
                            str = Localization.GetText("{0} Recovery Speedup", Helper.Localization.REWARDS);
                            str2 = Localization.GetText("Reduces the recovery time of a troop by {0}", Helper.Localization.REWARDS);
                            title = string.Format(str, TimeHelper.ChangeSecondsToFormatTimeWords(item.Value, true));
                            desc = string.Format(str2, TimeHelper.ChangeSecondsToFormatTimeWords(item.Value));
                            break;
                        case NewBoostTech.TroopMarchingReductionMultiplier://27
                            str = Localization.GetText("{0} Marching Time Reduction", Helper.Localization.REWARDS);
                            title = string.Format(str, item.Value + "%");
                            str2 = Localization.GetText("Reduces one-way marching time of the army by {0}", Helper.Localization.REWARDS);
                            desc = string.Format(str2, item.Value + "%");
                            break;

                        case NewBoostTech.BuildingSpeedMultiplier:/*7*/
                        case NewBoostTech.BuildingTimeBonus:/*20*/
                            str = Localization.GetText("{0} Construction Speedup", Helper.Localization.REWARDS);
                            title = string.Format(str, TimeHelper.ChangeSecondsToFormatTimeWords(item.Value, true));
                            str2 = Localization.GetText("Reduces the construction time by {0}", Helper.Localization.REWARDS);
                            desc = string.Format(str2, TimeHelper.ChangeSecondsToFormatTimeWords(item.Value));
                            break;

                        case NewBoostTech.ResearchSpeedMultiplier:/*10*/
                        case NewBoostTech.ResearchTimeBonus:/*21*/
                            str = Localization.GetText("{0} Research Speedup", Helper.Localization.REWARDS);
                            str2 = Localization.GetText("Reduces the research time by {0}", Helper.Localization.REWARDS);
                            title = string.Format(str, TimeHelper.ChangeSecondsToFormatTimeWords(item.Value, true));
                            desc = string.Format(str2, TimeHelper.ChangeSecondsToFormatTimeWords(item.Value));
                            break;
                    }
                    break;
            }

            return (title, desc, value);
        }
    }

    public class UserItem : ItemTable
    {
        public long PlayerDataId { get; set; }
        public int Count { get; set; }
    }
}

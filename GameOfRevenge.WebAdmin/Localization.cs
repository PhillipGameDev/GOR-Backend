using System;
namespace GameOfRevenge.WebAdmin
{
    public static class Localization
    {
        public const string MAIN_UI = "MainUI";
        public const string QUESTS = "Quests";
        public const string CHAPTERS_ID = "Chapters";
        public const string CHAPTERS_INFO = "Chapters.Info";
        public const string TUTORIAL = "Tutorial";
        public const string OTHER = "Other";
        public const string TIME = "Time";
        public const string ACADEMY = "Academy";
        public const string BATTLE_REPORT = "BattleReport";
        public const string BUILDING_UPGRADE = "BuildingUpgrade";
        public const string BUILDINGS_ID = "Buildings";
        public const string BUILDINGS_INFO = "Buildings.Info";
        public const string TECHNOLOGIES_ID = "Technologies";
        public const string TECHNOLOGIES_INFO = "Technologies.Info";
        public const string CITY_BOOSTS_ID = "CityBoosts";
        public const string CITY_BOOSTS_INFO = "CityBoosts.Info";
        public const string ACTIVE_BOOSTS = "ActiveBoosts";
        public const string BOOST_TECHS = "BoostTechs";
        public const string HEROES_ID = "Heroes";
        public const string HEROES_INFO = "Heroes.Info";
        public const string HEROES_PHRASE = "Heroes.Phrase";
        public const string SOLDIER = "Soldier";
        public const string ACCOUNT = "Account";
        public const string PLAYER_PROFILE = "PlayerProfile";
        public const string CHARACTER_SELECTION = "CharacterSelection";
        public const string KING = "King";
        public const string EQUIPMENT_ID = "Equipment";
        public const string EQUIPMENT_INFO = "Equipment.Info";
        public const string CHAT = "Chat";
        public const string INFIRMARY = "Infirmary";
        public const string SHOP = "Shop";
        public const string PRODUCTS_ID = "Products";
        public const string PRODUCTS_INFO = "Products.Info";
        public const string REWARDS = "Rewards";
        public const string VIP = "VIP";
        public const string ALLIANCE = "Alliance";
        public const string UNDER_ATTACK = "UnderAttack";
        public const string TROOP_SELECTION = "TroopSelection";
        public const string MAIL = "Mail";
        public const string FRIENDSHIP = "Friendship";
        public const string SEND_ARMY = "SendArmy";
        public const string COORDINATES = "Coordinates";
        public const string COLUMNS = "Columns";
        public const string LANGUAGE = "Language";
        public const string ENUMS = "Enums";

//        public static string LanguageCode { get; private set; } = DEFAULT_LANGUAGE;

        public static bool IsArabic => false;//(LanguageCode == "AR");

//        public static event Action<string> OnLanguageChange;

//        public static readonly string DEFAULT_LANGUAGE = (Application.systemLanguage == SystemLanguage.Arabic) ? "AR" : "EN";

/*        public static void Init()
        {
            SetLanguage(PlayerPrefs.GetString(ApiManager.LANGUAGE_KEY, DEFAULT_LANGUAGE));
        }

        public static void SetLanguage(string code)
        {
            if (code == null) code = DEFAULT_LANGUAGE;
            code = code.ToUpper();

            LocalizationManager.GetInstance().LoadTranslations(code);
            LanguageCode = code;
            OnLanguageChange?.Invoke(code);
            PlayerPrefs.SetString(ApiManager.LANGUAGE_KEY, code);
            PlayerPrefs.Save();
        }*/

/*        public static int GetIndex(string text, string groupName = OTHER)
        {
            return LocalizationManager.Instance.GetIndex(text, groupName);
        }

        public static string GetText(int index, string groupName = OTHER)
        {
            return LocalizationManager.Instance.GetText(index, groupName);
        }*/

        public static string GetText(string text, string groupName = OTHER)
        {
            return text;// LocalizationManager.Instance.GetText(text, groupName);
        }

/*        public static void SetText(TextMeshProUGUI component, string text, string groupName = null)
        {
            LocalizationManager.Instance.SetText(component, text, groupName, false);
        }

        public static void SetTextForced(TextMeshProUGUI component, string text, string groupName = null)
        {
            LocalizationManager.Instance.SetText(component, text, groupName, true);
        }

        public static void SetTexts(List<LocalizedTextComponent> components, string groupName)
        {
            LocalizationManager.Instance.SetTexts(components, groupName);
        }*/
    }
}

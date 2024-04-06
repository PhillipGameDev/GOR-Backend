namespace GameOfRevenge.Common.Helper
{
    public class Localization : ILocalizationBase
    {
        public const string MAIN_UI = "MainUI";
        public const string QUESTS = "Quests";
        public const string CHAPTERS_ID = "Chapters";
        public const string CHAPTERS_INFO = "Chapters.Info";
        public const string TUTORIAL = "Tutorial";
        public const string OTHER = "Other";
        public const string TIME = "Time";
        public const string ACADEMY = "Academy";
        public const string ACADEMY_ITEM_ID = "AcademyItem";
        public const string BLACK_SMITH = "BlackSmith";
        public const string BATTLE_REPORT = "BattleReport";
        public const string BUILDING_UPGRADE = "BuildingUpgrade";
        public const string BUILDINGS_ID = "Buildings";
        public const string BUILDINGS_INFO = "Buildings.Info";
        public const string MONSTERS = "Monsters";
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
        public const string AVATAR_SELECTION = "AvatarSelection";
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
        public const string MARCHING_ARMIES = "MarchingArmies";
        public const string COORDINATES = "Coordinates";
        public const string COLUMNS = "Columns";
        public const string LANGUAGE = "Language";
        public const string ENUMS = "Enums";
        public const string CLAN_ROLE = "ClanRole";
        public const string EVENT = "Event";


        //        private readonly ILocalizationBase localizationBase;

        //        public static string LanguageCode { get; private set; } = DEFAULT_LANGUAGE;

        public bool IsArabic => false;//(LanguageCode == "AR");

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

        public int GetIndex(string text, string groupName = OTHER)
        {
            throw new System.NotImplementedException();
//            return localizationBase.GetIndex(text, groupName);
        }

        public string GetText(int index, string groupName = OTHER)
        {
            throw new System.NotImplementedException();
//            return localizationBase.GetText(index, groupName);
        }

        public string GetText(string text, string groupName = OTHER)
        {
            return text;
//            return localizationBase.GetText(text, groupName);
        }

        public void SetText(object component, string text, string groupName, bool forced = false)
        {
            throw new System.NotImplementedException();
//            localizationBase.SetText(component, text, groupName, forced);
        }

        public void SetTextForced(object component, string text, string groupName)
        {
            throw new System.NotImplementedException();
//            localizationBase.SetText(component, text, groupName, true);
        }

        public void SetTexts(object components, string groupName)
        {
            throw new System.NotImplementedException();
//            localizationBase.SetTexts(components, groupName);
        }
    }

    public interface ILocalizationBase
    {
        bool IsArabic { get; }
        int GetIndex(string text, string groupName);
        string GetText(int index, string groupName);
        string GetText(string text, string groupName);
        void SetText(object component, string text, string groupName, bool forced);
        void SetTexts(object components, string groupName);
    }
}

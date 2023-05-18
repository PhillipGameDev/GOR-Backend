using GameOfRevenge.Model;

namespace GameOfRevenge
{
    public class GlobalConst
    {
        public static (string worldName, byte worldId, BoundingBox boundingBox, Vector tileDimention) GetPopWorld()
        {
            var bounding = new BoundingBox(new Vector(0, 0), new Vector(worldArea, worldArea));
            var tile = new Vector(tileArea, tileArea);
            return ("popWorld", 1, bounding, tile);
        }

        public const float worldArea = 1000;   //(1000*1000)/10*10 = 10000; tils
        public const float tileArea = 1;
        public const float TilesIaX = 10;
        public const float TilesIaY = 10;
        public const byte defaultWorldId = 1;

        // thats for land area 
        public const int minLandTileX = 406;
        public const int maXLandTileX = 580;
        public const int minLandTileY = 250;
        public const int maXLandTileY = 355;

        //thats for reborn areaTiles
        public const int minRebornTileX = 480;
        public const int maxRebornTileX = 512;
        public const int minRebornTileY = 330;
        public const int maxRebornTileY = 349;

        public const int ReBornTimeMs = 10000;
        public const double totalHealth = 2000;
        public const double turetTotalHealth = 1000;
        public const double animalTotalHealth = 1000;
        public const double healthIncreasePerSec = 40;
        public const int combatModeTime = 10000;
        public const int totalToolLevel = 7;
        public const int RespawnUniqueX = 504;
        public const int RespawnUniqueY = 345;

        public const double DefaultTroopTrainingTime = 60;
        public const double ResourceGenerateTime = 60 * 60;
        public const double DefaultAttackSpeed = 1;
        public const double DefaultAttackTime = 5000; //in milisecond
    }
}

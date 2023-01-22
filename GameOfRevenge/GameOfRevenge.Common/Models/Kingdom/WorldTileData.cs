namespace GameOfRevenge.Common.Models.Kingdom
{
    public class WorldTileData
    {
        public int PlayerId { get; set; }

        public WorldTileData(int playerId)
        {
            PlayerId = playerId;
        }
    }
}

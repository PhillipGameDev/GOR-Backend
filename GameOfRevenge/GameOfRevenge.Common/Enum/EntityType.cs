namespace GameOfRevenge.Common
{
//    [JsonConverter(typeof(StringEnumConverter))]
    public enum EntityType : byte
    {
        Default = 0,//for compatibility with older version, we should remove it later
        Player = 1,
        Monster = 2,
        Fortress = 3
    }
}

using GameOfRevenge.Common.Models.Table;
using Newtonsoft.Json;
using System;
using System.Data;

namespace GameOfRevenge.Common.Models.Kingdom
{
    public class WorldDataTable : BaseTable, IBaseTable
    {
        public int Id { get; set; }
        public int WorldId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public WorldTileData TileData { get; set; }

        public override void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            Id = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            WorldId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            X = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Y = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            TileData = reader.GetValue(index) == DBNull.Value ? default : JsonConvert.DeserializeObject<WorldTileData>(reader.GetString(index));
        }
    }
}

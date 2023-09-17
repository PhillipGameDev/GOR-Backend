using System;
using System.Data;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models
{
    public class PlayerID : IBaseTable
    {
        public int PlayerId { get; set; }
//        public int Invaded { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int WorldTileId { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            var index = 0;
            PlayerId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
//            Invaded = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            if (reader.FieldCount > 1)
            {
                X = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
                Y = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
                if (reader.FieldCount > 3)
                {
                    WorldTileId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index);
                }
            }
            else
            {
                X = -1;
                Y = -1;
            }
        }

        public PlayerID()
        {
        }

        public PlayerID(int playerID, int x, int y)
        {
            PlayerId = playerID;
            X = x;
            Y = y;
        }
    }
}

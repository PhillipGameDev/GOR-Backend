using System;
using System.Data;
using GameOfRevenge.Common.Interface.Model.Table;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models.Kingdom
{
    public class WorldTable : BaseRefStringTable, IBaseTable, IBaseRefEnumTable<string>, IReadOnlyBaseRefEnumTable<string>
    {
        public short ZoneX { get; set; }
        public short ZoneY { get; set; }
        public short ZoneSize { get; set; }
        public short CurrentZone { get; set; }

        public int SizeX => ZoneX * ZoneSize;
        public int SizeY => ZoneY * ZoneSize;
        public int TotalZones => ZoneX * ZoneY;

        public override void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            Id = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Name = reader.GetString(index) ?? string.Empty; index++;
            Code = reader.GetString(index) ?? default; index++;
            ZoneX = reader.GetValue(index) == DBNull.Value ? (short)0 : reader.GetInt16(index); index++;
            ZoneY = reader.GetValue(index) == DBNull.Value ? (short)0 : reader.GetInt16(index); index++;
            ZoneSize = reader.GetValue(index) == DBNull.Value ? (short)0 : reader.GetInt16(index); index++;
            CurrentZone = reader.GetValue(index) == DBNull.Value ? (short)0 : reader.GetInt16(index);
        }
    }

    public class WorldData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int PlayerId { get; set; }
    }
}

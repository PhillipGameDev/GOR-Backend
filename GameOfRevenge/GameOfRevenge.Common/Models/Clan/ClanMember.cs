﻿using GameOfRevenge.Common.Helper;
using GameOfRevenge.Common.Models.Table;
using System;
using System.Data;

namespace GameOfRevenge.Common.Models.Clan
{
    public class ClanMember : IBaseTable
    {
        public int Id { get; set; }
        public int ClanId { get; set; }
        public int PlayerId { get; set; }
        public string Name { get; set; }
        public ClanRole Role { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            Id = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            ClanId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Role = reader.GetValue(index) == DBNull.Value ? ClanRole.Other : reader.GetString(index).ToEnum<ClanRole>(); index++;
            PlayerId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Name = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            X = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Y = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index);
        }
    }
}

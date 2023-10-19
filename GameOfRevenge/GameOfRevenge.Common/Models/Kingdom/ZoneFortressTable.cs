using System;
using System.Data;
using GameOfRevenge.Common.Models.Table;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace GameOfRevenge.Common.Models.Kingdom
{
    [DataContract]
    public class ZoneFortressTable : BaseTable, IBaseTable
    {
        [DataMember]
        public int ZoneFortressId { get; set; }
        [DataMember]
        public int WorldId { get; set; }
        [DataMember]
        public short ZoneIndex { get; set; }
        [DataMember]
        public int HitPoints { get; set; }
        [DataMember]
        public int Attack { get; set; }
        [DataMember]
        public int Defense { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool Finished { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public int ClanId { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int PlayerId { get; set; }

        [DataMember]
        public string Data { get; set; }


        [DataMember(EmitDefaultValue = false)]
        public DateTime? StartTime { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public int Duration { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<PlayerTroops> PlayerTroops { get; set; }

        public override void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            ZoneFortressId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            WorldId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            ZoneIndex = reader.GetValue(index) == DBNull.Value ? (short)0 : reader.GetInt16(index); index++;
            HitPoints = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Attack = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Defense = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;

            Finished = reader.GetValue(index) == DBNull.Value ? false : reader.GetBoolean(index); index++;
            ClanId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Name = reader.GetValue(index) == DBNull.Value ? null : reader.GetString(index); index++;

            if (reader.FieldCount < 10) return;

            PlayerId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt32(index); index++;
            Data = reader.GetValue(index) == DBNull.Value ? null : reader.GetString(index);
            if (!string.IsNullOrEmpty(Data))
            {
                try
                {
                    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<ZoneFortressData>(Data);
                    StartTime = data.StartTime;
                    Duration = data.Duration;
                    PlayerTroops = data.PlayerTroops;
                }
                catch
                { }
            }
        }
    }
}

using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System;

namespace GameOfRevenge.Common.Models
{
    [DataContract]
    public class ZoneFortress : TimerBase
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

        [DataMember(EmitDefaultValue = false)]
        public DateTime? FirstCapturedTime { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<PlayerTroops> PlayerTroops { get; set; }

        public List<TroopInfos> GetAllTroops()
        {
            if (PlayerTroops == null) return null;

            var allTroops = PlayerTroops.SelectMany(playerTroops => playerTroops.Troops)
            .Where(troopInfos => (troopInfos.TroopData != null) && troopInfos.TroopData.Any())
            .GroupBy(troopInfos => new { troopInfos.TroopType, troopInfos.TroopData.First().Level })
            .Select(group => new TroopInfos()
            {
                TroopType = group.Key.TroopType,
                TroopData = group.SelectMany(troopInfos => troopInfos.TroopData)
                                    .GroupBy(troopDetails => troopDetails.Level)
                                    .Select(levelGroup => new TroopDetails()
                                    {
                                        Level = levelGroup.Key,
                                        Count = levelGroup.Sum(troop => troop.Count),
                                        Wounded = levelGroup.Sum(troop => troop.Wounded)
                                    }).ToList()
            }).ToList();

            return allTroops;
        }
    }

    [DataContract]
    public class PlayerTroops
    {
        [DataMember(EmitDefaultValue = false)]
        public int PlayerId { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public bool Recalled { get; set; }
        [DataMember]
        public List<TroopInfos> Troops { get; set; }

        public PlayerTroops()
        {
        }

        public PlayerTroops(int playerId, List<TroopInfos> troops)
        {
            PlayerId = playerId;
            Troops = troops;
        }
    }

    [DataContract]
    public class ZoneFortressData : TimerBase
    {
        [DataMember(EmitDefaultValue = false)]
        public DateTime? FirstCapturedTime { get; set; }
//        [DataMember(EmitDefaultValue = false)]
//        public bool Claimed { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<PlayerTroops> PlayerTroops { get; set; }
    }
}

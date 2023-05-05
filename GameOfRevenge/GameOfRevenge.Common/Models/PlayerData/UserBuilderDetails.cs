using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GameOfRevenge.Common.Models
{
    [DataContract]
    public class UserBuilderDetails
    {
        [DataMember]
        public int Location { get; set; }

        public int TimeLeft(List<StructureInfos> structures)
        {
            var bld = structures.SelectMany(x => x.Buildings).FirstOrDefault(x => (x.Location == Location));

            return (bld != null)? bld.TimeLeft : 0;
        }
    }

    [DataContract, Serializable]
    public class UserRecordBuilderDetails : UserBuilderDetails
    {
        [DataMember]
        public long Id { get; set; }

        public UserRecordBuilderDetails()
        {
        }

        public UserRecordBuilderDetails(long id, int location)
        {
            Id = id;
            Location = location;
        }
    }
}

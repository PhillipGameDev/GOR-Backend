using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GameOfRevenge.Common.Interface.Model.Table;
using GameOfRevenge.Common.Models.Boost;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GameOfRevenge.Common.Models
{
    [Serializable]
    [DataContract]
    public class UserNewBoost : TimerBase
    {
        //        [DataMember]
        public NewBoostType Type { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public byte Level { get; set; }
        /*#if UNITY_2019_1_OR_NEWER
                [DataMember]
                public string StartTime { get; set; }
        #else*/
        //        [DataMember]
        //        public DateTime StartTime { get; set; }
        //#endif
        //        [DataMember]
        //        public int Duration { get; set; }


        /*        private DateTime EndTime
                {
                    get
                    {
        /*#if UNITY_2019_1_OR_NEWER
                        DateTime.TryParse(EndTime, out DateTime endTime);
                        return endTime;
        #else* /
                        return StartTime.AddMilliseconds(Duration);
        //#endif
                    }
                }*/

        /*        public double TimeLeft
                {
                    get
                    {
                        double totalSeconds = (EndTime - DateTime.UtcNow).TotalSeconds;
                        if (totalSeconds < 0) totalSeconds = 0;
                        return totalSeconds;
                    }
                }*/
    }

    [Serializable]
    [DataContract]
    public class UserRecordNewBoost : UserNewBoost
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public new NewBoostType Type { get; set; }

        public UserRecordNewBoost()
        {
        }

        public UserRecordNewBoost(long id, UserNewBoost boost)
        {
            Id = id;
            Type = boost.Type;
            Level = boost.Level;
            StartTime = boost.StartTime;
            Duration = boost.Duration;
        }
    }

    /*    public interface IReadOnlyNewBoostTechDataRequirement
        {
            IReadOnlyList<IReadOnlyDataRequirement> Requirements { get; }
        }*/



/*    [Serializable]
    [DataContract]
    public class SpecVIPBoostDataTable : SpecNewBoostDataTable
    {
        [DataMember]
        public new List<SpecVIPBoostData> Boosts { get; set; } = new List<SpecVIPBoostData>();

        public SpecVIPBoostDataTable()
        {
        }
    }*/

    [Serializable]
    [DataContract]
    public class SpecNewBoostDataTable
    {
        [DataMember]
        public List<SpecNewBoostData> Boosts { get; set; } = new List<SpecNewBoostData>();
        [DataMember]
        public List<CityBoostType> CityBoosts { get; set; }
        [DataMember]
        public List<VIPBoostType> VIPBoosts { get; set; }
        [DataMember]
        public Dictionary<byte, Dictionary<byte, object>> Tables { get; set; } = new Dictionary<byte, Dictionary<byte, object>>();

        public SpecNewBoostDataTable()
        {
        }
    }


/*    [DataContract]
    public class SpecVIPBoostData : ISpecNewBoostData<BaseSpec>
    {
        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public NewBoostType Type { get; private set; }

        [DataMember]
        public IReadOnlyList<BaseSpec> Techs { get; private set; }

        [DataMember(EmitDefaultValue = false)]
        public byte Table { get; private set; }
//        [DataMember]
//        public IReadOnlyList<VIPBoostTechSpec> Techs { get; }
//        [DataMember]
//        public byte VIPLevels { get; }
        [JsonIgnore]
        public Dictionary<byte, object> Levels { get; set; }

        public SpecVIPBoostData(NewBoostType type, List<VIPBoostTechSpec> techs, byte table = 0, object tableData = null)
        {
//            VIPLevels = vipLevels;
            Type = type;
            Table = table;
            Techs = techs;
            if (tableData == null) return;

            byte idx = 0;
            var levels = new Dictionary<byte, object>();
            if (tableData.GetType() == typeof(int[]))
            {
                var array = (int[])tableData;
                foreach (var data in array)
                {
                    idx++;
                    if (int.TryParse(data.ToString(), out int val)) levels.Add(idx, data);
                }
            }
            else
            {
                var array = (float[])tableData;
                foreach (var data in array)
                {
                    idx++;
                    if (float.TryParse(data.ToString(), out float val)) levels.Add(idx, data);
                }
            }
            Levels = levels;
        }
    }*/

/*    public interface ISpecNewBoostData
    {
        NewBoostType Type { get; }

        IReadOnlyList<NewBoostTechSpec> Techs { get; }

        byte Table { get; }

        Dictionary<byte, object> Levels { get; set; }
    }*/

    //    [Serializable]
    [DataContract]
    public class SpecNewBoostData// : IReadOnlyNewBoostTechDataRequirement
    {
        //        public long Id { get; set; }

        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public NewBoostType Type { get; private set; }

        [DataMember]
        public IReadOnlyList<NewBoostTechSpec> Techs { get; private set; }

        [DataMember(EmitDefaultValue = false)]
        public byte Table { get; private set; }

//        [DataMember(EmitDefaultValue = false)]
//        public IReadOnlyList<IReadOnlyNewBoostLevel> Levels { get; }
        [JsonIgnore]
        public Dictionary<byte, object> Levels { get; set; }

        public SpecNewBoostData(NewBoostType type, List<NewBoostTechSpec> techs, byte table = 0, object tableData = null)//, List<IReadOnlyNewBoostLevel> levels)
        {
            Type = type;
            Table = table;
            Techs = techs;
            if (tableData == null) return;

            byte idx = 0;
            var levels = new Dictionary<byte, object>();
            if (tableData.GetType() == typeof(int[]))
            {
                var array = (int[])tableData;
                foreach (var data in array)
                {
                    idx++;
                    if (int.TryParse(data.ToString(), out int val)) levels.Add(idx, data);
                }
            }
            else
            {
                var array = (float[])tableData;
                foreach (var data in array)
                {
                    idx++;
                    if (float.TryParse(data.ToString(), out float val)) levels.Add(idx, data);
                }
            }
            Levels = levels;
        }
    }

    public class IReadOnlyNewBoostLevel
    {
        public byte Level { get; }
        public int TimeTaken { get; }
        public IReadOnlyList<IReadOnlyDataRequirement> Requirements => requirements;

        private List<DataRequirement> requirements;

        /*        public IReadOnlyNewBoostLevel(byte level, int timeTaken, List<DataRequirement> requirements)
                {
                    Level = level;
                    TimeTaken = timeTaken;
                    this.requirements = requirements;
                }*/
    }

/*    [DataContract]
    public class VIPBoostTechSpec : NewBoostTechSpec
    {
        [DataMember]
        public byte StartVIPLevel { get; set; }

        public VIPBoostTechSpec()
        {
        }

        public VIPBoostTechSpec(NewBoostTech tech, byte table, object tableData, byte startLevel, string format = null, string column = null) : base(tech, table, tableData, format, column)
        {
            StartVIPLevel = startLevel;
        }
    }*/

    //    [Serializable]
    [DataContract]
    public class NewBoostTechSpec
    {
        [DataMember(EmitDefaultValue = false)]
        public byte StartLevel { get; set; }

        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public NewBoostTech Tech { get; set; }

        //        [JsonIgnore]
        [DataMember(EmitDefaultValue = false)]
        public byte Table { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Column { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Format { get; set; }
        //        public int StartLevel { get; }
        [JsonIgnore]
        public Dictionary<byte, object> Levels { get; set; }

        public NewBoostTechSpec()
        {
        }

        public NewBoostTechSpec(NewBoostTech tech, byte table, object tableData, string format = null, string column = null) : this(tech, table, tableData, 0, format, column)
        {
        }

        public NewBoostTechSpec(NewBoostTech tech, byte table, object tableData, byte startLevel, string format = null, string column = null)// : base(tech, table)
        {
            StartLevel = startLevel;
            Tech = tech;
            Table = table;
            Column = column;
            Format = format;
            if (tableData == null) return;

            byte idx = 0;
            var levels = new Dictionary<byte, object>();
            if (tableData.GetType() == typeof(int[]))
            {
                var array = (int[])tableData;
                foreach (var data in array)
                {
                    idx++;
                    if (int.TryParse(data.ToString(), out int val)) levels.Add(idx, data);
                }
            }
            else
            {
                var array = (float[])tableData;
                foreach (var data in array)
                {
                    idx++;
                    if (float.TryParse(data.ToString(), out float val)) levels.Add(idx, data);
                }
            }
            Levels = levels;
        }

        /*        public NewBoostTechSpec(NewBoostTech tech, byte table)//, T[] tableData)//, int startLevel = 1)
                {
                    Tech = tech;
                    Table = table;
         //           TableData = tableData;
        //            StartLevel = startLevel;
                }*/
    }

/*    public class NewBoostTechSpecDetailed<T>// : NewBoostTechSpec
    {
        //        [JsonProperty(Order = 1)]
        [JsonIgnore]
        public Dictionary<byte, T> Levels { get; }

        public NewBoostTechSpecDetailed(NewBoostTech tech, byte table, object tableData)// : base(tech, table)
        {
            T[] array = (T[])tableData;

            byte idx = 0;
            var levels = new Dictionary<byte, T>();
            foreach (var data in array)
            {
                idx++;
                float.TryParse(data.ToString(), out float val);
                if (val != 0) levels.Add(idx, data);
            }
            Levels = levels;
        }
    }*/


    /*    public interface IReadOnlyNewBoostDataTable : IReadOnlyBaseRefEnumLevelDataTable
        {
            int Value { get; }
            int TimeTaken { get; }
        }*/

}

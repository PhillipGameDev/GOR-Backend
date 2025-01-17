﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GameOfRevenge.Common.Models
{
    [DataContract]
    public class UserVIPDetails : TimerBase
    {
        private static readonly List<int> levels = new List<int>() {
0,
130,
500,
1000,
2000,
3000,
6000,
11500,
22000,
30000,
45000,
70000,
125000,
170000,
210000,
280000,
330000,
390000,
440000,
500000
};

        public static int VIPLevel(int pts)
        {
            //TODO: improve this when we remove levels array from this class.
            //we should order levels and get the lowest value
            var idx = 1;
            if (pts > 0)
            {
                int basePts = 0;
                int len = levels.Count-1;
                for (idx = 0; idx < len; idx++)
                {
                    if (pts >= (basePts + levels[idx]))
                    {
                        basePts += levels[idx];
                        continue;
                    }

                    break;
                }
//                    idx--;
            }
            return idx;
        }

        [DataMember]
        public int Points { get; set; }

//#if UNITY_2019_4_OR_NEWER
//        [DataMember]
//        public int Level { get; set; }
//#else
        [DataMember]
        public int Level => VIPLevel(Points);
//#endif

        public UserVIPDetails()
        {
        }

        public UserVIPDetails(int pts)
        {
            Points = pts;
        }
    }
}

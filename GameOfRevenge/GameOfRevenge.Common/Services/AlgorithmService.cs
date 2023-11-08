using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfRevenge.Common.Services
{
    public class AlgorithmService
    {
        public static int GetValue(AlgorithmType algorithmType, int initValue, int key, int level)
        {
            switch (algorithmType)
            {
                case AlgorithmType.Linear: return initValue + key * level;
                case AlgorithmType.Multiple: return initValue * (int)Math.Pow(key, level);
            }

            return 0;
        }
    }

}

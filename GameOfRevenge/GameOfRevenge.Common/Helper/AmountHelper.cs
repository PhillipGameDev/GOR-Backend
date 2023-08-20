using System;
using System.Globalization;

namespace GameOfRevenge.Common.Helper
{
    public static class AmountHelper
    {
        public static float ToSliderAmount(float timeLeft, float totalTime)
        {
            if (totalTime > 86400) //24h
            {
                return 1;
            }
            else
            {
                return timeLeft / totalTime;
            }
        }

        public static string ToIntKMB(long value, bool floor = false)
        {
            if (value > 999999999)
            {
                if (floor) value = (long)Math.Floor((double)value / 1000000000) * 1000000000;
                return value.ToString("0,,,B", CultureInfo.InvariantCulture);
            }
            else if (value > 999999)
            {
                if (floor) value = (long)Math.Floor((double)value / 1000000) * 1000000;
                return value.ToString("0,,M", CultureInfo.InvariantCulture);
            }
            else if (value > 999)
            {
                if (floor) value = (long)Math.Floor((double)value / 1000) * 1000;
                return value.ToString("0,K", CultureInfo.InvariantCulture);
            }
            else
            {
                return value.ToString(CultureInfo.InvariantCulture);
            }
        }

        public static string ToKMB(long value, bool floor = false)
        {
            if (value > 999999999)
            {
                if (floor) value = (long)Math.Floor((double)value / 100000000) * 100000000;
                return value.ToString("0,,,.###B", CultureInfo.InvariantCulture);
            }
            else if (value > 999999)
            {
                if (floor) value = (long)Math.Floor((double)value / 100000) * 100000;
                return value.ToString("0,,.##M", CultureInfo.InvariantCulture);
            }
            else if (value > 999)
            {
                if (floor) value = (long)Math.Floor((double)value / 100) * 100;
                return value.ToString("0,.#K", CultureInfo.InvariantCulture);
            }
            else
            {
                return value.ToString(CultureInfo.InvariantCulture);
            }
        }

        public static string ToIntK(long value)
        {
            double valueK = value / 1000f;
            return valueK.ToString("#,0K");
        }

        public static string ToK(long value)
        {
            value = ((long)(value / 100) * 100);
            double valueK = value / 1000f;
            return valueK.ToString("#,0.0K");
        }

        public static double GetTotalSeconds(string startTime, string endTime)
        {
            return Convert.ToDateTime(endTime).Subtract(Convert.ToDateTime(startTime)).TotalSeconds;
        }
    }
}

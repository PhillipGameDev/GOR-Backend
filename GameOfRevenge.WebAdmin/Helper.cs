namespace GameOfRevenge.WebAdmin
{
    using System;
    using System.Globalization;
//    using UnityEngine;
//    using UnityEngine.EventSystems;

    public static class Helper
    {
//        private static List<RaycastResult> results = new List<RaycastResult>();
        private static CultureInfo arabicCulture = new CultureInfo("ar-SA");
        public static string ChangeSecondsToFormatTimeWords(int seconds, bool camelcase = false)
        {
            var cd = "d";
            var ch = "h";
            var cm = "m";
            var cs = "s";
            if (!Localization.IsArabic && camelcase)
            {
                cd = "D";
                ch = "H";
                cm = "M";
                cs = "S";
            }
            var time = TimeSpan.FromSeconds(seconds);
            string str = string.Empty;
            if ((int)time.TotalDays > 0)
            {
                int days = (int)time.TotalDays;
                str = Localization.GetText("{0} " + ((days == 1) ? $"{cd}ay" : $"{cd}ays"), Localization.TIME);
                str = string.Format(str, days);
            }
            else
            {
                var h = (int)time.TotalHours;
                var m = time.Minutes;
                var s = time.Seconds;
                int count = ((h > 0) ? 1 : 0) + ((m > 0) ? 1 : 0) + ((s > 0) ? 1 : 0);
                if (count == 3)
                {
                    str = string.Format(Localization.GetText("{0}" + ch, Localization.TIME), (int)time.TotalHours) + " " +
                        string.Format(Localization.GetText("{0}" + cm, Localization.TIME), m) + " " +
                        string.Format(Localization.GetText("{0}" + cs, Localization.TIME), s);
                }
                else if (count > 1)
                {
                    if (h > 0) str = string.Format(Localization.GetText("{0}" + ch + "r", Localization.TIME), h) + " ";
                    if (m > 0) str += string.Format(Localization.GetText("{0}" + cm + "in", Localization.TIME), m) + " ";
                    if (s > 0) str += string.Format(Localization.GetText("{0}" + cs + "ec", Localization.TIME), s);
                    str = str.TrimEnd();
                }
                else
                {
                    if (h > 0)
                    {
                        str = string.Format(Localization.GetText("{0} " + ((h > 1) ? $"{ch}ours" : $"{ch}our"), Localization.TIME), h);
                    }
                    if (m > 0)
                    {
                        str = string.Format(Localization.GetText("{0} " + ((m > 1) ? $"{cm}inutes" : $"{cm}inute"), Localization.TIME), m);
                    }
                    if (s > 0)
                    {
                        str = string.Format(Localization.GetText("{0} " + ((s > 1) ? $"{cs}econds" : $"{cs}econd"), Localization.TIME), s);
                    }
                }
            }

            return str;
        }

        public static string ChangeSecondsToMessageFormat(double seconds, bool fulldate = false)
        {
            return ChangeSecondsToMessageFormat(TimeSpan.FromSeconds(seconds), fulldate);
        }

        public static string ChangeSecondsToMessageFormat(TimeSpan time, bool fulldate = false)
        {
            string str;
            if (fulldate || (time.TotalDays >= 28))
            {
                str = DateTime.Now.AddSeconds(-time.TotalSeconds).ToString("MM/dd/yyyy h:mm tt", Localization.IsArabic ? arabicCulture : null);
            }
            else if ((int)time.TotalDays > 0)
            {
                int days = (int)time.TotalDays;
                str = Localization.GetText((days == 1) ? "{0} day ago" : "{0} days ago", Localization.TIME);
                str = string.Format(str, days);
            }
            else if ((int)time.TotalHours > 0)
            {
                str = Localization.GetText("{0}h {1}m ago", Localization.TIME);
                str = string.Format(str, (int)time.TotalHours, time.Minutes);
            }
            else if ((int)time.TotalMinutes > 0)
            {
                str = Localization.GetText("{0}m ago", Localization.TIME);
                str = string.Format(str, time.Minutes);
            }
            //            else if (time.Seconds > 1)
            //            {
            //                str = string.Format("{0}s ago", time.Seconds);
            //            }
            else
            {
                str = Localization.GetText("Now", Localization.TIME);
            }
            return str;
        }

        public static string ChangeSecondsToTimeFormat(double seconds, bool clock = true)
        {
            return ChangeSecondsToTimeFormat(TimeSpan.FromSeconds(seconds), clock);
        }

        public static string ChangeSecondsToTimeFormat(TimeSpan time, bool clock = true)
        {
            string str = string.Empty;
            if ((int)time.TotalDays > 0)
            {
                int days = (int)time.TotalDays;
                str = Localization.GetText((days == 1) ? "{0} day" : "{0} days", Localization.TIME);
                str = string.Format(str, days);
            }
            else if (clock)
            {
                if ((int)time.TotalHours > 0)
                {
                    str = string.Format("{0}:{1:D2}:{2:D2}", (int)time.TotalHours, time.Minutes, time.Seconds);
                }
                else
                {
                    str = string.Format("{0}:{1:D2}", time.Minutes, time.Seconds);
                }
            }
            else
            {
                if ((int)time.TotalHours > 0)
                {
                    str = Localization.GetText("{0}h {1}m", Localization.TIME);
                    str = string.Format(str, (int)time.TotalHours, time.Minutes);
                }
                else if ((int)time.TotalMinutes > 0)
                {
                    str = Localization.GetText("{0}m", Localization.TIME);
                    str = string.Format(str, time.Minutes);
                }
            }

            return str;
        }

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

        public static string ParseByCase(string strInput)
        {
            string strOutput = "";
            if (!string.IsNullOrWhiteSpace(strInput))
            {
                int currCharPos = 0;
                int intLastCharPos = strInput.Length - 1;
                for (currCharPos = 0; currCharPos <= intLastCharPos; currCharPos++)
                {
                    char currInputChar = strInput[currCharPos];
                    char prevInputChar = currInputChar;
                    if (currCharPos > 0)
                    {
                        prevInputChar = strInput[currCharPos - 1];
                    }
                    else
                    {
                        currInputChar = char.ToUpper(currInputChar);
                        prevInputChar = currInputChar;
                    }

                    if (char.IsUpper(currInputChar) && char.IsLower(prevInputChar)) strOutput += " ";

                    strOutput += currInputChar;
                }
            }

            return strOutput;
        }

/*        public static bool IsPointerOverUIObject()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            results.Clear();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

            return results.Count > 0;
        }*/
    }
}

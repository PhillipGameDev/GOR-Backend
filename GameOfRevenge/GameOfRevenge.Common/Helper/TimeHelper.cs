using System;
using System.Globalization;

namespace GameOfRevenge.Common.Helper
{
    public static class TimeHelper
    {
        private static readonly CultureInfo arabicCulture = new CultureInfo("ar-SA");

        public static ILocalizationBase Localization = new Localization();

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
                str = Localization.GetText("{0} " + ((days == 1) ? $"{cd}ay" : $"{cd}ays"), Helper.Localization.TIME);
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
                    str = string.Format(Localization.GetText("{0}" + ch, Helper.Localization.TIME), (int)time.TotalHours) + " " +
                        string.Format(Localization.GetText("{0}" + cm, Helper.Localization.TIME), m) + " " +
                        string.Format(Localization.GetText("{0}" + cs, Helper.Localization.TIME), s);
                }
                else if (count > 1)
                {
                    if (h > 0) str = string.Format(Localization.GetText("{0}" + ch + "r", Helper.Localization.TIME), h) + " ";
                    if (m > 0) str += string.Format(Localization.GetText("{0}" + cm + "in", Helper.Localization.TIME), m) + " ";
                    if (s > 0) str += string.Format(Localization.GetText("{0}" + cs + "ec", Helper.Localization.TIME), s);
                    str = str.TrimEnd();
                }
                else
                {
                    if (h > 0)
                    {
                        str = string.Format(Localization.GetText("{0} " + ((h > 1) ? $"{ch}ours" : $"{ch}our"), Helper.Localization.TIME), h);
                    }
                    if (m > 0)
                    {
                        str = string.Format(Localization.GetText("{0} " + ((m > 1) ? $"{cm}inutes" : $"{cm}inute"), Helper.Localization.TIME), m);
                    }
                    if (s > 0)
                    {
                        str = string.Format(Localization.GetText("{0} " + ((s > 1) ? $"{cs}econds" : $"{cs}econd"), Helper.Localization.TIME), s);
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
                str = Localization.GetText((days == 1) ? "{0} day ago" : "{0} days ago", Helper.Localization.TIME);
                str = string.Format(str, days);
            }
            else if ((int)time.TotalHours > 0)
            {
                str = Localization.GetText("{0}h {1}m ago", Helper.Localization.TIME);
                str = string.Format(str, (int)time.TotalHours, time.Minutes);
            }
            else if ((int)time.TotalMinutes > 0)
            {
                str = Localization.GetText("{0}m ago", Helper.Localization.TIME);
                str = string.Format(str, time.Minutes);
            }
            //            else if (time.Seconds > 1)
            //            {
            //                str = string.Format("{0}s ago", time.Seconds);
            //            }
            else
            {
                str = Localization.GetText("Now", Helper.Localization.TIME);
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
                str = Localization.GetText((days == 1) ? "{0} day" : "{0} days", Helper.Localization.TIME);
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
                    str = Localization.GetText("{0}h {1}m", Helper.Localization.TIME);
                    str = string.Format(str, (int)time.TotalHours, time.Minutes);
                }
                else if ((int)time.TotalMinutes > 0)
                {
                    str = Localization.GetText("{0}m", Helper.Localization.TIME);
                    str = string.Format(str, time.Minutes);
                }
            }

            return str;
        }


        public static string ParseByCase(string strInput)
        {
            string strOutput = "";
            if (!string.IsNullOrWhiteSpace(strInput))
            {
                int intLastCharPos = strInput.Length - 1;
                for (var currCharPos = 0; currCharPos <= intLastCharPos; currCharPos++)
                {
                    char currInputChar = strInput[currCharPos];
                    char prevInputChar;
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
